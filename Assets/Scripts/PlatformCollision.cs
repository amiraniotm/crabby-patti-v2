using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformCollision : HittableBlock
{
    private Tilemap tilemap;
    private GridLayout gridLayout;
    private TileManager tileManager;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        gridLayout = GetComponentInParent<GridLayout>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
    }

    private void FlipTiles(Collision2D collision)
    {
        tileManager.FlipEnemies(collision);
        
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        GameObject collidingObject = collision.gameObject;
        
        string collisionSide = DetectCollisionDirection(collision);
        
        if(collisionSide == "upper"){
            if(collidingObject.tag == "Player"){

                PlayerMovement player = collidingObject.GetComponent<PlayerMovement>();
                player.isJumping = false;
                FlipTiles(collision);
            }            
        }
    }
}
