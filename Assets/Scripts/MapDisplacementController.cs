using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplacementController : MonoBehaviour
{
    [SerializeField] private GameObject[] stageObjects;
    [SerializeField] private GameObject wallObject, spawnPlatObject,platPrefab,entryPoint;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float spawnInterval, obstacleChance;
    [SerializeField] private ObjectPool displacementPool;
    [SerializeField] private PlayerSpawnPlatform spawnPlat;
 
    private MasterController masterController;
    private CameraMovement mainCamera;
    private GameObject latestPlat;
    private List<GameObject> spawnedPlats = new List<GameObject>();
    public List<Obstacle> currentObstacles = new List<Obstacle>();
    private Vector3 originalPlatScale = Vector3.zero;
    private bool initialPlat = true;
    private float platWidth, platHeight;

    private void Awake()
    {
        SpriteRenderer platRenderer = platPrefab.GetComponent<SpriteRenderer>();
        platWidth = platRenderer.bounds.size.x;
        platHeight = platRenderer.bounds.size.y;
    }

    private void Update()
    {
        if(masterController != null && masterController.scrollPhase) {
            MoveWalls();
            MoveSpawnPoint();
        }

        CheckPlatforms();
    }

    public void SetDisplacementObjects(MasterController MCRef)
    {
        masterController = MCRef;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
    }

    public void StartDisplacement()
    {
        mainCamera.TriggerPan();
        entryPoint.transform.position = CalculateAdjPos(entryPoint.transform.position);

        InvokeRepeating("SetPlatform", 0f, spawnInterval);
        InvokeRepeating("CheckForObstacle", 0f, spawnInterval);
    }


    private void SetPlatform()
    {
        if(initialPlat){
            latestPlat = Instantiate(platPrefab, spawnPlatObject.transform.position, Quaternion.identity);
            initialPlat = false;
        } else {
            Vector3 newPlatPos = GetRandomPlatPos();
            latestPlat = displacementPool.GetPooledObject("CrabPlatform");
            latestPlat.transform.position = newPlatPos;
            if(originalPlatScale.x == 0) {
                originalPlatScale = latestPlat.transform.localScale;
            }

            ChangePlatSize(latestPlat);
            latestPlat.SetActive(true);
        }

        if(!spawnedPlats.Contains(latestPlat)){
            spawnedPlats.Add(latestPlat);
        }
    }

    private Vector3 GetRandomPlatPos()
    {
        float randX = Random.Range(latestPlat.transform.position.x - (5 * platWidth), 
                                    latestPlat.transform.position.x + (5 * platWidth));
        
        if(randX < -(mainCamera.screenWidth / 2)) {
            randX = (-mainCamera.screenWidth / 2) + platWidth;
        } else if (randX > (mainCamera.screenWidth / 2) ) {
            randX = (mainCamera.screenWidth / 2) - platWidth;
        }

        float currentCameraY = mainCamera.gameObject.transform.position.y;
        float randY = currentCameraY + (mainCamera.screenHeight / 2) + platHeight;

        Vector3 randPos = new Vector3(randX, randY, latestPlat.transform.position.z);
        
        return randPos;
    }

    private void ChangePlatSize(GameObject plat)
    {
        float lengthMod = Random.Range(3 , 7);
        Vector3 newScale = new Vector3(originalPlatScale.x * lengthMod,
                                        plat.transform.localScale.y,
                                        plat.transform.localScale.z);

        plat.transform.localScale = newScale;
    }

    private void CheckPlatforms()
    {
        foreach(GameObject plat in spawnedPlats) {
            Vector3 cameraCorner = mainCamera.GetCurrentCorner("lowerleft");

            if(plat.transform.position.y < (cameraCorner.y - platHeight)) {
                plat.SetActive(false);
            }
        }
    }

    public void StopPlatformsAndObstacles()
    {
        CancelInvoke();
        foreach(Obstacle obs in currentObstacles ) {
            if(!obs.leaving) {
                obs.forceLeave = true;
                obs.DropAttack();
            }    
        }
        
        initialPlat = true;
    }

    private void MoveWalls()
    {
        if(!wallObject.activeSelf) {
            wallObject.SetActive(true);
        }
        
        Vector3 newWallPos = new Vector3 (wallObject.transform.position.x,
                                    mainCamera.gameObject.transform.position.y,
                                    wallObject.transform.position.z
                                    );

        wallObject.transform.position = newWallPos;
    }

    private void MoveSpawnPoint()
    {
        if(!spawnPlat.respawning && spawnPlat.currentRespawnTime == 0) {
            spawnPlat.currentRespawnTime = spawnPlat.maxRespawnTime;
        }

        float step = mainCamera.panSpeed * Time.deltaTime;

        spawnPlatObject.transform.position = Vector3.MoveTowards(spawnPlatObject.transform.position, CalculateAdjPos(spawnPlatObject.transform.position), step);
        spawnPlat.startPoint = Vector3.MoveTowards(spawnPlat.startPoint, CalculateAdjPos(spawnPlat.startPoint), step);
        spawnPlat.endPoint = Vector3.MoveTowards(spawnPlat.endPoint, CalculateAdjPos(spawnPlat.endPoint), step);
        player.startPosition = Vector3.MoveTowards(player.startPosition, CalculateAdjPos(player.startPosition), step);
        player.spawnEndPoint = Vector3.MoveTowards(player.spawnEndPoint, CalculateAdjPos(player.spawnEndPoint), step);
    }

    private Vector3 CalculateAdjPos(Vector3 initialPos)
    {
        Vector3 adjPos = new Vector3(initialPos.x, initialPos.y + mainCamera.panAdjDist, initialPos.z); 

        return adjPos;
    }

    public void MoveStage()
    {
        int waterLayer = LayerMask.NameToLayer("Water");        
        player.SetLayer(waterLayer);
        
        foreach(GameObject stageObj in stageObjects) {
            if(stageObj.tag != "Platforms" && stageObj.tag != "PlayArea") {
                stageObj.SetActive(false);
            }

            stageObj.transform.position = CalculateAdjPos(stageObj.transform.position);  
        }
    }

    public void EndDisplacement()
    {
        wallObject.SetActive(false);
        mainCamera.SetInitialShakePos();

        foreach(GameObject stageObj in stageObjects) {
            if(stageObj.tag != "Platforms" && stageObj.tag != "PlayArea") {
                stageObj.SetActive(true);
            }
        }
    }

    private void CheckForObstacle()
    {
        if(currentObstacles.Count < masterController.currentLevel.maxObstacles) {
            int rand = Random.Range(0,100);

            if(rand < obstacleChance) {
                GameObject obstacleObj = displacementPool.GetPooledObject(masterController.currentLevel.obstacleName);
                Obstacle obstacleScript = obstacleObj.GetComponent<Obstacle>();
                obstacleScript.SetSide();

                if(obstacleScript.isSided && (obstacleScript.side == "left")) {
                    Vector3 cameraCorner = mainCamera.GetCurrentCorner("lowerleft"); 
                    obstacleObj.transform.position = new Vector3( cameraCorner.x, cameraCorner.y, transform.position.z );
                } else {
                    Vector3 cameraCorner = mainCamera.GetCurrentCorner("upperright"); 
                    obstacleObj.transform.position = new Vector3( cameraCorner.x, cameraCorner.y, transform.position.z);
                }

                obstacleObj.transform.SetParent(wallObject.transform);
                obstacleObj.SetActive(true);
                obstacleScript.AdjustPosToSide();
                currentObstacles.Add(obstacleScript);
            }
        }
    }
}
