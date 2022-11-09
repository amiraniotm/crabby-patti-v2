using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] public float walkSpeed;
    [SerializeField] public float runSpeed;
    [SerializeField] protected float topJumpSpeed;
    [SerializeField] public EnemyCounter enemyCounter;

    protected PlatformCollision platforms;
    protected Animator animator;
    protected Renderer mainRenderer;
    protected SpriteRenderer spriteRenderer;
    protected ScreenWrap screenWrap;
    protected float adjustedJumpSpeed;
    public MasterController masterController;
    public Vector2 originPosition;

    protected bool canJump;
    protected bool flippedHorizontal;
    public bool isDead = false;
    public bool flippedVertical = false;
    public bool grounded;
    public bool spawned = false;
    protected float currentJumpSpeed = 0.0f;
    
    public bool onGround = false, onTop = false, onMid = false;
    public bool spawning = true;
    new public BoxCollider2D collider; 
    public Rigidbody2D body;  
    
    protected void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        screenWrap = GetComponent<ScreenWrap>();
        mainRenderer = GetComponent<Renderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        platforms = GameObject.FindGameObjectWithTag("Platforms").GetComponent<PlatformCollision>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        collider = GetComponent<BoxCollider2D>();
        
        adjustedJumpSpeed = topJumpSpeed;
    }

    protected void Jump()
    {
        if(!isDead && canJump){
            body.velocity = new Vector2(body.velocity.x, adjustedJumpSpeed);
        }else{
            body.velocity = new Vector2(body.velocity.x, 2 * adjustedJumpSpeed / 3);
        }

        grounded = false;
    }
    
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "SpawnPlatform" || collision.gameObject.tag == "FloatingPlatform"){
            
            string collisionSide = platforms.DetectCollisionDirection(collision);

            if(collisionSide == "upper"){
                grounded = true;    
            }     
        }
    }

    
    protected void OnCollisionExit2D(Collision2D collision) 
    {
        if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "SpawnPlatform") {
            grounded = false;    
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Ground" && gameObject.tag != "Player") {
            onGround = true;
        }

        if(otherCollider.gameObject.tag == "TopArea") {
            onTop = true;
        }

        if(otherCollider.gameObject.tag == "MidArea") {
            onMid = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider) 
    {
        if(otherCollider.gameObject.tag == "Ground" && gameObject.tag != "Player") {
            onGround = false;
        }

        if(otherCollider.gameObject.tag == "TopArea") {
            onTop = false;
        }   

        if(otherCollider.gameObject.tag == "MidArea") {
            onMid = false;
        }
    }
    
    protected void Stop()
    {
        body.velocity = new Vector2(0, body.velocity.y);
    }
    
    protected void Hold()
    {
        body.velocity = new Vector2(0, 0);
    }

}
