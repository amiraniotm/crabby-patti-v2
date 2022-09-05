using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatformController : MonoBehaviour
{
    [SerializeField] private GameObject platPrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private CameraMovement mainCamera;

    private Vector3 initialSpawnPoint;
    private PlayerSpawnPlatform spawnPlat;
    private GameObject spawnPlatObject;
    private GameObject latestPlat;
    private bool initialPlat = true;
    private float platWidth;
    private float platHeight;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SpriteRenderer platRenderer = platPrefab.GetComponent<SpriteRenderer>();
        platWidth = platRenderer.bounds.size.x;
        platHeight = platRenderer.bounds.size.y;
    }

    public void GetSpawnPoint()
    {
        spawnPlatObject = null;
        StartCoroutine(GetSPawnPointCoroutine());
    }

    public void TriggerPlatforms()
    {
        InvokeRepeating("SetPlatform", 0f, spawnInterval);
    }

    private void SetPlatform()
    {
        if(initialPlat){
            latestPlat = Instantiate(platPrefab, initialSpawnPoint, Quaternion.identity);
            initialPlat = false;
        } else {
            Vector3 newPlatPos = GetRandomPlatPos();
            latestPlat = Instantiate(platPrefab, newPlatPos, Quaternion.identity);
        }
    }

    private Vector3 GetRandomPlatPos()
    {
        float randX = Random.Range(latestPlat.transform.position.x - (3 * platWidth), 
                                    latestPlat.transform.position.x + (3 * platWidth));
        
        if(randX < -(mainCamera.screenWidth / 2)) {
            randX = -mainCamera.screenWidth / 2;
        } else if (randX > (mainCamera.screenWidth / 2) ) {
            randX = mainCamera.screenWidth / 2;
        }

        float currentCameraY = mainCamera.gameObject.transform.position.y;
        float randY = currentCameraY + (mainCamera.screenHeight / 2) + platHeight;

        Vector3 randPos = new Vector3(randX, randY, latestPlat.transform.position.z);
        
        return randPos;
    }

    private IEnumerator GetSPawnPointCoroutine()
    {
        while(spawnPlatObject == null) {
            spawnPlatObject = GameObject.FindGameObjectWithTag("SpawnPlatform");
            yield return 0;
        }

        spawnPlat = spawnPlatObject.GetComponent<PlayerSpawnPlatform>();
        initialSpawnPoint = spawnPlat.transform.position;
    }
}
