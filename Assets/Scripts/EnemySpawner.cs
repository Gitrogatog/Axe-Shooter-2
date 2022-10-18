using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public Wave[] waves;
    public EnemyAIScript[] enemies;

    Axe axe;
    LevelGenerator levelGenerator;
    private int enemiesRemainingAlive = 1;
    private float nextSpawnTime;
    private Wave currentWave;
    private int currentWaveNum = 0;
    Vector3 campPosOld;
    bool isCamping;

    bool isDisabled = false;
    public bool devMode = false;

    public event System.Action OnWaveEnd;
    //public event System.Action OnNewWave;
    public Vector2 playerStartPos;

    void Start(){
        //enemiesRemainingAlive = waves[currentWaveNum].enemyCount;
        levelGenerator = FindObjectOfType<LevelGenerator>();
        levelGenerator.CreateMap(currentWaveNum);
        SpawnEnemies();
        axe = FindObjectOfType<Axe>();
    }

    void OnEnemyDeath(){
        Debug.Log("Enemy Died");
        enemiesRemainingAlive--;
        if(enemiesRemainingAlive <= 0){
            OnWaveEnd();
            axe.ForceRecall();
        }
    }
    
    void SpawnEnemies(){
        int enemiesRemaining = waves[currentWaveNum].enemyCount;
        enemiesRemainingAlive = 0;
        Debug.Log("Enemies: " + enemiesRemaining);
        while(levelGenerator.FloorSpaceAvailable() && enemiesRemaining > 0){
            Vector2 randCoord = levelGenerator.GetRandomFloorCoord();
            if(randCoord.x != playerStartPos.x || randCoord.y != playerStartPos.y){
                EnemyAIScript enemyAI = Instantiate(enemies[Random.Range(0, enemies.Length)], randCoord, Quaternion.identity);
                enemyAI.OnDeath += OnEnemyDeath;
                enemiesRemaining--;
                enemiesRemainingAlive++;
                enemyAI.SetCharacteristics(waves[currentWaveNum].enemyMoveSpeed, waves[currentWaveNum].enemyHealth, waves[currentWaveNum].damage);
            }
            
        }
    }
    
    /*
    void SpawnEnemies(){
        int enemiesRemaining = GameManagerScript.instance.GetCurrentRound().enemyCount;
        while(levelGenerator.FloorSpaceAvailable() && enemiesRemaining > 0){
            Vector2 randCoord = levelGenerator.GetRandomFloorCoord();
            if(randCoord.x != playerStartPos.x || randCoord.y != playerStartPos.y){
                EnemyAIScript enemyAI = Instantiate(enemies[Random.Range(0, enemies.Length)], randCoord, Quaternion.identity);
                enemyAI.OnDeath += OnEnemyDeath;
                enemiesRemaining--;
                enemyAI.SetCharacteristics(GameManagerScript.instance.GetCurrentRound().enemyMoveSpeed, GameManagerScript.instance.GetCurrentRound().enemyHealth, GameManagerScript.instance.GetCurrentRound().damage);
            }
            
        }
    } */

    public void NextWave(){
        //OnNewWave();
        
        currentWaveNum++;
        if(currentWaveNum >= waves.Length){
            SceneManager.LoadScene("End Scene");
        }
        else{
            levelGenerator.CreateMap(currentWaveNum);
            SpawnEnemies();
        }
        
    }

    public int GetWaveNum(){
        return currentWaveNum;
    }

    [System.Serializable]
    public class Wave{
        public int enemyCount;
        public float enemyMoveSpeed;
        public float damage;
        public float enemyHealth;
        public Color enemyColor;
    }
}
