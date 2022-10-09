using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    //[SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private EnemyPool enemyPool;

    protected EnemyCounter enemyCounter;
    public bool lastSpawned;
    public bool spawning = false;
    public bool readyToSpawn = true;
    public float readyTime;
    private GameObject currentEnemyObject;

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

        currentEnemyObject = enemyPool.GetPooledEnemy(enemyType);
        
        if(currentEnemyObject != null) {
            currentEnemyObject.SetActive(true);
            currentEnemyObject.transform.position = transform.position;
            
            Enemy enemyScript = currentEnemyObject.GetComponent<Enemy>();
            enemyScript.originPosition = transform.position;
            enemyScript.type = enemyType;
            enemyScript.spawnPoint = gameObject.GetComponent<SpawnPoint>();
            enemyScript.Start();

            lastSpawned = !lastSpawned;
        }
    }

    private IEnumerator GetReadyRoutine()
    {
        yield return new WaitForSeconds(readyTime);

        readyToSpawn = true;
    }

}
