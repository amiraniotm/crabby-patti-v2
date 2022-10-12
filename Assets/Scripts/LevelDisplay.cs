using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField] private Text livesText, timeText, pointsText, levelText, finalScoreText;
    [SerializeField] public GameObject gameOverPanel, pausePanel, gameOverSign, youWonSign, timePanel;
    
    private MasterController masterController;
    private bool onGameOverScreen;

    private void Awake()
    {
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetLevelDisplay(this);
    }

    void Start()
    {
        onGameOverScreen = false;
        UpdateText();
    }

    void Update()
    {
        if(onGameOverScreen) {
            if(Input.anyKey) {
                masterController.ResetGame();
                onGameOverScreen = false;
            }
        }

        UpdateText();
        UpdateLevelText();
    }

    public void ShowGameOverScreen()
    {
        finalScoreText.text = "Final Score: " + masterController.pointsCount.ToString("0");
        gameOverPanel.SetActive(true);
        
        if(masterController.gameOver){
            gameOverSign.SetActive(true);
            youWonSign.SetActive(false);
        } else {
            gameOverSign.SetActive(false);
            youWonSign.SetActive(true);
        }

        onGameOverScreen = true;
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

    public void UpdateText()
    {
        livesText.text = masterController.livesCount.ToString("0");
        timeText.text = TimeSpan.FromSeconds(masterController.timeCount).ToString("m\\'ss\\'ff");
        pointsText.text = masterController.pointsCount.ToString("0");
    }

    public void UpdateLevelText()
    {
        if(masterController.timeUp) {
            levelText.text = "Time's up!";
        } else if(!masterController.levelStarted && !masterController.changingLevel) {
            int levelToDisplay = masterController.currentLevelKey;
            int phaseToDisplay = masterController.currentPhaseKey;
            levelText.text = "Level " + levelToDisplay.ToString("0") + " - " + phaseToDisplay.ToString("0");
        } else if(masterController.pauseController.gamePaused) {
            levelText.text = "PAUSE";
        } else if(masterController.startingDisplacement) {
            levelText.color = Color.yellow;
            levelText.text = "CLIMB!!";
        } else {
            levelText.color = Color.white;
            levelText.text = "";

        }
    }
}
