using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private List<TileData> tileDatas;
    [SerializeField] private List<TileBase> availableLevelTiles;
    [SerializeField] private GameObject platformObject;
    [SerializeField] private MapDisplacementController mapDisController;

    private Dictionary<TileBase,TileData> dataFromTiles;
    private Renderer[] platformRenderers;
    
    public MasterController masterController;
    public BoxCollider2D playerCollider;
    private Tilemap platformsTileMap; 
    private float unflipCounter = 0.15f;
    private Vector3Int newTilePosition;
    public bool platformsMoved;
    
    private void Awake()
    {
        platformRenderers = platformObject.GetComponentsInChildren<Renderer>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetTileManager(this);
    }

    private void Update()
    {
        if (platformsTileMap == null) {
            SetTileMap();
        }
        

        if(masterController.scrollPhase) {
            bool platformsVisible = ArePlatformsVisible();

            if(!platformsVisible && !platformsMoved) {
                platformsMoved = true;
                mapDisController.MoveStage();
            } else if(platformsVisible && platformsMoved) {
                platformsMoved = false;
                mapDisController.StopPlatforms();
            }
        }
    }

    public void SetTileMap()
    {
        platformsTileMap = GameObject.FindGameObjectWithTag("Platforms").GetComponent<Tilemap>();
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
        flipbox.transform.position = new Vector2(collision.contacts[0].point.x , collision.contacts[0].point.y + (platformsTileMap.cellSize.y) );
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

    public void SwapTile(Vector2 wavePosition, TileBase tileToSwap)
    {
        Vector3Int touchPosition = platformsTileMap.WorldToCell(wavePosition);

        newTilePosition = new Vector3Int(touchPosition.x, touchPosition.y - 1, touchPosition.z);
        
        Vector3Int formerTilePosition = new Vector3Int(newTilePosition.x - 1, newTilePosition.y, newTilePosition.z);
        
        if(platformsTileMap.HasTile(newTilePosition)) {
            platformsTileMap.SetTile(newTilePosition, tileToSwap);
            platformsTileMap.SetTile(formerTilePosition, tileToSwap); 
        } 
        
        if(platformsTileMap.HasTile(touchPosition)) {
            platformsTileMap.SetTile(touchPosition, tileToSwap);
        }
    }

    public float GetTileSpeedMod(Vector2 playerPosition)
    {
        Vector3Int tilePosition = platformsTileMap.WorldToCell(playerPosition);

        newTilePosition = new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z);

        if(platformsTileMap.HasTile(newTilePosition)) {
            TileBase tile = platformsTileMap.GetTile(newTilePosition);

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
        Vector3Int tilePosition = platformsTileMap.WorldToCell(worldPosition);

        return platformsTileMap.HasTile(tilePosition);
    }

    public void SetLevelTiles(int levelKey)
    {
        Tilemap[] levelTileMaps = FindObjectsOfType<Tilemap>();
        
        foreach(Tilemap map in levelTileMaps) {
            BoundsInt bounds = map.cellBounds;
            TileBase[] allTiles = map.GetTilesBlock(bounds);

            foreach(TileBase tile in allTiles) {
                if(tile != null) {
                    map.SwapTile(tile, availableLevelTiles[levelKey]);
                }
            }
        }
    }

    public bool ArePlatformsVisible()
    {
        foreach(var renderer in platformRenderers)
        {
            if(renderer.isVisible)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator FlipboxCoroutine(BoxCollider2D flipbox)
    {
        yield return new WaitForSeconds(unflipCounter);

        Destroy(flipbox);
    }
}
