using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask grassMask;
    public LayerMask roadMask;
    public LayerMask unwalkableMask;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public TerrainType[] walkableRegions;
    public int obstacleProximityPenalty = 10; //Penalty for being near unwalkable objects
    public float steepnessLimit;
    LayerMask walkableMask;
    public Node[,] grid;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    float nodeDiameter;
    int gridSizeX, gridSizeZ;
    int raycastMask;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

    private void Awake()
    {
        raycastMask = grassMask | roadMask | unwalkableMask;
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
    }

    private void Start()
    {
        CreateGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown("z") && displayGridGizmos == true)
            displayGridGizmos = false;
        else if (Input.GetKeyDown("z") && displayGridGizmos == false)
            displayGridGizmos = true;
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeZ;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeZ]; //create 3d grid
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2; //gets bottom left point as a reference point

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                int movementPenalty = 0;

                Ray regionsRay = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                Ray ray = new Ray(worldPoint + Vector3.up * (gridWorldSize.y + 20), Vector3.down); //ray that fires at ground
                RaycastHit hit;
                if (Physics.Raycast(regionsRay, out hit, 100, walkableMask))
                {
                    walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }

                if (!walkable)
                {
                    movementPenalty += obstacleProximityPenalty;
                }

                if (Physics.Raycast(ray, out hit, gridWorldSize.y + 50, raycastMask))
                {
                    bool _walkable;
                    if (hit.transform.gameObject.layer == 8) //if the gameobject is on the unwalkable layer
                        _walkable = false;
                    else
                        _walkable = true;

                    float slopeAngle = Vector3.Angle(transform.up, hit.normal);
                    if (slopeAngle > steepnessLimit)
                    {
                        _walkable = false;
                    }
                    else
                    {
                        movementPenalty += Mathf.RoundToInt(slopeAngle / 3);
                    }
                    worldPoint = new Vector3(worldPoint.x, hit.point.y, worldPoint.z); //change worldpoint y position to the rays hit y position
                    grid[x, z] = new Node(_walkable, worldPoint, x, Mathf.RoundToInt(hit.point.y), z, movementPenalty, slopeAngle);
                }
                else //if there is no ground or obstacle to hit
                {
                    grid[x, z] = new Node(false, worldPoint, x, Mathf.RoundToInt(worldPoint.y), z, 1, 90);
                }
            }
        }

        BlurPenaltyMap(3);
    }

    void BlurPenaltyMap(int blurSize) //NEEDS REWRITING AND COMMENTS
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;
        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeZ];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeZ];

        for (int z = 0; z < gridSizeZ; z++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, z] += grid[sampleX, z].movementPenalty;
            }
            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                penaltiesHorizontalPass[x, z] = penaltiesHorizontalPass[x - 1, z] - grid[removeIndex, z].movementPenalty + grid[addIndex, z].movementPenalty;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int z = 1; z < gridSizeZ; z++)
            {
                int removeIndex = Mathf.Clamp(z - kernelExtents - 1, 0, gridSizeZ);
                int addIndex = Mathf.Clamp(z + kernelExtents, 0, gridSizeZ - 1);

                penaltiesVerticalPass[x, z] = penaltiesVerticalPass[x, z - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, z] / (kernelSize * kernelSize));
                grid[x, z].movementPenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax)
                    penaltyMax = blurredPenalty;
                if (blurredPenalty < penaltyMin)
                    penaltyMin = blurredPenalty;
            }
        }
    }

    public List<Node> GetNeighbouringNodes(Node n)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) //iterates through all the neighbouring nodes
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //skip this node because it's the centre node of the 3x3
                    continue;

                int checkX = n.gridX + x;
                int checkY = n.gridZ + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeZ) //ensures the x or y coordinate is valid
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node GetNodeInWorld(Vector3 worldPos)
    {
        //calculates what percentage the world position is in the grid
        Vector2 posPercentages = new Vector2((worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x, (worldPos.z + gridWorldSize.z / 2) / gridWorldSize.z);
        //ensure its not outside the precentage range of 0-1
        posPercentages.x = Mathf.Clamp01(posPercentages.x);
        posPercentages.y = Mathf.Clamp01(posPercentages.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * posPercentages.x);
        int y = Mathf.RoundToInt((gridSizeZ - 1) * posPercentages.y);
        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, gridWorldSize.z));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                if (n.walkable)
                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                else
                    Gizmos.color = Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter/1.2f));
            }
        }
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
