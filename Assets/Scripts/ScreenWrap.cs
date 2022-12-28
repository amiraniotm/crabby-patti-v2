using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    [SerializeField] private Transform GhostPrefab;
    [SerializeField] private bool hasGhosts, autoLeave;

    public bool isVisible = false;
    protected Camera cam;
    protected Renderer[] renderers;
    protected Transform[] ghosts = new Transform[2];
    protected Character characterScript;
    public float screenWidth;
    public float screenHeight;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Start()
    {
        characterScript = GetComponent<Character>();

        cam = Camera.main;
        var screenBottomLeft = cam.ViewportToWorldPoint(new Vector2(0, 0));
        var screenTopRight = cam.ViewportToWorldPoint(new Vector2(1, 1));
        
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.y - screenBottomLeft.y;

        if(hasGhosts) {
            CreateGhosts();
        }
    }

    void Update()
    {
        isVisible = CheckRenderers();

        if(characterScript == null || !characterScript.spawning) {
            CheckScreenWrap();
        }
    }

    protected void CheckScreenWrap()
    {
        if(!isVisible){
            if(characterScript != null && characterScript.isDead && gameObject.tag == "Enemies") {
                characterScript.enemyCounter.currentEnemies.Remove(gameObject);
                Enemy enemyScript = GetComponent<Enemy>();
                enemyScript.Vanish();
            } else if(gameObject.transform.position.y < (cam.gameObject.transform.position.y - (screenHeight / 2)) && gameObject.tag == "Player") {
                PlayerMovement playerScript = GetComponent<PlayerMovement>();
                playerScript.Die();
            } else if(characterScript != null && !characterScript.isDead && !characterScript.spawning && !characterScript.masterController.scrollPhase) {
                if(gameObject.tag != "Enemies" || (gameObject.tag == "Enemies" && !characterScript.onGround)) {
                    GhostSwap();
                }
            } else if(!hasGhosts && !autoLeave) {
                gameObject.SetActive(false);
            }
            
        }
    }

    private bool CheckRenderers()
    {
        foreach(var renderer in renderers)
        {
            // If at least one render is visible, return true
            if(renderer.isVisible)
            {
                return true;
            }
        }

        // Otherwise, the object is invisible
        return false;
    }

    private void CreateGhosts()
    {
        for(int i = 0; i < 2; i++)
        {
            ghosts[i] = Instantiate(GhostPrefab, transform.position, Quaternion.identity);
            GhostMovement newGhost = ghosts[i].GetComponent<GhostMovement>();
            newGhost.originalObject = gameObject;
            newGhost.screenWrap = this;
        }

        PositionGhosts();
    }

    private void PositionGhosts()
    {
        // All ghost positions will be relative to the ships (this) transform,
        // so let's star with that.
        var newGhostPosition = transform.position;
    
        // We're positioning the ghosts behind the edges of the screen.
        // Right
        newGhostPosition.x = transform.position.x + screenWidth;
        newGhostPosition.y = transform.position.y;
        ghosts[0].position = newGhostPosition;
    
        // Left
        newGhostPosition.x = transform.position.x - screenWidth;
        newGhostPosition.y = transform.position.y;
        ghosts[1].position = newGhostPosition;
    
        // All ghost ships should have the same rotation as the main ship
        for(int i = 0; i < 2; i++)
        {
            ghosts[i].rotation = transform.rotation;
        }
    }

    private void GhostSwap()
    {
        foreach(var ghost in ghosts) {
            if (ghost.position.x < screenWidth && ghost.position.x > -screenWidth) {
                transform.position = ghost.position;
    
                break;
            }
        }
    }
}
