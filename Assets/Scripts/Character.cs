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
    protected Rigidbody2D body;
    protected Animator animator;
    protected ScreenWrap screenWrapScript;
    protected float adjustedJumpSpeed;

    protected bool canJump;
    protected bool flippedHorizontal;
    public bool isDead = false;
    public bool flippedVertical = false;
    public bool grounded;
    public bool spawned = false;
    protected float currentJumpSpeed = 0.0f;
    
    public bool onGround = false;
    public bool spawning = true;
    
    protected void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        screenWrapScript = GetComponent<ScreenWrap>();
        platforms = GameObject.FindGameObjectWithTag("Platforms").GetComponent<PlatformCollision>();
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
        if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "SpawnPlatform"){
            
            string collisionSide = platforms.DetectCollisionDirection(collision);

            if(collisionSide == "upper"){
                grounded = true;    
            }     
        }
    }

    
    protected void OnCollisionExit2D(Collision2D collision) 
    {
        if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "SpawnPlatform")
            grounded = false;    
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