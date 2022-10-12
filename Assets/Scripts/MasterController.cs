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
    [SerializeField] public List<Sprite> bgList = new List<Sprite>();
    
    public PauseController pauseController;
    private PlayerMovement player;
    public Level currentLevel;
    public LevelDisplay levelDisplay;
    public EnemyCounter enemyCounter;
    public ItemController itemController;
    public GameObject entryPoint, backgroundObject, playerObject, MDCObject;
    public TileManager tileManager;
    public SpriteRenderer backgroundRenderer;
    private MapDisplacementController mapDisController;

    public bool changingLevel = false;
    public bool levelStarted;
    public bool gameOver = false;
    public bool timeUp = false;
    public bool scrollPhase = false;
    public int currentLevelKey;
    public int currentPhaseKey;
    public int livesCount = 1;
    public int pointsCount = 0;    
    private int levelTransitionTime = 1;
    private float phaseChangeDuration = 1.5f;    
    private float phaseChangeTimer;
    public float timeCount;
    public bool startingDisplacement;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentLevelKey = 0;
        currentPhaseKey = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown("escape") && SceneManager.GetActiveScene().name == "Title") {
            if(instructionsPanel.activeSelf) {
                HideInstructionsPanel();
            } else if(optionsPanel.activeSelf) {
                HideOptionsPanel();
            }
        }

        if(levelStarted && !gameOver && pauseController != null && !pauseController.gamePaused && !scrollPhase) {
            timeCount -= Time.deltaTime;

            if(timeCount <= 0) {
                timeCount = 0;
                timeUp = true;
                Time.timeScale = 0;
                player.isDead = true;
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
            currentPhaseKey = 1;
            soundController.StopMusic();
            soundController.SetCurrentMusicClip();
            soundController.PlayMusic();
            currentLevel = availableLevels[currentLevelKey - 1];
            timeCount = currentLevel.levelTime;
            currentLevel.SetEnemies();
            mainCamera.GetBackgroundImage();
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
            if(currentPhaseKey < currentLevel.levelPhases) {
                phaseChangeTimer = phaseChangeDuration;
                itemController.StopItems();
                levelDisplay.timePanel.SetActive(false);
                startingDisplacement = true;
                StartCoroutine(StartDisplacementCoroutine());
            } else {
                soundController.StopMusic();
                levelStarted = false;
                Time.timeScale = 0;
                changingLevel = true;
                
                StartCoroutine(NextLevelCoroutine());
            }
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
        Destroy(soundController.gameObject);
        Destroy(mainCamera.gameObject);

        Time.timeScale = 1;
        SceneManager.LoadScene(currentLevelKey);
        Destroy(gameObject);
    }

    public void EndScrollPhase()
    {
        scrollPhase = false;
        itemController.StartItems(5.0f);
        levelDisplay.timePanel.SetActive(true);
        //EXTRA TIME PER PHASE
        timeCount += 30;
        mapDisController.EndDisplacement();
        enemyCounter.Start();

        Time.timeScale = 0;
        phaseChangeTimer = phaseChangeDuration;
        levelStarted = false;
        StartCoroutine(NextPhaseCoroutine());
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
        while(entryPoint == null || backgroundObject == null || playerObject == null || MDCObject == null) {
            entryPoint = GameObject.FindGameObjectWithTag("EntryPoint");
            backgroundObject = GameObject.FindGameObjectWithTag("Background");
            playerObject = GameObject.FindGameObjectWithTag("Player");
            MDCObject = GameObject.FindGameObjectWithTag("DisplacementController");

            yield return 0;
        }

        backgroundRenderer = backgroundObject.GetComponent<SpriteRenderer>();
        player = playerObject.GetComponent<PlayerMovement>();
        mapDisController = MDCObject.GetComponent<MapDisplacementController>();
        mapDisController.SetDisplacementObjects(this);
    }

    private IEnumerator NextPhaseCoroutine()
    {
        while (phaseChangeTimer > 0)
        {
            phaseChangeTimer -= Time.unscaledDeltaTime;
            
            yield return 0;
        }

        Time.timeScale = 1;
        levelStarted = true;
        phaseChangeTimer = 0.0f;
    }

    private IEnumerator StartDisplacementCoroutine()
    {
        while (phaseChangeTimer > 0)
        {
            phaseChangeTimer -= Time.unscaledDeltaTime;
            
            yield return 0;
        }

        scrollPhase = true;
        mapDisController.StartDisplacement();
        currentPhaseKey += 1;
        phaseChangeTimer = 0.0f;
        startingDisplacement = false;
    }
}
