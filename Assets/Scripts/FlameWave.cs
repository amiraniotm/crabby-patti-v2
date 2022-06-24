using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWave : MonoBehaviour
{
    private TileManager tileManager;
    public string direction = "right";
    private float speed = 12.0f;
    protected PlatformCollision platforms;
    protected Rigidbody2D body;
    protected new BoxCollider2D collider;
    protected Renderer[] renderers;
    protected Vector2 raycastOrigin;
    protected Vector2 raycastDirection;
    protected float raycastMaxDistance;
    protected float faceCenter;
    protected RaycastHit2D edgeHit;
    protected float burnTime = 0.5f;
    public string status;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        platforms = GameObject.FindGameObjectWithTag("Platforms").GetComponent<PlatformCollision>();
        renderers = GetComponentsInChildren<Renderer>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();

        StartCoroutine(QuenchCoroutine());

        ShiftDirection();
    }

    void Update()
    {
        body.gravityScale = 0.0f;
        body.velocity = new Vector2(speed, 0);

        CheckRenderers();

        SetEdgeRaycast();

        CheckEdges();
    }

    private void ShiftDirection()
    {
        if(direction == "left") {
            speed *= -1;
            transform.localScale *= new Vector2(-1,1);
        }
    }

    protected void SetEdgeRaycast()
    {
        float size = collider.bounds.size.x;
        
        if(direction == "left"){
            faceCenter = collider.bounds.min.x - 0.1f;
        } else {
            faceCenter = collider.bounds.max.x + 0.1f;
        }

        raycastOrigin = new Vector2(faceCenter, collider.bounds.center.y);
        raycastDirection = transform.TransformDirection(Vector2.down);
        raycastMaxDistance = 3 * collider.bounds.extents.y;
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

    private void CheckEdges()
    {
        edgeHit = Physics2D.Raycast(raycastOrigin, raycastDirection, raycastMaxDistance);
        
        if(edgeHit){
            GameObject objectHit = edgeHit.transform.gameObject;

            tileManager.BurnTile(edgeHit.point, objectHit);
        } else if(!edgeHit){
            Quench();
        }
        
    }

    protected void Quench()
    {
        tileManager.RefreshTileList();
        Destroy(gameObject);
    }

    protected IEnumerator QuenchCoroutine()
    {
        yield return new WaitForSeconds(burnTime);

        Quench();
    }
}
