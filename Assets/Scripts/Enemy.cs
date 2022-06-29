using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] public string type;
    [SerializeField] public int bounty;
    
    protected Vector2 initialShakePosition;
    public SpawnPoint spawnPoint;
    public GameObject projectile;
    private Coroutine lastUnflipCoroutine;
    private Coroutine lastShakeCoroutine;
    new private CapsuleCollider2D collider; 
    protected bool readyToBlow = false;
    protected bool change = false;
    protected bool getsMad = false;
    protected bool explodes = false;
    protected bool explode = false;
    protected bool mad = false;
    protected bool canHover = false;
    protected bool triggerChange = false;
    protected bool shaking;
    protected bool readyToSpawn;
    protected float spawningTime = 1f;
    protected float changeTime = 1.2f;
    protected float unflipTime;
    protected float madTime;
    protected float speedMultiplier;
	protected float shakeMagnitude = 0.05f;
    
    new protected void Awake()
    {
        base.Awake();

        collider = GetComponent<CapsuleCollider2D>();
    }

    protected void Start()
    {      
        enemyCounter = GameObject.FindGameObjectWithTag("EnemyCounter").GetComponent<EnemyCounter>();
        
        Spawn();
    }

    protected void Spawn()
    {
        spawnPoint.spawning = true;
        readyToSpawn = false;

        if(!enemyCounter.currentEnemies.Contains(gameObject)){
            enemyCounter.currentEnemies.Add(gameObject);
        }

        if(getsMad && mad){
            mad = false;
            animator.SetBool("mad",mad);
        }

        if(transform.position.x > 0 && walkSpeed > 0){
            FlipHorizontal();
        }

        spawning = true;
        StartCoroutine(SpawnCoroutine());
    }

    protected void Update()
    {
        if(spawning) {
            body.gravityScale = 0.0f;
            body.velocity = new Vector2(walkSpeed / 3, 0);
        } else if(!spawning && body.velocity.y < -0.1) {
            body.gravityScale = 3.5f;
        } else if(!spawning) {
            body.gravityScale = 1.0f;
        } else if (isDead) {
            body.gravityScale = 10.0f;
        }

        if(!screenWrapScript.isVisible && onGround){
            Respawn();
        }

        if(readyToSpawn){
            Hold();
            if(!spawnPoint.spawning && spawnPoint.readyToSpawn){
                Spawn();
            }
        }

        if(!flippedVertical && grounded && !canHover && !isDead){
            if(!mad){
                Walk();
            } else {
                Run();
            }
        }

        animator.SetBool("flippedVertical",flippedVertical);
        animator.SetBool("grounded", grounded);
    }

    protected void Walk()
    {
        if(grounded){
            body.velocity = new Vector2(walkSpeed * speedMultiplier, body.velocity.y);
        }
    }
    
    protected void Run()
    {
        if(grounded){
            body.velocity = new Vector2(runSpeed, body.velocity.y);
        }
    }

    protected void SlowDown()
    {
        body.velocity = new Vector2(body.velocity.x / 2, body.velocity.y);
    }

    protected void Respawn()
    {
        transform.position = originPosition;
        onGround = false;
        
        Hold();

        readyToSpawn = true;
    }
    
    new protected void OnCollisionExit2D(Collision2D collision) 
    {        
        base.OnCollisionExit2D(collision);
        
        if(collision.gameObject.tag == "Platforms"){
            SlowDown();
        }

    }

    protected void FlipHorizontal()
    {
        walkSpeed *= -1;
        runSpeed *= -1;
        transform.localScale *= new Vector2(-1,1);
    }   

    public void FlipVertical()
    {
        if(!isDead){
            Hold();
            Jump();

            if(!getsMad || (!explodes && mad)){
                if(!flippedVertical) {
                    flippedVertical = true;
                    lastUnflipCoroutine = StartCoroutine(UnflipCoroutine());
                    lastShakeCoroutine = StartCoroutine(ShakeCoroutine());
                } else {
                    Unflip();
                }
            } else if(getsMad && !explodes) {
                mad = true;
                animator.SetBool("mad",mad);
                StartCoroutine(CalmDownRoutine());
            } else if(explodes){
                triggerChange = true;
            }
        }
    }

    protected void StartShaking()
	{
        initialShakePosition = transform.position;

		InvokeRepeating ("Shake", 0f, 0.3f);
	}

    protected void Shake()
	{
        if(!isDead) {
            if( transform.position.x >= initialShakePosition.x ){
                transform.position = new Vector2(initialShakePosition.x - shakeMagnitude, initialShakePosition.y);
            }else if( transform.position.x < initialShakePosition.x ){
                transform.position = new Vector2(initialShakePosition.x + shakeMagnitude, initialShakePosition.y);
            }
        }
	}

	protected void StopShaking()
	{
		CancelInvoke ("Shake");
		transform.position = initialShakePosition;
        shaking = false;
	}

    private void Unflip()
    {
        StopCoroutine(lastUnflipCoroutine);
        StopCoroutine(lastShakeCoroutine);
        if(shaking){
            StopShaking();
        }
        flippedVertical = false;

        if(getsMad){
            mad = false;
            animator.SetBool("mad", mad);
        }
    }

    public void Die(Rigidbody2D playerBody)
    {
        if(shaking){
            StopShaking();
        }

        isDead = true;
        collider.enabled = false;
        body.velocity = new Vector2(playerBody.velocity.x * 2, playerBody.velocity.y);
    }

    protected void Explode()
    {
        GameObject flameR = Instantiate(projectile, transform.position, Quaternion.identity);
        GameObject flameL = Instantiate(projectile, transform.position, Quaternion.identity);
        FlameWave flameScript = flameL.GetComponent<FlameWave>();
        flameScript.direction = "left";
        enemyCounter.currentEnemies.Remove(gameObject);
        Destroy(gameObject);
        enemyCounter.EnemyDied();
    }

    private IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(spawningTime);

        body.gravityScale = 1.0f;
        spawning = false;
        spawnPoint.spawning = false;
    }

    private IEnumerator ShakeCoroutine()
    {
        float shakeStart = 2 * unflipTime / 3;
        
        yield return new WaitForSeconds(shakeStart);

        if(!shaking && !isDead && flippedVertical){
            StartShaking();
            shaking = true;
        }

    }

    private IEnumerator UnflipCoroutine()
    {
        yield return new WaitForSeconds(unflipTime);

        if(flippedVertical && !isDead){
            Jump();
            Unflip();
        }
    }

    protected IEnumerator CalmDownRoutine()
    {
        yield return new WaitForSeconds(madTime);

        if(!flippedVertical && !isDead){
            if(!explodes){
                mad = false;
                animator.SetBool("mad",mad);
            }else{
                Hold();
                readyToBlow = true;
                isDead = true;
            }
        }
    }

    protected IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(changeTime);

        Explode();
    }
}
