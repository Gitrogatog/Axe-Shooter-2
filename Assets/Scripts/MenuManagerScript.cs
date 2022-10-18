using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManagerScript : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullscreenToggle;
    public int[] screenWidths;
    private int activeScreenResIndex;

    void Start(){
        activeScreenResIndex = PlayerPrefs.GetInt("Screen Res Index", 0);
        bool isFullscreen = (PlayerPrefs.GetInt("Fullscreen") == 1)?true:false;
        volumeSliders[0].value = AudioManagerScript.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManagerScript.instance.sfxVolumePercent;
        volumeSliders[2].value = AudioManagerScript.instance.musicVolumePercent;

        for(int i = 0; i < resolutionToggles.Length; i++){
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }

        fullscreenToggle.isOn = isFullscreen;
    }

    public void Play(){
        SceneManager.LoadScene("Game Scene");
        Debug.Log("Game Loaded");
    }

    public void Quit(){
        Application.Quit();
    }

    public void OptionsMenu(){
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void MainMenu(){
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void SetScreenResolution(int i){
        if(resolutionToggles[i].isOn){
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("Screen Res Index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullscreen(bool isFullscreen){
        for(int i = 0; i < resolutionToggles.Length; i++){
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if(isFullscreen){
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else{
            SetScreenResolution(activeScreenResIndex);
        }

        PlayerPrefs.SetInt("Fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value){
        AudioManagerScript.instance.SetVolume(value, AudioManagerScript.AudioChannel.Master);
    }

    public void SetSfxVolume(float value){
        AudioManagerScript.instance.SetVolume(value, AudioManagerScript.AudioChannel.Sfx);
    }

    public void SetMusicVolume(float value){
        AudioManagerScript.instance.SetVolume(value, AudioManagerScript.AudioChannel.Music);
    }
}
