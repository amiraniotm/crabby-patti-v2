using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplacementController : MonoBehaviour
{
    [SerializeField] private GameObject[] stageObjects;
    [SerializeField] private GameObject wallObject, spawnPlatObject,platPrefab,entryPoint;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float spawnInterval;
    [SerializeField] private PlayerSpawnPlatform spawnPlat;

    private MasterController masterController;
    private CameraMovement mainCamera;
    private GameObject latestPlat;
    private List<GameObject> spawnedPlats = new List<GameObject>();
    private bool initialPlat = true;
    private float platWidth;
    private float platHeight;

    private void Awake()
    {
        SpriteRenderer platRenderer = platPrefab.GetComponent<SpriteRenderer>();
        platWidth = platRenderer.bounds.size.x;
        platHeight = platRenderer.bounds.size.y;
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
    }


    private void SetPlatform()
    {
        if(initialPlat){
            latestPlat = Instantiate(platPrefab, spawnPlatObject.transform.position, Quaternion.identity);
            initialPlat = false;
        } else {
            Vector3 newPlatPos = GetRandomPlatPos();
            latestPlat = Instantiate(platPrefab, newPlatPos, Quaternion.identity);
        }

        spawnedPlats.Add(latestPlat);
    }

    private Vector3 GetRandomPlatPos()
    {
        float randX = Random.Range(latestPlat.transform.position.x - (3 * platWidth), 
                                    latestPlat.transform.position.x + (3 * platWidth));
        
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

    public void StopPlatforms()
    {
        CancelInvoke();
    }

    public void DestroyPlatforms()
    {
        foreach(GameObject plat in spawnedPlats) {
            Destroy(plat);
        }
    }

    private void Update()
    {
        if(masterController != null && masterController.scrollPhase) {
            MoveWalls();
            MoveSpawnPoint();
        }
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
            stageObj.transform.position = CalculateAdjPos(stageObj.transform.position);  
        }
    }

    public void EndDisplacement()
    {
        GameObject wallObject = GameObject.FindGameObjectWithTag("Walls");
        wallObject.SetActive(false);
        DestroyPlatforms();
    }
}
