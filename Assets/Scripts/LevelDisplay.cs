using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField] private Text livesText, timeText, pointsText, levelText, finalScoreText;
    [SerializeField] private Level[] availableLevels; 
    [SerializeField] private GameObject gameOverPanel, introPanel, pausePanel, gameOverSign, youWonSign;
    [SerializeField] private PlayerMovement player; 
    [SerializeField] private EnemyCounter enemyCounter; 
    [SerializeField] private TileManager tileManager; 
    [SerializeField] SoundController soundController;
    [SerializeField] PauseController pauseController;
    [SerializeField] ItemController itemController;
    
    public Level currentLevel;
    public bool levelStarted;
    public bool gameOver = false;
    public int livesCount = 3;
    private bool timeUp = false;
    private int pointsCount = 0;    
    public int currentLevelKey = 0;
    public float timeCount;
    private float levelChangeTime = 1.5f;
    private bool onGameOverScreen;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if(introPanel.activeSelf) {
            Time.timeScale = 0;
        }
    }

    void Start()
    {
        onGameOverScreen = false;
        levelStarted = false;
        SetLevel();
        UpdateText();
    }

    void Update()
    {
        if(levelStarted && !gameOver && !pauseController.gamePaused) {
            levelText.text = "";

            timeCount -= 1 * Time.deltaTime;

            if(timeCount <= 0) {
                timeCount = 0;
                timeUp = true;
                Time.timeScale = 0;
                levelText.text = "Time's up!";
                player.isDead = true;
                StartCoroutine(NextLevelCoroutine());
            }
        } else if (gameOver) {
            ShowGameOverScreen();
        }

        if(onGameOverScreen) {
            if(Input.anyKey) {
                soundController.StopMusic();
                ResetGame();
            }
        }

        if(introPanel.activeSelf) {
            if(Input.anyKey) {
                soundController.StopMusic();
                soundController.SetCurrentMusicClip();
                soundController.PlayMusic();
                introPanel.SetActive(false);
                Time.timeScale = 1;
            }
        }

        UpdateText();
    }

    public void PlayerDied()
    {
        livesCount -= 1;
        
        if(livesCount > 0){
            livesText.text = livesCount.ToString("0");
        } else {
            gameOver = true;
        }
    }

    public void AddPoints(int points)
    {
        pointsCount += points;
        pointsText.text = pointsCount.ToString("0");
    }

    private void SetLevel()
    {
        currentLevel = availableLevels[currentLevelKey];
        currentLevel.SetEnemies();
        int numberToDisplay = currentLevelKey + 1;
        levelText.text = "Level " + numberToDisplay.ToString("0");
        timeCount = currentLevel.levelTime;
    }

    public void CheckEnemies()
    {
        if(enemyCounter.currentEnemies.Count == 0 && !enemyCounter.stillSpawing){
            soundController.StopMusic();
            levelStarted = false;
            currentLevelKey += 1;
            soundController.SetCurrentMusicClip();
            Time.timeScale = 0;
            StartCoroutine(NextLevelCoroutine());
        }
    }

    private void StartNextLevel()
    {
        if(currentLevelKey < availableLevels.Length) {
            Time.timeScale = 1;
            SceneManager.LoadScene(currentLevelKey);
            Start();
            enemyCounter.Start();
            player.PlayerSpawn();
            soundController.PlayMusic();
        } else if (currentLevelKey >= availableLevels.Length) {
            ShowGameOverScreen();
        }
    }

    private void ShowGameOverScreen()
    {
        finalScoreText.text = "Final Score: " + pointsCount.ToString("0");
        gameOverPanel.SetActive(true);
        
        if(gameOver){
            gameOverSign.SetActive(true);
            youWonSign.SetActive(false);
        } else {
            gameOverSign.SetActive(false);
            youWonSign.SetActive(true);
        }

        onGameOverScreen = true;
    }

    private void ResetGame()
    {
        levelStarted = false;
        gameOver = false;
        livesCount = 3;
        pointsCount = 0;    
        currentLevelKey = 0;
        
        gameOverPanel.SetActive(false);
        Destroy(player.spawnPlatformObject);
        Destroy(player.gameObject);
        Destroy(enemyCounter.gameObject);
        Destroy(tileManager.gameObject);
        Destroy(itemController.gameObject);
        Destroy(pauseController.gameObject);
        Destroy(soundController.gameObject);

        foreach(SpawnPoint spawnPt in enemyCounter.spawnPoints) {
            Destroy(spawnPt.gameObject);
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(currentLevelKey);
        Destroy(gameObject);
    }

    public void DisplayPause()
    {
        pausePanel.SetActive(true);
        levelText.text = "PAUSED";
    }

    public void HidePause()
    {
        pausePanel.SetActive(false);
        levelText.text = "";
    }

    private IEnumerator NextLevelCoroutine()
    {
        float pauseEndTime = Time.realtimeSinceStartup + levelChangeTime;

        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }

        if(!timeUp) {
            StartNextLevel();
        } else {
            gameOver = true;
        }
    }

    public void UpdateText()
    {
        livesText.text = livesCount.ToString("0");
        timeText.text = TimeSpan.FromSeconds(timeCount).ToString("ss\\'ff");
        pointsText.text = pointsCount.ToString("0");
    }

}
