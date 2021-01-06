using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    public AudioMixer audioMixer;

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
        audioMixer.SetFloat("volumeMaster", volume);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("volumeMusic", volume);
    }
    public void SetGameplayVolume(float volume)
    {
        audioMixer.SetFloat("volumeGameplay", volume);
    }

    public void FullscreenToggle()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetResolution()
    {

    }
}
