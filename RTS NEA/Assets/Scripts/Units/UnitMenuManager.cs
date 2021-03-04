using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UnitMenuManager : MonoBehaviour
{
    public GameObject unitSelectUI;
    public GameObject mapSelectUI;
    public TerrainEditor terrainEditor;

    public TMP_Text pikeman1Text;
    public TMP_Text archer1Text;
    public TMP_Text cavalry1Text;
    public TMP_Text pikeman2Text;
    public TMP_Text archer2Text;
    public TMP_Text cavalry2Text;
    public TMP_Text teamOneMoney;
    public TMP_Text teamTwoMoney;

    public Slider mapSizeSlider;
    public TMP_InputField seedInput;
    public Button seedButton;
    public Image mapImage;

    int temp;
    UnitManager unitManager;

    void Awake()
    {
        unitManager = GameObject.Find("UnitManager").GetComponent<UnitManager>();

        if (GenerationValues.GetSeed() == 0)
            GenerationUtilities.GenerateRandomSeed();
    }

    void Update()
    {
        UpdateText();
    }

    void UpdateText()
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

    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ToggleUnitSelectUI()
    {
        bool unitSelectIsActive = unitSelectUI.activeSelf;
        unitSelectUI.SetActive(!unitSelectIsActive);
        mapSelectUI.SetActive(unitSelectIsActive);
    }

    public void ChangeMapSize(float size)
    {
        GenerationValues.SetMapSize((int)size);
    }

    public void ApplySeed()
    {
        int parsedSeed = 0;
        if (int.TryParse(seedInput.text, out parsedSeed))
            GenerationValues.SetSeed(parsedSeed);

        UpdateMapPreview();
    }

    public void RandomSeed()
    {
        GenerationUtilities.GenerateRandomSeed();
    }

    public void UpdateMapPreview()
    {
        Texture2D tex = terrainEditor.UpdateMapPreview();
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        mapImage.sprite = sprite;
    }
}
