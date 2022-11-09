using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeChecker : MonoBehaviour
{
    [SerializeField] private float raycastXGap, raycastYMod;

    public Vector2 frontRaycastOrigin, backRaycastOrigin, raycastDirection;
    public float frontFace, backFace, raycastMaxDistance;
    public RaycastHit2D frontEdgeHit, backEdgeHit;
    protected Renderer mainRenderer;
    int platformLayer;
    public string direction = "right";
    public bool platChecked;

    private void Awake()
    {
        platformLayer = LayerMask.GetMask("Platforms");
        mainRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        SetEdgeRaycast();

        CheckEdges();
    }

    protected void SetEdgeRaycast()
    {
        float size = mainRenderer.bounds.size.x;
        
        if(direction == "left"){
            frontFace = mainRenderer.bounds.min.x - raycastXGap;
            backFace = mainRenderer.bounds.max.x + raycastXGap;
        } else {
            frontFace = mainRenderer.bounds.max.x + raycastXGap;
            backFace = mainRenderer.bounds.min.x - raycastXGap;
        }

        frontRaycastOrigin = new Vector2(frontFace, mainRenderer.bounds.center.y);
        backRaycastOrigin = new Vector2(backFace, mainRenderer.bounds.center.y);
        raycastDirection = transform.TransformDirection(Vector2.down);
        raycastMaxDistance = raycastYMod * mainRenderer.bounds.extents.y;
    }

    private void CheckEdges()
    {
        platChecked = true;
        frontEdgeHit = Physics2D.Raycast(frontRaycastOrigin, raycastDirection, raycastMaxDistance, platformLayer);
        backEdgeHit = Physics2D.Raycast(backRaycastOrigin, raycastDirection, raycastMaxDistance, platformLayer);
        Debug.DrawRay(backRaycastOrigin, raycastDirection * raycastMaxDistance, Color.red );
        Debug.DrawRay(frontRaycastOrigin, raycastDirection * raycastMaxDistance, Color.green ); 
    }
}
