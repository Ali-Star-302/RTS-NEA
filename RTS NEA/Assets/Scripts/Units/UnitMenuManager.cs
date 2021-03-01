using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UnitMenuManager : MonoBehaviour
{
    public TMP_Text pikeman1Text;
    public TMP_Text archer1Text;
    public TMP_Text cavalry1Text;
    public TMP_Text pikeman2Text;
    public TMP_Text archer2Text;
    public TMP_Text cavalry2Text;

    public TMP_Text teamOneMoney;
    public TMP_Text teamTwoMoney;

    int temp;
    UnitManager unitManager;

    void Awake()
    {
        unitManager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
    }

    void Update()
    {
        //Team 1
        unitManager.teamOneUnits.TryGetValue("Pikeman", out temp);
        pikeman1Text.text = "Pikemen: " + temp.ToString();
        unitManager.teamOneUnits.TryGetValue("Archer", out temp);
        archer1Text.text = "Archers: " + temp.ToString();
        unitManager.teamOneUnits.TryGetValue("Cavalry", out temp);
        cavalry1Text.text = "Cavalry: " + temp.ToString();
        teamOneMoney.text = "Money: " + unitManager.teamOneMoney.ToString();

        //Team 2
        unitManager.teamTwoUnits.TryGetValue("Pikeman", out temp);
        pikeman2Text.text = "Pikemen: " + temp.ToString();
        unitManager.teamTwoUnits.TryGetValue("Archer", out temp);
        archer2Text.text = "Archers: " + temp.ToString();
        unitManager.teamTwoUnits.TryGetValue("Cavalry", out temp);
        cavalry2Text.text = "Cavalry: " + temp.ToString();
        teamTwoMoney.text = "Money: " + unitManager.teamTwoMoney.ToString();


    }

    public void ConfirmButton()
    {
        SceneManager.LoadScene("Procedural Generation");
    }
}
