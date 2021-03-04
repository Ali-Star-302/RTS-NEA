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

    public LayerMask grassMask;
    public LayerMask roadMask;

    LayerMask spawnableMask;
    Vector3 pikeman1Spawn, archer1Spawn, cavalry1Spawn, pikeman2Spawn, archer2Spawn, cavalry2Spawn;

    private void Awake()
    {
        spawnableMask = grassMask | roadMask;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        teamOneMoney = maxMoney;
        teamTwoMoney = maxMoney;
    }

    public void ChangeUnit(int value, int team, string unitType)
    {
        float unitCost = GetUnitCost(unitType);

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

    Vector3 FindUnitSpawnPosition(int team, string unitName)
    {
        bool spawnFound = false;
        Vector3 spawnPosition = Vector3.zero;
        Dictionary<string, int> unitDict = new Dictionary<string, int>();
        if (team == 1)
            unitDict = teamOneUnits;
        else
            unitDict = teamTwoUnits;

        //Continuously loops until a suitable spawn is found
        while (!spawnFound)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, 500f, spawnableMask))
            {

            }

            for (int i = 0; i < unitDict[unitName]; i++)
            {

            }
        }

        return spawnPosition;
    }

    void FindTeamSpawnPositions(int team)
    {
        bool spawnFound = false;
        while (!spawnFound)
        {

        }
    }

    void InstantiateUnit(GameObject unitPrefab, int team, Vector3 position)
    {
        //position = new Vector3(position.x, 150, position.z);

        GameObject temp;
        temp = Instantiate(unitPrefab, position, Quaternion.identity);
        temp.GetComponent<Unit>().team = team;
    }
    void InstantiateUnit(GameObject unitPrefab, int team)
    {
        GameObject temp;
        temp = Instantiate(unitPrefab, new Vector3(0, 100, 0), Quaternion.identity);
        temp.GetComponent<Unit>().team = team;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Procedural Generation")
        {
            Vector3[] spawnPositions;
            //FindSpawnPosition(1, "Pikeman");
            FindTeamSpawnPositions(1);

            //Team 1
            for (int i = 0; i < teamOneUnits["Pikeman"]; i++)
            {
                InstantiateUnit(pikemanPrefab, 1, SelectedUnit.GetFormationPosition(Vector3.zero, teamOneUnits["Pikeman"], i));
            }
            for (int i = 0; i < teamOneUnits["Archer"]; i++)
            {
                InstantiateUnit(archerPrefab, 1);
            }
            for (int i = 0; i < teamOneUnits["Cavalry"]; i++)
            {
                InstantiateUnit(cavalryPrefab, 1);
            }

            //Team 2
            for (int i = 0; i < teamTwoUnits["Pikeman"]; i++)
            {
                InstantiateUnit(pikemanPrefab, 2);
            }
            for (int i = 0; i < teamTwoUnits["Archer"]; i++)
            {
                InstantiateUnit(archerPrefab, 2);
            }
            for (int i = 0; i < teamTwoUnits["Cavalry"]; i++)
            {
                InstantiateUnit(cavalryPrefab, 2);
            }
        }
    }

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
