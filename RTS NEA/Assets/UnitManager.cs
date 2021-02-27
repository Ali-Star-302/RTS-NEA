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


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ChangeUnit(int value, int team, string unitType)
    {
        if (team == 1)
        {
            if (teamOneUnits[unitType] + value >= 0)
                teamOneUnits[unitType] += value;
        }
        else
        {
            if (teamTwoUnits[unitType] + value >= 0)
                teamTwoUnits[unitType] += value;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Procedural Generation")
        {
            //Team 1
            for (int i = 0; i < teamOneUnits["Pikeman"]; i++)
            {
                InstantiateUnit(pikemanPrefab, 1);
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

    void InstantiateUnit(GameObject unitPrefab, int team)
    {
        GameObject temp;
        temp = Instantiate(unitPrefab, new Vector3(0,100,0), Quaternion.identity);
        temp.GetComponent<Unit>().team = team;
    }
}
