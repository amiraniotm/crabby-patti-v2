using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] public SpawnPoint[] spawnPoints;
    [SerializeField] public GameObject[] availableBosses; 
    
    private Level currentLevel;
    public MasterController masterController;
    public List<GameObject> currentEnemies = new List<GameObject>();

    private int currentPhase;
    public float spawnInterval = 2.5f;
    public bool stillSpawing;

    private void Awake()
    {
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetEnemyCounter(this);
    }

    public void Start()
    {
        stillSpawing = true;
    
        currentLevel = masterController.currentLevel;
        currentPhase = masterController.currentPhaseKey - 1;
        InvokeRepeating("NewEnemy", spawnInterval, spawnInterval);

        foreach(SpawnPoint spawnPt in spawnPoints) {
            spawnPt.Start();
        }
    }

    private void NewEnemy()
    {
        foreach(SpawnPoint spawnPoint in spawnPoints){
            if(spawnPoint.lastSpawned){
                spawnPoint.lastSpawned = !spawnPoint.lastSpawned;
            }else{
                foreach(KeyValuePair<string,int> keyValue in currentLevel.levelEnemies[currentPhase]) {
                    if(keyValue.Value > 0) {
                        string enemyType = keyValue.Key;
                        spawnPoint.SpawnEnemy(enemyType);
                        currentLevel.levelEnemies[currentPhase][keyValue.Key] = keyValue.Value - 1;
                        currentLevel.enemyCount[currentPhase] -= 1;
                        
                        if(currentLevel.enemyCount[currentPhase] == 0) {
                            stillSpawing = false;
                            CancelInvoke();
                        }

                        break;
                    }
                }
            }
        }
    }

    public void EnemyDied()
    {
        masterController.CheckEnemies();
    }

    public void FlipAll()
    {
        foreach(GameObject enemy in currentEnemies) {
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            if(enemyScript.grounded) {
                enemyScript.FlipVertical();
            }
        }
    }

    public void SpawnBoss()
    {
        int bossKey = masterController.currentLevelKey - 1;
        GameObject currentBoss = availableBosses[bossKey];

        currentBoss.SetActive(true); 
    }

}
