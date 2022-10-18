using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIScript : MonoBehaviour
{
    public Image fadePlane;
    public GameObject roundEndUI;
    public GameObject gameOverUI;
    public TextMeshProUGUI roundText;
    int currentRound = 1;

    public RectTransform healthBar;
    public RectTransform ammoBar;

    EnemySpawner spawner;
    PlayerController player;
    WeaponsManager weaponsManager;
    void Awake()
    {
        //Cursor.visible = false;
        player = FindObjectOfType<PlayerController>();
        player.OnDeath += OnGameOver;
        spawner = FindObjectOfType<EnemySpawner>();
        spawner.OnWaveEnd += OnBeatRound;
        roundText.text = "Round 1";
        //spawner.OnNewWave += StartNewRound;
    }

    void Update(){
        //scoreUI.text = ScoreKeeperScript.score.ToString("D6");
        float healthPercent = 0;
        if(player != null){
            healthPercent = player.health / player.maxHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
        float ammoPercent = 0;
        if(player != null){
            ammoPercent = player.GetAmmoFraction();
        }
        ammoBar.localScale = new Vector3(ammoPercent, 1, 1);
    }

    void OnGameOver(){
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.9f), 1));
        //healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    void OnBeatRound(){
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.9f), 1));
        //healthBar.transform.parent.gameObject.SetActive(false);
        roundEndUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time){
        float speed = 1 / time;
        float percent = 0;
        while(percent < 1){
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    //Input
    public void StartNewGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartNewRound(){
        currentRound++;
        roundText.text = "Round " + currentRound;
        spawner.NextWave();
        Cursor.visible = false;
        StartCoroutine(Fade(new Color(0, 0, 0, 0.9f), Color.clear, 1f));
        healthBar.transform.parent.gameObject.SetActive(true);
        roundEndUI.SetActive(false);
        player.OnNewWave();
    }

    public void ReturnToMainMenu(){
        SceneManager.LoadScene("Menu Scene");
    }
}
