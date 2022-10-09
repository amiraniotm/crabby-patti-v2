using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Level", menuName = "Levels")] 
public class Level : ScriptableObject
{
    [SerializeField] public string levelType; 
    [SerializeField] public float levelTime;
    [SerializeField] public int levelPhases;

    public List<Dictionary<string,int>> levelEnemies = new List<Dictionary<string, int>>();
    public List<int> enemyCount = new List<int>();

    public void SetEnemies()
    {        
        enemyCount = new List<int>();
        levelEnemies = new List<Dictionary<string, int>>();

        for(int i = 0; i < levelPhases; i++) {
            enemyCount.Add(0);
            levelEnemies.Add(new Dictionary<string, int>());

            levelEnemies[i].Add("Crabcatcher", 0);
            levelEnemies[i].Add("CrabcatcherPlus", 0);
            levelEnemies[i].Add("ReptAgent", 0);
            levelEnemies[i].Add("ReptBaby", 0);
            levelEnemies[i].Add("Flamey", 0);
            levelEnemies[i].Add("ReptLizard", 0);
            levelEnemies[i].Add("Icey", 0);
            levelEnemies[i].Add("Gooey", 0);
            
            //TEST ENEMIES
            if(levelType == "beach") {
                levelEnemies[i]["Crabcatcher"] = 1;
                levelEnemies[i]["CrabcatcherPlus"] = 0;
                levelEnemies[i]["ReptAgent"] = 0;
                levelEnemies[i]["ReptBaby"] = 0;
                levelEnemies[i]["Flamey"] = 0;
                levelEnemies[i]["ReptLizard"] = 0;
                levelEnemies[i]["Icey"] = 0;
                levelEnemies[i]["Gooey"] = 0;
            } else if(levelType == "volcano") {
                levelEnemies[i]["Crabcatcher"] = 1;
                levelEnemies[i]["CrabcatcherPlus"] = 2;
                levelEnemies[i]["ReptAgent"] = 0;
                levelEnemies[i]["Flamey"] = 0;
                levelEnemies[i]["ReptLizard"] = 0;
                levelEnemies[i]["Icey"] = 0;
                levelEnemies[i]["Gooey"] = 0;
            } else if(levelType == "city") {
                levelEnemies[i]["Crabcatcher"] = 2;
                levelEnemies[i]["CrabcatcherPlus"] = 3;
                levelEnemies[i]["ReptAgent"] = 0;
                levelEnemies[i]["ReptBaby"] = 0;
                levelEnemies[i]["Flamey"] = 0;
                levelEnemies[i]["ReptLizard"] = 0;
                levelEnemies[i]["Icey"] = 0;
                levelEnemies[i]["Gooey"] = 0;
            }
            /**
            //CURRENT GAME ENEMIES
            if(levelType == "beach") {
                levelEnemies["Crabcatcher"] = 3;
                levelEnemies["CrabcatcherPlus"] = 3;
                levelEnemies["ReptAgent"] = 2;
                levelEnemies["ReptBaby"] = 0;
                levelEnemies["Flamey"] = 0;
            } else if(levelType == "volcano") {
                levelEnemies["Crabcatcher"] = 2;
                levelEnemies["ReptLizard"] = 1;
                levelEnemies["CrabcatcherPlus"] = 2;
                levelEnemies["Gooey"] = 1;
                levelEnemies["ReptAgent"] = 2;
                levelEnemies["Icey"] = 1;
                levelEnemies["Flamey"] = 1;
            } else if(levelType == "city") {
                levelEnemies["Crabcatcher"] = 2;
                levelEnemies["CrabcatcherPlus"] = 2;
                levelEnemies["ReptLizard"] = 2;
                levelEnemies["Icey"] = 2;
                levelEnemies["ReptAgent"] = 3;
                levelEnemies["Flamey"] = 2;
                levelEnemies["Gooey"] = 2;
                levelEnemies["ReptBaby"] = 2;
            }**/
            
            foreach(KeyValuePair<string,int> enemy in levelEnemies[i]){
                enemyCount[i] += enemy.Value;
            }
        }


    }   
}