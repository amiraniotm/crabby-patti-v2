using System.Collections;
using System.Collections.Generic;
using System;
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
    private bool onIce, tripped;
    private PlayerSpawnPlatform spawnPlatform;
    private Inventory inventory; 

    new private void Awake()
    {
        base.Awake();

        spawnPlatform = spawnPlatformObject.GetComponent<PlayerSpawnPlatform>();
        inventory = GetComponent<Inventory>();
        startPosition = transform.position;
    }

    public void Start()
    {        
        canJump = true;
        adjustedWalkSpeed = walkSpeed;
        
        spawnEndPoint = new Vector3(transform.position.x, transform.position.y - spawnPlatform.spawnGap, transform.position.z);

        PlayerSpawn();
    }

    private void Update()
    {      
        if(!pauseController.gamePaused && !tripped) {
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
        if(horizontalInput != 0 && !spawning && !spawned) {
            HideRespawnPlatform();
        }
        
        if(grounded && !onIce) {
            body.velocity = new Vector2(horizontalInput * adjustedWalkSpeed, body.velocity.y);
        } else if (!grounded) {
            body.velocity = new Vector2(2 * horizontalInput * adjustedWalkSpeed / 2.0f , body.velocity.y);    
        }
    }

    protected void CheckRespawnCondition()
    {
        if(!screenWrap.isVisible && isDead){
            masterController.PlayerDied();
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
        if( grounded && (spawned || masterController.scrollPhase) ) {
            masterController.soundController.PlaySound(jumpSound, 0.15f);
            isJumping = true;
            currentJumpTimer = maxJumpTime;
            body.velocity = Vector2.up * (adjustedJumpSpeed + (0.3f * Math.Abs(body.velocity.x)));
        }else if(isJumping){
            if( currentJumpTimer > 0 ) {
                body.velocity = Vector2.up * adjustedJumpSpeed * 1.3f;
                currentJumpTimer -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        animator.SetTrigger("jump");

        grounded = false;

        if(!spawning && !spawned){
            HideRespawnPlatform();
        }
    }

    new protected void OnCollisionEnter2D(Collision2D collision) 
    {
        base.OnCollisionEnter2D(collision);
        
        if(!spawning) {
            if( collision.gameObject.tag == "Enemies" ) {
                Enemy collidingEnemy = collision.gameObject.GetComponent<Enemy>();
                masterController.soundController.PlaySound(enemyCollisionSound, 0.4f);

                if(collidingEnemy.flippedVertical) {
                    KillEnemy(collidingEnemy);
                    Kick();
                }else if(!collidingEnemy.flippedVertical && !collidingEnemy.isDead) {
                    if(inventory.currentItem != null && inventory.currentItem.itemType == "shell") {
                        inventory.currentItem.UseEffect();
                    } else {
                        Die();
                    }
                }
            } else if ( collision.gameObject.tag == "Projectiles" ) {
                Projectile hitProj = collision.gameObject.GetComponent<Projectile>();

                if((!hitProj.grounded && hitProj.thrown && !hitProj.trippable) || !hitProj.throwable) {
                    Die(); 
                } else if (hitProj.grounded && hitProj.thrown && hitProj.trippable) {
                    hitProj.myCollider.enabled = false;
                    tripped = true;
                    StartCoroutine(UntripCoroutine());  
                } else {    
                    hitProj.grounded = false;
                    hitProj.body.velocity = body.velocity;
                    Kick();
                }
            } else if ( collision.gameObject.tag == "Waves" ) {
                Die(); 
            } 
        }
    }

    public void KillEnemy(Enemy enemy)
    {
        enemy.Die(body);
    }

    public void Die()
    {   
        isDead = true;
        animator.SetTrigger("die");
        inventory.LoseItem();

        Hold();
        base.Jump();

        collider.enabled = false;
    }

    public void PlayerSpawn() 
    {
        StopCoroutine(SpawnPlatformCoroutine());

        int waterLayer = LayerMask.NameToLayer("Water");  
        SetLayer(waterLayer);
        spawning = true;
        spawned = false;
        isDead = false;
        isFalling = false;
        transform.position = startPosition;
        collider.enabled = true;
        spawnPlatformObject.SetActive(true);
        spawnPlatform.respawning = true;
        spawnPlatform.coroutineStarted = false;
        animator.SetTrigger("respawn");
    }

    protected void HideRespawnPlatform()
    {
        spawned = true;
        spawnPlatform.respawning = false;
        spawnPlatform.currentRespawnTime = 0.0f;
        spawnPlatformObject.SetActive(false);
        spawnPlatformObject.transform.position = spawnPlatform.startPoint;
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
            speedMod = tileManager.GetTileSpeedMod(groundHit.point);

            if(speedMod > 0) {
                adjustedWalkSpeed = walkSpeed * speedMod;
                adjustedJumpSpeed = topJumpSpeed * speedMod;
                onIce = false;
            } else {
                onIce = true;
            }
        }
        
    }

    public void StartPlatformCoroutine()
    {
        if(!masterController.levelStarted) {
            masterController.levelStarted = true;
        }
        
        StartCoroutine(SpawnPlatformCoroutine());
    }

    private new void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "PlayArea") {
            int defaultLayer = LayerMask.NameToLayer("Default"); 
            SetLayer(defaultLayer);
        }
    }

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }

    public void Kick()
    {
        animator.SetTrigger("kick");
    }

    public IEnumerator SpawnPlatformCoroutine()
    {
        yield return new WaitForSeconds(spawnPlatformTimer);

        if(!spawned && !spawning) {
            HideRespawnPlatform();
        }
    }

    public IEnumerator UntripCoroutine()
    {
        float tripCount = 0.5f;

        while(tripCount > 0){
            tripCount -= Time.deltaTime;

            yield return 0;
        }

        tripped = false;
    }

}
