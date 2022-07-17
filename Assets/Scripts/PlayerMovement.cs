using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : Character
{
    [SerializeField] public GameObject spawnPlatformObject;
    [SerializeField] public LevelDisplay levelDisplay;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private PauseController pauseController;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip enemyCollisionSound;
    
    public Vector3 startPosition;
    public Vector3 spawnStartPoint;
    public Vector3 spawnEndPoint;
    public bool shelled;

    protected float speedMod = 1.0f;
    protected RaycastHit2D groundHit;
    protected Vector2 raycastOrigin;
    protected Vector2 raycastDirection;
    protected float raycastMaxDistance;
    protected float adjustedWalkSpeed;
    public bool isJumping;
    protected bool isFalling;
    
    private float spawnPlatformTimer = 3.0f;
    private float currentJumpTimer;
    private float upwardGravity = 5.0f;
    private float downwardGravity = 12.0f;
    private PlayerSpawnPlatform spawnPlatform;
    private Inventory inventory; 
    private MasterController masterController;

    new private void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        spawnPlatform = spawnPlatformObject.GetComponent<PlayerSpawnPlatform>();
        collider = GetComponent<BoxCollider2D>();
        inventory = GetComponent<Inventory>();
        startPosition = transform.position;
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetPlayer(this);
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

                    animator.SetBool("walking", horizontalInput != 0 && grounded);
                    animator.SetBool("grounded", grounded);    
                }

                if(body.velocity.y < -0.01) {
                    body.gravityScale = downwardGravity;
                    isFalling = true;
                } else {
                    body.gravityScale = upwardGravity;
                    isFalling = false;
                }
            }else if(spawning){
                float step = spawnPlatform.speed * Time.deltaTime;

                Hold();
                transform.position = Vector2.MoveTowards(transform.position, spawnEndPoint, step);
            }

            animator.SetBool("falling",isFalling);
            animator.SetBool("dead",isDead);
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
            masterController.PlayerDied();
            animator.SetTrigger("respawn");
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
            if(inventory.currentItem != null && inventory.currentItem.flippedHorizontal && !inventory.currentItem.onUse) {
                inventory.currentItem.transform.localScale *= new Vector2(-1,1);
                inventory.currentItem.flippedHorizontal = false;
            }
        }else if(horizontalInput < -0.01f && flippedHorizontal){
            transform.localScale *= new Vector2(-1,1);
            flippedHorizontal = false;
            if(inventory.currentItem != null && !inventory.currentItem.flippedHorizontal && !inventory.currentItem.onUse) {
                inventory.currentItem.transform.localScale *= new Vector2(-1,1);
                inventory.currentItem.flippedHorizontal = true;
            }
        }
    }

    new protected void Jump()
    {
        if( grounded && spawned ) {
            masterController.soundController.PlaySound(jumpSound, 0.15f);
            isJumping = true;
            currentJumpTimer = maxJumpTime;
            body.velocity = Vector2.up * adjustedJumpSpeed;
        }else if(isJumping){
            if( currentJumpTimer > 0 ) {
                body.velocity = Vector2.up * (adjustedJumpSpeed) * 1.3f;
                currentJumpTimer -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        animator.SetTrigger("jump");

        grounded = false;

        if(!spawning){
            HideRespawnPlatform();
        }
    }

    new protected void OnCollisionEnter2D(Collision2D collision) 
    {
        base.OnCollisionEnter2D(collision);
        
        if( collision.gameObject.tag == "Enemies" ) {
            Enemy collidingEnemy = collision.gameObject.GetComponent<Enemy>();
            masterController.soundController.PlaySound(enemyCollisionSound, 0.4f);

            if(collidingEnemy.flippedVertical) {
                KillEnemy(collidingEnemy);
                animator.SetTrigger("kick");
            }else if(!collidingEnemy.flippedVertical && !collidingEnemy.isDead) {
                if(inventory.currentItem != null && inventory.currentItem.itemType == "shell") {
                    inventory.currentItem.UseEffect();
                } else {
                    Die();
                }
            }
        } else if ( collision.gameObject.tag == "Projectiles" ) {
            Die();
            
        }
    }

    public void KillEnemy(Enemy enemy)
    {
        enemy.Die(body);
        masterController.AddPoints(enemy.bounty);
    }

    public void Die()
    {   
        isDead = true;
        animator.SetTrigger("die");

        Hold();
        base.Jump();

        collider.enabled = false;
    }

    public void PlayerSpawn() 
    {
        StopCoroutine(SpawnPlatformCoroutine());
        
        inventory.LoseItem();
        gameObject.layer = 4;
        spawning = true;
        spawned = false;
        isDead = false;
        isFalling = false;
        transform.position = startPosition;
        collider.enabled = true;
        spawnPlatformObject.SetActive(true);
        spawnPlatform.coroutineStarted = false;
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
        if(!masterController.levelStarted) {
            masterController.levelStarted = true;
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
