using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ElementWave : MonoBehaviour
{
    [SerializeField] private TileBase elementTile;

    private TileManager tileManager;
    public string direction = "right";
    private float speed = 12.0f;
    protected PlatformCollision platforms;
    protected Rigidbody2D body;
    protected new BoxCollider2D collider;
    protected Renderer[] renderers;
    protected Vector2 frontRaycastOrigin;
    protected Vector2 backRaycastOrigin;
    protected Vector2 raycastDirection;
    protected float raycastMaxDistance;
    protected float frontFace;
    protected float backFace;
    protected RaycastHit2D frontEdgeHit;
    protected RaycastHit2D backEdgeHit;
    protected float onTime = 0.4f;
    public string status;
    int platformLayer;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        platforms = GameObject.FindGameObjectWithTag("Platforms").GetComponent<PlatformCollision>();
        renderers = GetComponentsInChildren<Renderer>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        platformLayer = LayerMask.GetMask("Platforms");

        StartCoroutine(QuenchCoroutine());

        ShiftDirection();
    }

    void Update()
    {
        body.velocity = new Vector2(speed, body.velocity.y);

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
            frontFace = collider.bounds.min.x - 0.1f;
            backFace = collider.bounds.max.x + 0.1f;
        } else {
            frontFace = collider.bounds.max.x + 0.1f;
            backFace = collider.bounds.min.x - 0.1f;
        }

        frontRaycastOrigin = new Vector2(frontFace, collider.bounds.center.y);
        backRaycastOrigin = new Vector2(backFace, collider.bounds.center.y);
        raycastDirection = transform.TransformDirection(Vector2.down);
        raycastMaxDistance = 2.0f * collider.bounds.extents.y;
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
        frontEdgeHit = Physics2D.Raycast(frontRaycastOrigin, raycastDirection, raycastMaxDistance, platformLayer);
        backEdgeHit = Physics2D.Raycast(backRaycastOrigin, raycastDirection, raycastMaxDistance, platformLayer);
        //Debug.DrawRay(backRaycastOrigin, raycastDirection * raycastMaxDistance, Color.red );
        //Debug.DrawRay(frontRaycastOrigin, raycastDirection * raycastMaxDistance, Color.green );

        if(backEdgeHit){
            tileManager.SwapTile(backEdgeHit.point, elementTile);
        }
        
        if(frontEdgeHit){
            tileManager.SwapTile(frontEdgeHit.point, elementTile);
        } else if(!frontEdgeHit){
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
        yield return new WaitForSeconds(onTime);

        Quench();
    }
}
