using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    [SerializeField] public SpawnPoint[] spawnPoints;
    
    
    private Level currentLevel;
    private LevelDisplay levelDisplay;
    public List<GameObject> currentEnemies = new List<GameObject>();

    public float spawnInterval = 1.5f;
    public bool stillSpawing;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        stillSpawing = true;
        
        if(levelDisplay == null) {
            levelDisplay = GameObject.FindGameObjectWithTag("LevelDisplay").GetComponent<LevelDisplay>();
        }

        currentLevel = levelDisplay.currentLevel;
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
                foreach(KeyValuePair<string,int> keyValue in currentLevel.levelEnemies) {
                    if(keyValue.Value > 0) {
                        string enemyType = keyValue.Key;
                        spawnPoint.SpawnEnemy(enemyType);
                        currentLevel.levelEnemies[keyValue.Key] = keyValue.Value - 1;
                        currentLevel.enemyCount -= 1;
                        
                        if(currentLevel.enemyCount == 0) {
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
        levelDisplay.CheckEnemies();
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

}
