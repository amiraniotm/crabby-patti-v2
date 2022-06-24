using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : Character
{
    [SerializeField] public GameObject spawnPlatformObject;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private LevelDisplay levelDisplay;
    [SerializeField] private PauseController pauseController;
    [SerializeField] private float maxJumpTime;
    
    public Vector3 startPosition;
    public Vector3 spawnStartPoint;
    public Vector3 spawnEndPoint;

    protected float speedMod = 1.0f;
    protected RaycastHit2D groundHit;
    protected Vector2 raycastOrigin;
    protected Vector2 raycastDirection;
    protected float raycastMaxDistance;
    protected float adjustedWalkSpeed;
    public bool isJumping;
    
    private float spawnPlatformTimer = 3.0f;
    private float currentJumpTimer;
    private PlayerSpawnPlatform spawnPlatform; 
    new private BoxCollider2D collider;

    new private void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        spawnPlatform = spawnPlatformObject.GetComponent<PlayerSpawnPlatform>();
        collider = GetComponent<BoxCollider2D>();
        startPosition = transform.position;
    }

    public void Start()
    {        
        canJump = true;
        adjustedWalkSpeed = walkSpeed;
        
        spawnStartPoint = spawnPlatformObject.transform.position;
        spawnEndPoint = new Vector3(transform.position.x, transform.position.y - spawnPlatform.spawnGap, transform.position.z);

        PlayerSpawn();
    }

    private void Update()
    {      
        if(!pauseController.gamePaused) {
            if(!spawning){            
                CheckRespawnCondition();

                SetGroundRaycast();

                CheckGround();

                if(!isDead){
                    if(Input.GetKey("space")) {
                        Jump();
                    } 

                    if (Input.GetKeyUp("space")) {
                        isJumping = false;
                    }
                    
                    float horizontalInput = Input.GetAxis("Horizontal");

                    SetInputVelocity(horizontalInput);

                    FlipHorizontal(horizontalInput);

                    animator.SetBool("walking", horizontalInput != 0);
                    animator.SetBool("grounded", grounded);       
                }

                if(body.velocity.y < -0.1) {
                    body.gravityScale = 8.0f;
                } else {
                    body.gravityScale = 5.0f;
                }
            }else if(spawning){
                float step = spawnPlatform.speed * Time.deltaTime;

                Hold();
                transform.position = Vector2.MoveTowards(transform.position, spawnEndPoint, step);
            }
        }  
    }

    protected void SetInputVelocity(float horizontalInput)
    {
        if(horizontalInput != 0 && !spawning) {
            HideRespawnPlatform();
        }
        
        if(grounded) {
            body.velocity = new Vector2(horizontalInput * adjustedWalkSpeed, body.velocity.y);
        } else {
            body.velocity = new Vector2(2 * horizontalInput * adjustedWalkSpeed / 3 , body.velocity.y);    
        }

    }

    protected void CheckRespawnCondition()
    {
        if(!screenWrapScript.isVisible && isDead){
            levelDisplay.PlayerDied();
            PlayerSpawn();

            return;
        }
    }

    protected void FlipHorizontal(float horizontalInput)
    {
        // Flip player sprite when walking
        if(horizontalInput > 0.01f && !flippedHorizontal){
            transform.localScale *= new Vector2(-1,1);
            flippedHorizontal = true;
        }else if(horizontalInput < -0.01f && flippedHorizontal){
            transform.localScale *= new Vector2(-1,1);
            flippedHorizontal = false;
        }
    }

    new protected void Jump()
    {
        if( grounded && spawned ) {
            isJumping = true;
            currentJumpTimer = maxJumpTime;
            body.velocity = Vector2.up * adjustedJumpSpeed;
        }

        if(isJumping){

            if( currentJumpTimer > 0 ) {
                body.velocity = Vector2.up * (adjustedJumpSpeed);
                currentJumpTimer -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        grounded = false;

        animator.SetTrigger("jump");

        if(!spawning){
            HideRespawnPlatform();
        }
    }

    new protected void OnCollisionEnter2D(Collision2D collision) 
    {
        base.OnCollisionEnter2D(collision);
        
        if( collision.gameObject.tag == "Enemies" ) {
            Enemy collidingEnemy = collision.gameObject.GetComponent<Enemy>();

            if(collidingEnemy.flippedVertical) {
                collidingEnemy.Die(body);
                levelDisplay.AddPoints(collidingEnemy.bounty);
                animator.SetTrigger("kick");
            }else if(!collidingEnemy.flippedVertical && !collidingEnemy.isDead) {
                animator.SetBool("dead",true);
                Die();
            }
        } else if ( collision.gameObject.tag == "Projectiles" ) {
            Die();
            animator.SetBool("dead",true);
        }
    }

    public void Die()
    {   
        isDead = true;

        Hold();
        base.Jump();

        collider.enabled = false;
    }

    public void PlayerSpawn() 
    {
        StopCoroutine(SpawnPlatformCoroutine());
        
        gameObject.layer = 4;
        spawning = true;
        spawned = false;
        isDead = false;
        transform.position = startPosition;
        collider.enabled = true;
        spawnPlatformObject.SetActive(true);
        spawnPlatform.coroutineStarted = false;
        animator.SetTrigger("respawn");
        animator.SetBool("dead",false);
    }

    protected void HideRespawnPlatform()
    {
        spawned = true;
        spawnPlatformObject.SetActive(false);
        spawnPlatformObject.transform.position = spawnStartPoint;
        gameObject.layer = 0;
    }

    protected void SetGroundRaycast()
    {
        raycastOrigin = transform.position;
        raycastDirection = transform.TransformDirection(Vector2.down);
        raycastMaxDistance = 2 * collider.bounds.extents.y;
    }

    private void CheckGround()
    {
        groundHit = Physics2D.Raycast(raycastOrigin, raycastDirection, raycastMaxDistance, LayerMask.GetMask("Platforms"));
        //Debug.DrawRay(raycastOrigin, raycastDirection * raycastMaxDistance, Color.red );
        if(groundHit){
            GameObject objectHit = groundHit.transform.gameObject;

            speedMod = tileManager.GetTileSpeedMod(groundHit.point, objectHit);

            adjustedWalkSpeed = walkSpeed * speedMod;
            adjustedJumpSpeed = topJumpSpeed * speedMod;
        }
        
    }

    public void StartPlatformCoroutine()
    {
        if(!levelDisplay.levelStarted) {
            levelDisplay.levelStarted = true;
        }
        
        StartCoroutine(SpawnPlatformCoroutine());
    }

    public IEnumerator SpawnPlatformCoroutine()
    {
        yield return new WaitForSeconds(spawnPlatformTimer);

        if(!spawned && !spawning) {
            HideRespawnPlatform();
        }
    }

}
