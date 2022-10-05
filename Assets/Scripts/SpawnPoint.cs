using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;

    protected EnemyCounter enemyCounter;
    public bool lastSpawned;
    public bool spawning = false;
    public bool readyToSpawn = true;
    public float readyTime;
    private GameObject currentEnemyObject;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("EnemyCounter").GetComponent<EnemyCounter>();
        readyTime = enemyCounter.spawnInterval - 0.5f;

        if(transform.position.x < 0){
            lastSpawned = false;
        }else{
            lastSpawned = true;
        }
    }

    public void SpawnEnemy(string enemyType)
    {
        readyToSpawn = false;

        StartCoroutine(GetReadyRoutine());
        
        if(enemyType == "Crabcatcher"){
            currentEnemyObject = Instantiate(enemyPrefabs[0],transform.position,Quaternion.identity);
        }else if(enemyType == "CrabcatcherPlus"){
            currentEnemyObject = Instantiate(enemyPrefabs[1],transform.position,Quaternion.identity);
        }else if(enemyType == "ReptAgent"){
            currentEnemyObject = Instantiate(enemyPrefabs[2],transform.position,Quaternion.identity);
        }else if(enemyType == "ReptBaby"){
            currentEnemyObject = Instantiate(enemyPrefabs[3],transform.position,Quaternion.identity);
        }else if(enemyType == "Flamey"){
            currentEnemyObject = Instantiate(enemyPrefabs[4],transform.position,Quaternion.identity);
        }else if(enemyType == "ReptLizard"){
            currentEnemyObject = Instantiate(enemyPrefabs[5],transform.position,Quaternion.identity);
        }else if(enemyType == "Icey"){
            currentEnemyObject = Instantiate(enemyPrefabs[6],transform.position,Quaternion.identity);
        }else if(enemyType == "Gooey"){
            currentEnemyObject = Instantiate(enemyPrefabs[7],transform.position,Quaternion.identity);
        }else {
            return;
        }

        Enemy enemyScript = currentEnemyObject.GetComponent<Enemy>();
        enemyScript.originPosition = transform.position;
        enemyScript.type = enemyType;
        enemyScript.spawnPoint = gameObject.GetComponent<SpawnPoint>();

        lastSpawned = !lastSpawned;
    }

    private IEnumerator GetReadyRoutine()
    {
        yield return new WaitForSeconds(readyTime);

        readyToSpawn = true;
    }

}
