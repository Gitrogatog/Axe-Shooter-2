using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;
    public Round[] rounds;
    int currentRound = 0;

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    public void OnBeatRound(){
        currentRound++;
        if(currentRound >= rounds.Length){
            currentRound = rounds.Length - 1;
        }
    }

    public void ReturnToTitle(){
        SceneManager.LoadScene(0);
    }

    public Round GetCurrentRound(){
        return rounds[currentRound];
    }
    public class Round{
        public int enemyCount;
        public float enemyMoveSpeed;
        public float damage;
        public float enemyHealth;
    }
}
