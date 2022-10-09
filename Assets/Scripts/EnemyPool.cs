using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    
    private List<GameObject> pooledEnemies = new List<GameObject>();
    public Dictionary<GameObject, int> enemiesToPool = new Dictionary<GameObject, int>();    

    void Start()
    {
        foreach (GameObject enemy in enemyPrefabs)
        {
            if(enemy.name.Contains("Crabcatcher")) {
                enemiesToPool.Add(enemy, 10);
            } else {
                enemiesToPool.Add(enemy, 5);
            };
        }   

        foreach(KeyValuePair<GameObject,int> enemy in enemiesToPool){
            for (int i = 0; i < enemy.Value; i++)
            {
                GameObject pooledEnemy = Instantiate(enemy.Key);
                pooledEnemy.SetActive(false);
                pooledEnemies.Add(pooledEnemy);
            }
        }
    }

    public GameObject GetPooledEnemy(string enemyName)
    {
        for (int i = 0; i <  pooledEnemies.Count; i++)
        {
            string cleanName = pooledEnemies[i].name.Replace("(Clone)", "");
            if(cleanName == enemyName && !pooledEnemies[i].activeInHierarchy) {
                return pooledEnemies[i];
            }
        }
        
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
