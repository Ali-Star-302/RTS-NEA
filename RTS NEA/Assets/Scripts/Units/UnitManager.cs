using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : MonoBehaviour
{
    public Dictionary<string, int> teamOneUnits = new Dictionary<string, int>() {
        { "Pikeman", 0 },
        { "Archer", 0 },
        { "Cavalry", 0 },
    };
    public Dictionary<string, int> teamTwoUnits = new Dictionary<string, int>(){
        { "Pikeman", 0 },
        { "Archer", 0 },
        { "Cavalry", 0 },
    };

    public GameObject pikemanPrefab;
    public GameObject archerPrefab;
    public GameObject cavalryPrefab;

    public float maxMoney;
    public float teamOneMoney;
    public float teamTwoMoney;
    public float pikemanCost;
    public float archerCost;
    public float cavalryCost;
    public float steepnessLimit;

    public LayerMask grassMask;
    public LayerMask roadMask;

    GridManager gridManager;
    LayerMask spawnableMask;
    Vector3[] pikemanSpawn, archerSpawn, cavalrySpawn;
    Vector3 cameraPosition;

    private void Awake()
    {
        spawnableMask = grassMask | roadMask;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        teamOneMoney = maxMoney;
        teamTwoMoney = maxMoney;
    }

    ///<summary> Increments/decrements the number of units and updates the team money </summary>
    public void ChangeUnit(int value, int team, string unitType)
    {
        float unitCost = GetUnitCost(unitType) * Mathf.Abs(value);

        if (team == 1)
        {
            if (teamOneMoney - unitCost < 0 && value > 0) //Stops the function if the transaction would go into negatives
                return;
            else if (teamOneUnits[unitType] + value >= 0)
            {
                teamOneUnits[unitType] += value;
                if (value > 0)
                {
                    teamOneMoney -= unitCost;
                }
                else
                    teamOneMoney += unitCost;
            }
        }
        else
        {
            if (teamTwoMoney - unitCost < 0 && value > 0) //Stops the function if the transaction would go into negatives
                return;
            else if (teamTwoUnits[unitType] + value >= 0)
            {
                teamTwoUnits[unitType] += value;
                if (value > 0)
                {
                    teamTwoMoney -= unitCost;
                }
                else
                    teamTwoMoney += unitCost;
            }
        }
    }

    ///<summary> Returns the array of positions for the formation of a unit type </summary>
    Vector3[] FindUnitSpawnPositions(string unitName, Vector3 position, Dictionary<string, int> unitDict)
    {
        Vector3[] spawnPositions = new Vector3[unitDict[unitName]];
        int unitCount = unitDict[unitName];
        
        for (int i = 0; i < unitCount; i++)
        {
            Vector3 checkPosition = SelectedUnit.GetFormationPosition(position, unitCount, i);
            checkPosition = new Vector3(checkPosition.x, 200f, checkPosition.z);

            if (Physics.Raycast(checkPosition, Vector3.down, out RaycastHit hit, 500f, spawnableMask))
            {
                //If the point is walkable and isn't too steep it is suitable for spawning
                if (gridManager.GetNodeFromPosition(hit.point).walkable && gridManager.GetNodeFromPosition(hit.point).steepness < steepnessLimit)
                {
                    spawnPositions[i] = hit.point;
                }
                else
                {
                    Debug.Log(unitName + " is null");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        return spawnPositions;
    }

    ///<summary> Sets the positions of each of the units in a team, returning whether it was successful or not </summary>
    bool FindTeamSpawnPositions(int team)
    {
        bool spawnFound = false;
        Vector3 raycastPosition;
        Vector3 hitposition;
        int attemptCounter = 0;

        Dictionary<string, int> unitDict = new Dictionary<string, int>();
        if (team == 1)
            unitDict = teamOneUnits;
        else
            unitDict = teamTwoUnits;

        while (!spawnFound)
        {
            //Debug.Log("Team: " + team + ", " + "Attempts: " + attemptCounter);
            attemptCounter++;
            if (attemptCounter > 100)
            {
                Debug.Log("Over 100 attempts :(");
                return false;
            }

            float bounds = (GenerationValues.GetMapSize() * GenerationValues.GetChunkSize())/2; //Edges of the map

            //Randomises the raycast position depending on the team
            if (team == 1)
                raycastPosition = new Vector3(Random.Range(-(bounds - 20f), bounds - 80f), 200f, Random.Range(30f, bounds - 20f));
            else
                raycastPosition = new Vector3(Random.Range(-(bounds - 20f), bounds - 80f), 200f, Random.Range(-30f, -(bounds - 20f)));

            RaycastHit hit;
            if (Physics.Raycast(raycastPosition, Vector3.down, out hit, 500f))
            {

                if (gridManager.GetNodeFromPosition(hit.point).walkable && gridManager.GetNodeFromPosition(hit.point).steepness < steepnessLimit)
                {
                    hitposition = hit.point;
                    pikemanSpawn = FindUnitSpawnPositions("Pikeman", hitposition, unitDict); //Finds spawn formation for pikemen
                    if (pikemanSpawn != null || unitDict["Pikeman"] == 0)
                    {
                        hitposition += Vector3.right * 30f;
                        archerSpawn = FindUnitSpawnPositions("Archer", hitposition, unitDict); //Finds spawn formation for archers
                        if (archerSpawn != null || unitDict["Archer"] == 0)
                        {
                            hitposition += Vector3.right * 30f;
                            cavalrySpawn = FindUnitSpawnPositions("Cavalry", hitposition, unitDict); //Finds spawn formation for cavalry
                            if (cavalrySpawn != null || unitDict["Cavalry"] == 0)
                            {
                                spawnFound = true;
                                Debug.Log("Team: " + team + " spawn found");
                                if (team == 1)
                                    cameraPosition = raycastPosition;
                            }
                        }
                    }
                }
            }
        }

        return true;
    }

    ///<summary> Spawns a unit at the given position and sets up its team variable </summary>
    void InstantiateUnit(GameObject unitPrefab, int team, Vector3 position)
    {
        position = new Vector3(position.x, position.y + 5, position.z);

        GameObject temp;
        temp = Instantiate(unitPrefab, position, Quaternion.identity);
        temp.GetComponent<Unit>().team = team;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //If the loaded scene is the battle scene units are spawned
        if (scene.name == "Battle")
        {
            gridManager = GameObject.Find("AStar").GetComponent<GridManager>();

            for (int _team = 1; _team <= 2; _team++)
            {
                Dictionary<string, int> unitDict = new Dictionary<string, int>();
                if (_team == 1)
                    unitDict = teamOneUnits;
                else
                    unitDict = teamTwoUnits;

                //Spawns each of the unit classes according to the random formations given to them
                if (FindTeamSpawnPositions(_team))
                {
                    for (int i = 0; i < unitDict["Pikeman"]; i++)
                    {
                        InstantiateUnit(pikemanPrefab, _team, pikemanSpawn[i]);
                    }
                    for (int i = 0; i < unitDict["Archer"]; i++)
                    {
                        InstantiateUnit(archerPrefab, _team, archerSpawn[i]);
                    }
                    for (int i = 0; i < unitDict["Cavalry"]; i++)
                    {
                        InstantiateUnit(cavalryPrefab, _team, cavalrySpawn[i]);
                    }
                }
            }

            Transform cameraController = GameObject.Find("Camera Rig").transform;
            cameraController.position = new Vector3(cameraPosition.x, cameraController.position.y, cameraPosition.z) + Vector3.right*40f;
        }
    }

    ///<summary> Gets the cost of units from the string name </summary>
    float GetUnitCost(string unitName)
    {
        float value;

        switch (unitName)
        {
            case "Pikeman":
                value = pikemanCost;
                break;
            case "Archer":
                value = archerCost;
                break;
            case "Cavalry":
                value = cavalryCost;
                break;
            default:
                value = 0;
                break;
        }

        return value;
    }
}
