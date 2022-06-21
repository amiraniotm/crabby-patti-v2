using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] LevelDisplay levelDisplay;
    [SerializeField] PauseController pauseController;
    [SerializeField] TileManager tileManager;

    private BoxCollider2D itemZone;
    public bool lifeSpawned = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        itemZone = GetComponent<BoxCollider2D>();

        InvokeRepeating("SpawnLife", 1f, 2f);
    }

    private void Update()
    {
        if(levelDisplay.timeCount < (levelDisplay.currentLevel.levelTime / 2) && !lifeSpawned) {
            SpawnLife();
        }

        /**Vector2 itemZoneTopRight = screenTopRight + new Vector2(-(itemZoneAdjust / 2), -(itemZoneAdjust / 2));
        Vector2 itemZoneBottomLeft = screenBottomLeft + new Vector2((itemZoneAdjust / 2), (itemZoneAdjust / 2));
        Vector2 itemZoneTopLeft = new Vector2(itemZoneTopRight.x - itemZoneWidth, itemZoneTopRight.y);
        Vector2 itemZoneBottomRight = new Vector2(itemZoneBottomLeft.x + itemZoneWidth, itemZoneBottomLeft.y );

        Debug.DrawLine(itemZoneTopRight, itemZoneTopLeft, Color.red);
        Debug.DrawLine(itemZoneTopLeft, itemZoneBottomLeft, Color.red);
        Debug.DrawLine(itemZoneBottomLeft, itemZoneBottomRight, Color.red);
        Debug.DrawLine(itemZoneBottomRight, itemZoneTopRight, Color.red);**/
    }

    private void SpawnLife()
    {
        lifeSpawned = true;
        
        bool itemSet = false;

        while(!itemSet){
            float randomX = Random.Range(itemZone.bounds.min.x, itemZone.bounds.max.x);
            float randomY = Random.Range(itemZone.bounds.min.y, itemZone.bounds.max.y);

            Vector2 newItemPos = new Vector2(randomX, randomY);

            bool isColliding = Physics.CheckSphere(newItemPos, 4f);

            if(itemZone.bounds.Contains(newItemPos) && !isColliding && !tileManager.CheckForTile(newItemPos)) {
                Instantiate(itemPrefab, newItemPos,Quaternion.identity);
                itemSet = true;
            }
        }
    }

}
