using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Toggle fullscreenToggle;

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string currentRes = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(currentRes);
        }

        resolutionDropdown.AddOptions(resolutionOptions);

        fullscreenToggle.isOn = Screen.fullScreen;

        //Set the value thats in the dropdown to the current resolution
        int resolutionIndex = 0;
        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if (resolutions[i].width + " x " + resolutions[i].height == Screen.currentResolution.width + " x " + Screen.currentResolution.height)
                resolutionIndex = i;
        }
        resolutionDropdown.value = resolutionIndex;

        //Set the value thats in the dropdown to the current quality setting
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
    }


    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("volumeMaster", Mathf.Log10(volume) * 20);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("volumeMusic", Mathf.Log10(volume) * 20);
    }
    public void SetGameplayVolume(float volume)
    {
        audioMixer.SetFloat("volumeGameplay", Mathf.Log10(volume) * 20);
    }

    public void FullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }
}
