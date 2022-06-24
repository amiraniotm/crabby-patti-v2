using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private TileBase charredTile;

    [SerializeField]
    private List<TileData> tileDatas;

    private Dictionary<TileBase,TileData> dataFromTiles;

    public BoxCollider2D playerCollider;
    private Tilemap tilemap; 
    private float unflipCounter = 0.15f;
    private Vector3Int newTilePosition;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (tilemap == null) {
            SetTileMap();
        }
    }

    public void SetTileMap()
    {
        tilemap = GameObject.FindGameObjectWithTag("Platforms").GetComponent<Tilemap>();
        RefreshTileList();
    }

    public void RefreshTileList()
    {
        dataFromTiles = new Dictionary<TileBase,TileData>();

        foreach(var tileData in tileDatas) {
            foreach(var tile in tileData.tiles ) {
                dataFromTiles.Add(tile, tileData);
            }   
        } 

    }

    public void FlipEnemies(Collision2D collision)
    {
        
        BoxCollider2D flipbox = gameObject.AddComponent<BoxCollider2D>();
        flipbox.transform.position = new Vector2(collision.contacts[0].point.x , collision.contacts[0].point.y + (tilemap.cellSize.y) );
        flipbox.size = new Vector2(playerCollider.size.x * 2.5f, playerCollider.size.y * 1.5f );
        
        StartCoroutine(FlipboxCoroutine(flipbox));        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemies") {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            enemy.FlipVertical();
        }
    }

    private IEnumerator FlipboxCoroutine(BoxCollider2D flipbox)
    {
        yield return new WaitForSeconds(unflipCounter);

        Destroy(flipbox);
    }

    public void BurnTile(Vector2 firePosition, GameObject objectHit)
    {
        Vector3Int touchPosition = tilemap.WorldToCell(firePosition);

        newTilePosition = new Vector3Int(touchPosition.x, touchPosition.y - 1, touchPosition.z);
        
        Vector3Int formerTilePosition = new Vector3Int(newTilePosition.x - 1, newTilePosition.y, newTilePosition.z);
        if(tilemap.HasTile(newTilePosition)) {
            tilemap.SetTile(newTilePosition, charredTile);
            tilemap.SetTile(formerTilePosition, charredTile); 
        }
    }

    public float GetTileSpeedMod(Vector2 playerPosition, GameObject objectHit)
    {
        Vector3Int tilePosition = tilemap.WorldToCell(playerPosition);

        newTilePosition = new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z);

        if(tilemap.HasTile(newTilePosition)) {
            TileBase tile = tilemap.GetTile(newTilePosition);

            if(tile == null){
                return 1.0f;
            }

            return dataFromTiles[tile].speedMod;
        } else {
            return 1.0f;
        } 
    }

    public bool CheckForTile(Vector2 worldPosition)
    {
        Vector3Int tilePosition = tilemap.WorldToCell(worldPosition);

        return tilemap.HasTile(tilePosition);
    }
}
