using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] LevelDisplay levelDisplay;

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
                Time.timeScale = 0;
                gamePaused = true;
            } else {
                levelDisplay.HidePause();
                Time.timeScale = 1;
                gamePaused = false; 
            }
        }
    }
}
