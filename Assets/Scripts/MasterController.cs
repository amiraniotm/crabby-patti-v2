using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    [SerializeField] public SoundController soundController;
    [SerializeField] private GameObject instructionsPanel, optionsPanel, titleSign, mainMenu;
    [SerializeField] private Level[] availableLevels; 
    [SerializeField] private AudioClip convertTimeSound;
    [SerializeField] private CameraMovement mainCamera;
    [SerializeField] private FloatingPlatformController floatPlatController;
    [SerializeField] public List<Sprite> bgList = new List<Sprite>();
    
    public PauseController pauseController;
    private PlayerMovement player;
    public Level currentLevel;
    public LevelDisplay levelDisplay;
    public EnemyCounter enemyCounter;
    public ItemController itemController;
    private GameObject entryPoint;
    public GameObject walls;
    public TileManager tileManager;
    public GameObject backgroundObject;
    public SpriteRenderer backgroundRenderer;

    public bool changingLevel = false;
    public bool levelStarted;
    public bool gameOver = false;
    public bool timeUp = false;
    public bool scrollPhase = false;
    public int currentLevelKey;
    public int livesCount = 1;
    public int pointsCount = 0;    
    private int levelTransitionTime = 1;
    public float timeCount;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentLevelKey = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown("escape")) {
            if(instructionsPanel.activeSelf) {
                HideInstructionsPanel();
            } else if(optionsPanel.activeSelf) {
                HideOptionsPanel();
            }
        }

        if(levelStarted && !gameOver && pauseController != null && !pauseController.gamePaused) {
            timeCount -= 1 * Time.deltaTime;

            if(timeCount <= 0) {
                timeCount = 0;
                timeUp = true;
                Time.timeScale = 0;
                player.isDead = true;
            }

            if(mainCamera.currentPanTime > 0f) {
                MoveWalls();
            } 

        } else if (gameOver) {
            levelDisplay.ShowGameOverScreen();
        }
    }

    public void ShowInstructionsPanel()
    {
        titleSign.SetActive(false);
        mainMenu.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void ShowOptionsPanel()
    {
        titleSign.SetActive(false);
        mainMenu.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void HideInstructionsPanel()
    {
        titleSign.SetActive(true);
        mainMenu.SetActive(true);
        instructionsPanel.SetActive(false);
    }

    public void HideOptionsPanel()
    {
        titleSign.SetActive(true);
        mainMenu.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void SetLevelDisplay(LevelDisplay LDRef)
    {     
        if(levelDisplay == null) {
            levelDisplay = LDRef;
        }
    }

    public void SetEnemyCounter(EnemyCounter ECRef)
    {             
        if(enemyCounter == null) {
            enemyCounter = ECRef;
        }
    }

    public void SetPlayer(PlayerMovement playerRef)
    {
        if(player == null) {
            player = playerRef;
        }
    }

    public void SetPauseController(PauseController PCRef)
    {
        if(pauseController == null) {
            pauseController = PCRef;
        }
    }

    public void SetItemController(ItemController ICRef)
    {     
        if(itemController == null) {
            itemController = ICRef;
        }
    }

    public void SetTileManager(TileManager TMRef)
    {     
        if(tileManager == null) {
            tileManager = TMRef;
        }
    }

    public void StartLevel()
    {
        if(currentLevelKey < availableLevels.Length) { 
            levelStarted = false;
            Time.timeScale = 1;
            currentLevelKey += 1;
            soundController.StopMusic();
            soundController.SetCurrentMusicClip();
            soundController.PlayMusic();
            currentLevel = availableLevels[currentLevelKey - 1];
            timeCount = currentLevel.levelTime;
            currentLevel.SetEnemies();
            mainCamera.GetBackgroundImage();
            floatPlatController.GetSpawnPoint();
            entryPoint = null;
            if(currentLevelKey > 1) {    
                backgroundRenderer.sprite = bgList[currentLevelKey - 1];
                tileManager.SetLevelTiles(currentLevelKey - 1);
                player.PlayerSpawn();
                enemyCounter.Start();
                itemController.FlushItems();
            } else {
                SceneManager.LoadScene(currentLevelKey);
            }
            StartCoroutine(SetLevelObjectsCoroutine());
        } else if (currentLevelKey >= availableLevels.Length) {
            levelDisplay.ShowGameOverScreen();
        }
    }

    public void AddPoints(int points)
    {
        pointsCount += points;
    }

    public void CheckEnemies()
    {
        if(enemyCounter.currentEnemies.Count == 0 && !enemyCounter.stillSpawing){
            soundController.StopMusic();
            /**
            //FROM HERE, MAP DISPLACEMENT STUFF
            scrollPhase = true;
            mainCamera.TriggerPan();
            entryPoint.SetActive(false);
            walls.SetActive(true);

            GameObject[] powBlocks = GameObject.FindGameObjectsWithTag ("PowBlock");
 
            foreach(GameObject PB in powBlocks)
            {
                PB.SetActive(false);
            }

            foreach(SpawnPoint spawnPt in enemyCounter.spawnPoints) {
                spawnPt.gameObject.SetActive(false);
            }
            floatPlatController.TriggerPlatforms();
            //MAP DISPLACEMENT END
            **/
            levelStarted = false;
            Time.timeScale = 0;
            changingLevel = true;
            
            StartCoroutine(NextLevelCoroutine());
        }
    }

    public void PlayerDied()
    {
        livesCount -= 1;
        
        if(livesCount == 0){
            gameOver = true;
            soundController.StopMusic();
        }
    }

    public void ResetGame()
    {
        currentLevelKey = 0;
        
        levelDisplay.gameOverPanel.SetActive(false);
        Destroy(player.spawnPlatformObject);
        Destroy(player.gameObject);
        Destroy(enemyCounter.gameObject);
        Destroy(pauseController.gameObject);
        Destroy(soundController.gameObject);
        Destroy(levelDisplay.gameObject);
        Destroy(tileManager.gameObject);
        Destroy(itemController.gameObject);
        Destroy(mainCamera.gameObject);

        foreach(SpawnPoint spawnPt in enemyCounter.spawnPoints) {
            Destroy(spawnPt.gameObject);
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(currentLevelKey);
        Destroy(gameObject);
    }

    public void MoveWalls()
    {
        Vector3 newWallPos = new Vector3(walls.transform.position.x,
                                        mainCamera.gameObject.transform.position.y,
                                        walls.transform.position.z);

        walls.transform.position = newWallPos;
    }

    public void MoveStage()
    {

    }

    private IEnumerator NextLevelCoroutine()
    {
        while (timeCount > 1)
        {
            float secondsCountFloat = 100 * Time.unscaledDeltaTime;
            int secondsCountInt = (int) secondsCountFloat;
            timeCount -= secondsCountInt;
            pointsCount += secondsCountInt;
            soundController.PlaySound(convertTimeSound, 0.05f);
            
            if(timeCount < 1) {
                timeCount = 0;
            }

            yield return 0;
        }

        float transitionEndTime = Time.realtimeSinceStartup + levelTransitionTime;

        while(Time.realtimeSinceStartup < transitionEndTime) {
            yield return 0;
        }

        if(!timeUp) {
            changingLevel = false;
            StartLevel();
        } else {
            gameOver = true;
        }
    }

    private IEnumerator SetLevelObjectsCoroutine()
    {
        while(entryPoint == null || walls == null) {
            entryPoint = GameObject.FindGameObjectWithTag("EntryPoint");
            walls = GameObject.FindGameObjectWithTag("Walls");
            backgroundObject = GameObject.FindGameObjectWithTag("Background");
            yield return 0;
        }

        walls.SetActive(false);
        backgroundRenderer = backgroundObject.GetComponent<SpriteRenderer>();
    }

}
