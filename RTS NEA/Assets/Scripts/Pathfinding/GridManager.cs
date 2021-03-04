using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask grassMask;
    public LayerMask roadMask;
    public LayerMask unwalkableMask;
    Vector3 gridWorldSize;
    public float nodeRadius; //Don't change variable name, it throws an "overflow exception" somehow
    public TerrainType[] walkableRegions;
    public int unwalkablePenalty = 10; //Penalty for being near unwalkable objects
    public float steepnessLimit;
    LayerMask walkableMask;
    public Node[,] grid;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    float nodeDiameter;
    int gridSizeX, gridSizeZ;
    int raycastMask;

    int minPenalty = int.MaxValue;
    int maxPenalty = int.MinValue;

    void Awake()
    {
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown("z") && displayGridGizmos == true)
            displayGridGizmos = false;
        else if (Input.GetKeyDown("z") && displayGridGizmos == false)
            displayGridGizmos = true;
    }

    public void StartGridCreation()
    {
        TerrainManager terrainManager = GameObject.Find("Terrain Manager").GetComponent<TerrainManager>();
        gridWorldSize = new Vector3(terrainManager.mapSize * GenerationValues.GetChunkSize(), 100, terrainManager.mapSize * GenerationValues.GetChunkSize());
        raycastMask = grassMask | roadMask | unwalkableMask;
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }

        CreateGrid();
    }

    public int GetMaxSize
    {
        get
        {
            return gridSizeX * gridSizeZ;
        }
    }

    ///<summary> Creates a grid of nodes used for pathfinding with unwalkable and walkable regions </summary>
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeZ]; //Create 3d grid
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2; //Gets bottom left point as a reference point

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                int movementPenalty = 0;

                //Ray regionsRay = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                Ray ray = new Ray(worldPoint + Vector3.up * (gridWorldSize.y + 20), Vector3.down); //Ray that fires at ground from up in the sky
                if (Physics.Raycast(ray, out RaycastHit hit, 100, walkableMask))
                {
                    walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty); //Gets the corresponding movement penalty from the terrain type
                }

                if (Physics.Raycast(ray, out hit, gridWorldSize.y + 50, raycastMask))
                {
                    if (hit.transform.gameObject.layer == 8) //If the gameobject is on the unwalkable layer it is set to unwalkable
                        walkable = false;
                    else
                        walkable = true;

                    //Sets nodes to unwalkable if too steep, otherwise the movement penalty is increased
                    float slopeAngle = Vector3.Angle(transform.up, hit.normal);
                    if (slopeAngle > steepnessLimit)
                    {
                        walkable = false;
                    }
                    else
                    {
                        movementPenalty += Mathf.RoundToInt(slopeAngle / 3);
                    }

                    if (!walkable)
                    {
                        movementPenalty += unwalkablePenalty;
                    }

                    worldPoint = new Vector3(worldPoint.x, hit.point.y, worldPoint.z); //Change worldpoint y position to the ray's hit y position
                    grid[x, z] = new Node(walkable, worldPoint, x, Mathf.RoundToInt(hit.point.y), z, movementPenalty, slopeAngle);
                }
                else //If there is no ground or obstacle to hit
                {
                    grid[x, z] = new Node(false, worldPoint, x, Mathf.RoundToInt(worldPoint.y), z, 1, 90);
                }
            }
        }

        BlurNodePenalties(3);
    }
    ///<summary> Uses the box blur algorithm to blur the penalties of nodes </summary>
    /// <param name="blurSize"> Dictates the amount of neighbouring nodes used to blur </param>
    void BlurNodePenalties(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1; //Ensures the kernel size is always odd
        int kernelBounds = (kernelSize - 1) / 2; //The number of nodes between the centre and the edge
        int[,] horizontalPenalties = new int[gridSizeX, gridSizeZ]; //Stores the results of the first, horizontal iteration of all the nodes
        int[,] verticalPenalties = new int[gridSizeX, gridSizeZ]; //Stores the results of the second, veritcal iteration of all the nodes

        //First horizontal iteration goes through finding the horizontal total of each node in the grid array
        for (int z = 0; z < gridSizeZ; z++)
        {
            for (int x = -kernelBounds; x <= kernelBounds; x++)
            {
                int currentX = Mathf.Clamp(x, 0, kernelBounds);
                horizontalPenalties[0, z] += grid[currentX, z].movementPenalty;
            }
            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelBounds - 1, 0, gridSizeX); //The index of the node that will be removed from the total, just leaving the left edge
                int addIndex = Mathf.Clamp(x + kernelBounds, 0, gridSizeX - 1); //The index of the node that will be added to the total on the right edge

                //The new penalty is the previous node, minus the penalty of the one leaving the kernel and plus the one entering the kernel
                horizontalPenalties[x, z] = horizontalPenalties[x - 1, z] - grid[removeIndex, z].movementPenalty + grid[addIndex, z].movementPenalty; 
            }
        }

        //Second vertical iteration goes through the horizontal penalties array, finding the vertical total of each node
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelBounds; y <= kernelBounds; y++)
            {
                int currentY = Mathf.Clamp(y, 0, kernelBounds);
                verticalPenalties[x, 0] += horizontalPenalties[x, currentY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)verticalPenalties[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int z = 1; z < gridSizeZ; z++)
            {
                int removeIndex = Mathf.Clamp(z - kernelBounds - 1, 0, gridSizeZ);
                int addIndex = Mathf.Clamp(z + kernelBounds, 0, gridSizeZ - 1);

                verticalPenalties[x, z] = verticalPenalties[x, z - 1] - horizontalPenalties[x, removeIndex] + horizontalPenalties[x, addIndex];

                blurredPenalty = Mathf.RoundToInt(verticalPenalties[x, z] / (kernelSize * kernelSize)); //The final average of both iterations rounded to the closest integer
                grid[x, z].movementPenalty = blurredPenalty;

                if (blurredPenalty > maxPenalty)
                    maxPenalty = blurredPenalty;
                if (blurredPenalty < minPenalty)
                    minPenalty = blurredPenalty;
            }
        }
    }

    ///<summary> Returns a list of the nodes neighbouring the inputted one </summary>
    public List<Node> GetNeighbouringNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) //Iterates through all the neighbouring nodes
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //Skip this node because it's the centre node of the 3x3
                    continue;

                int currentX = node.gridX + x;
                int currentY = node.gridZ + y;

                if (currentX >= 0 && currentX < gridSizeX && currentY >= 0 && currentY < gridSizeZ) //Ensures the x or y coordinate is valid
                {
                    neighbours.Add(grid[currentX, currentY]);
                }
            }
        }
        return neighbours;
    }

    ///<summary> Returns a node in the grid from a world position </summary>
    public Node GetNodeFromPosition(Vector3 worldPosition)
    {
        //Calculates what percentage the world position is in the grid
        Vector2 posPercentages = new Vector2((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x, (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z);

        //Ensure its not outside the percentage range of 0-1
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
                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(minPenalty, maxPenalty, n.movementPenalty));
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
