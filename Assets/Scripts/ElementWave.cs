using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ElementWave : MonoBehaviour
{
    [SerializeField] private TileBase elementTile;

    private TileManager tileManager;
    public EdgeChecker edgeChecker;
    private float speed = 12.0f;
    protected PlatformCollision platforms;
    protected Rigidbody2D body;
    protected new BoxCollider2D collider;
    protected Renderer[] renderers;    
    protected float onTime = 0.4f;
    public string status;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        platforms = GameObject.FindGameObjectWithTag("Platforms").GetComponent<PlatformCollision>();
        renderers = GetComponentsInChildren<Renderer>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        edgeChecker = GetComponent<EdgeChecker>();

        StartCoroutine(QuenchCoroutine());
    }

    private void Start()
    {
        ShiftDirection();
    }

    private void Update()
    {
        body.velocity = new Vector2(speed, body.velocity.y);
        
        CheckRenderers();

        if(edgeChecker.backEdgeHit){
            tileManager.SwapTile(edgeChecker.backEdgeHit.point, elementTile);
        }
        
        if(edgeChecker.frontEdgeHit){
            tileManager.SwapTile(edgeChecker.frontEdgeHit.point, elementTile);
        } else if(edgeChecker.platChecked && !edgeChecker.frontEdgeHit){
            Quench();
        }  
    }

    private void ShiftDirection()
    {
        if(edgeChecker.direction == "left") {
            speed *= -1;
            transform.localScale *= new Vector2(-1,1);
        }
    }

    private void CheckRenderers()
    {
        foreach(var renderer in renderers)
        {
            // If at least one render is visible, return true
            if(renderer.isVisible)
            {
                return;
            }
        }

        Quench();
    }

    protected void Quench()
    {
        tileManager.RefreshTileList();
        Destroy(gameObject);
    }

    protected IEnumerator QuenchCoroutine()
    {
        yield return new WaitForSeconds(onTime);

        Quench();
    }
}
