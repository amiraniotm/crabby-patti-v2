using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] LevelDisplay levelDisplay;
    [SerializeField] SoundController soundController;

    public bool gamePaused = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);    
    }

    void Update()
    {
        if(Input.GetKeyDown("return")) {
            if(levelDisplay.levelStarted && !levelDisplay.gameOver && !gamePaused) {
                levelDisplay.DisplayPause();
                soundController.PauseMusic();
                Time.timeScale = 0;
                gamePaused = true;
            } else {
                levelDisplay.HidePause();
                soundController.UnPauseMusic();
                Time.timeScale = 1;
                gamePaused = false; 
            }
        }
    }
}
