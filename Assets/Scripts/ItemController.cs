using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] LevelDisplay levelDisplay;
    [SerializeField] PauseController pauseController;
    [SerializeField] TileManager tileManager;
    [SerializeField] LayerMask platformMask;

    private BoxCollider2D itemZone;
    public bool lifeSpawned = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        itemZone = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if(levelDisplay.timeCount < (levelDisplay.currentLevel.levelTime / 2) && !lifeSpawned) {
            SpawnLife();
        }
    }

    private void SpawnLife()
    {
        lifeSpawned = true;
        
        bool itemSet = false;

        while(!itemSet){
            float randomX = Random.Range(itemZone.bounds.min.x, itemZone.bounds.max.x);
            float randomY = Random.Range(itemZone.bounds.min.y, itemZone.bounds.max.y);

            Vector2 newItemPos = new Vector2(randomX, randomY);

            bool isColliding = Physics.CheckSphere(newItemPos, 4f, platformMask);

            if(itemZone.bounds.Contains(newItemPos) && !isColliding && !tileManager.CheckForTile(newItemPos)) {
                Instantiate(itemPrefab, newItemPos,Quaternion.identity);
                itemSet = true;
            }
        }
    }

}
