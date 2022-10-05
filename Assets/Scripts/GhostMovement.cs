using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public GameObject originalObject;
    public ScreenWrap screenWrapScript;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer originalSpriteRenderer;
    public Character originalObjectScript;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        originalSpriteRenderer = originalObject.GetComponent<SpriteRenderer>();
        originalObjectScript = originalObject.GetComponent<Character>();
    }

    private void Update()
    {
        if(originalObject != null) {
            SetTransform();
        } else {
            Destroy(gameObject);
        }
    }

    private void SetTransform()
    {
        Vector2 newPosition = transform.position;

        if(!originalObjectScript.onGround) {
            if(transform.position.x > originalObject.transform.position.x){
                newPosition.x = originalObject.transform.position.x + screenWrapScript.screenWidth;
            } else if(transform.position.x < originalObject.transform.position.x){
                newPosition.x = originalObject.transform.position.x - screenWrapScript.screenWidth;
            }
        
            newPosition.y = originalObject.transform.position.y;

            transform.position = newPosition;
            transform.rotation = originalObject.transform.rotation;
            transform.localScale = originalObject.transform.localScale;
            spriteRenderer.sprite = originalSpriteRenderer.sprite;
        } else {
            transform.position = originalObjectScript.originPosition;
        }
    }
}
