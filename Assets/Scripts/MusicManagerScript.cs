using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManagerScript : MonoBehaviour
{
    public SceneMusic[] sceneSongs;

    string sceneName;

    void Start(){
        sceneName = SceneManager.GetActiveScene().name;
        Invoke("PlayMusic", 0.2f);
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }
    void OnLevelWasLoaded(int sceneIndex){
        string newSceneName = SceneManager.GetActiveScene().name;
        if(sceneName != newSceneName){
            sceneName = newSceneName;
            Invoke("PlayMusic", 0.2f);
        }
    }

    void PlayMusic(){
        AudioClip clipToPlay = null;
        foreach(SceneMusic sMusic in sceneSongs){
            if(sMusic.sceneName == sceneName){
                clipToPlay = sMusic.clip;
            }
        }
        if(clipToPlay != null){
            AudioManagerScript.instance.PlayMusic(clipToPlay, 2);
        }
    }

    [System.Serializable]
    public struct SceneMusic{
        public string sceneName;
        public AudioClip clip;
        public float volume;
    }
}
