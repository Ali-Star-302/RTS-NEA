using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitIncrementButton : MonoBehaviour
{
    public int value;
    public int team;
    public string unitType;

    UnitManager unitManager;

    void Awake()
    {
        unitManager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
        GetComponent<Button>().onClick.AddListener(() => unitManager.ChangeUnit(value, team, unitType));
    }
}
