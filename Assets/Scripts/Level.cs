using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level", menuName = "Levels")] 
public class Level : ScriptableObject
{
    [SerializeField] public string levelType; 
    [SerializeField] public float levelTime;

    public Dictionary<string,int> levelEnemies = new Dictionary<string, int>();
    public int enemyCount;

    public void SetEnemies()
    {        
        enemyCount = 0;

        if( levelEnemies.Count == 0 ) {
            levelEnemies.Add("Crabcatcher", 0);
            levelEnemies.Add("CrabcatcherPlus", 0);
            levelEnemies.Add("ReptAgent", 0);
            levelEnemies.Add("ReptBaby", 0);
            levelEnemies.Add("Flamey", 0);
        }
        
        //TEST ENEMIES
        /**if(levelType == "beach") {
            levelEnemies["Crabcatcher"] = 2;
            levelEnemies["CrabcatcherPlus"] = 0;
            levelEnemies["ReptAgent"] = 0;
            levelEnemies["ReptBaby"] = 0;
            levelEnemies["Flamey"] = 0;
        } else if(levelType == "volcano") {
            levelEnemies["Crabcatcher"] = 1;
            levelEnemies["CrabcatcherPlus"] = 2;
            levelEnemies["ReptAgent"] = 0;
            levelEnemies["Flamey"] = 0;
        } else if(levelType == "city") {
            levelEnemies["Crabcatcher"] = 2;
            levelEnemies["CrabcatcherPlus"] = 3;
            levelEnemies["ReptAgent"] = 0;
            levelEnemies["ReptBaby"] = 0;
            levelEnemies["Flamey"] = 0;
        }**/

        //CURRENT GAME ENEMIES
        if(levelType == "beach") {
            levelEnemies["Crabcatcher"] = 4;
            levelEnemies["CrabcatcherPlus"] = 4;
            levelEnemies["ReptAgent"] = 0;
            levelEnemies["ReptBaby"] = 0;
            levelEnemies["Flamey"] = 0;
        } else if(levelType == "volcano") {
            levelEnemies["Crabcatcher"] = 3;
            levelEnemies["CrabcatcherPlus"] = 3;
            levelEnemies["ReptAgent"] = 2;
            levelEnemies["Flamey"] = 2;
        } else if(levelType == "city") {
            levelEnemies["Crabcatcher"] = 4;
            levelEnemies["CrabcatcherPlus"] = 3;
            levelEnemies["ReptAgent"] = 3;
            levelEnemies["ReptBaby"] = 3;
            levelEnemies["Flamey"] = 3;
        } else if(levelType == "practice") {
            levelEnemies["Crabcatcher"] = 1;
            levelEnemies["CrabcatcherPlus"] = 1;
            levelEnemies["ReptAgent"] = 0;
            levelEnemies["ReptBaby"] = 0;
            levelEnemies["Flamey"] = 0;
        } 


        foreach(KeyValuePair<string,int> enemy in levelEnemies){
            enemyCount += enemy.Value;
        }
    }  

    public void SetPracticeEnemies(string newEnemyName)
    {
        Dictionary<string,int> enemiesCopy = new Dictionary<string, int>(levelEnemies);

        foreach(KeyValuePair<string,int> enemy in enemiesCopy){
            if(enemy.Value > 0) {
                levelEnemies[enemy.Key] = 0;
            }

            if(enemy.Key == newEnemyName) {
                levelEnemies[enemy.Key] = 1;
            }
        }
    } 
}