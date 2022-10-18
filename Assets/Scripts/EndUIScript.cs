using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndUIScript : MonoBehaviour
{
    void Start(){
        Cursor.visible = true;
    }
    public void PlayAgain(){
        SceneManager.LoadScene("Game Scene");
    }
    public void ReturnToMainMenu(){
        SceneManager.LoadScene("Menu Scene");
    }

    public void Quit(){
        Application.Quit();
    }
}
