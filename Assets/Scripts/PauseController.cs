using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] LevelDisplay levelDisplay;

    private MasterController masterController;
    private SoundController soundController;
    public bool gamePaused = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);    
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        masterController.SetPauseController(this);
    }

    void Update()
    {
        if(Input.GetKeyDown("return")) {
            if(masterController.levelStarted && !masterController.gameOver && !gamePaused) {
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
