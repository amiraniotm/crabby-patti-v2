using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] public string type;
    [SerializeField] public int bounty;
    
    private Coroutine lastUnflipCoroutine;
    private Coroutine lastShakeCoroutine;
    
    protected Vector2 initialShakePosition;
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
    
    public SpawnPoint spawnPoint;
    
    public void Start()
    {      
        enemyCounter = GameObject.FindGameObjectWithTag("EnemyCounter").GetComponent<EnemyCounter>();
        
        if(!spawned) {
            Spawn();
        }
    }

    protected virtual void Spawn()
    {
        readyToSpawn = false;
        collider.enabled = true;
        isDead = false;
        flippedVertical = false;

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
        if(!spawned) {
            spawnPoint.spawning = true;
            StartCoroutine(SpawnCoroutine());
        }
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

        if(!screenWrap.isVisible && onGround){
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

    public virtual void FlipVertical()
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
            } else if(getsMad) {
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

    protected virtual void Unflip()
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

    public void Vanish()
    {
        enemyCounter.currentEnemies.Remove(gameObject);
        gameObject.SetActive(false);
        enemyCounter.EnemyDied();
        enemyCounter.masterController.AddPoints(bounty);
    }

    protected void AdjustCollider()
    {
        Vector3 newSize = new Vector3 ( mainRenderer.bounds.size.x / Math.Abs(transform.localScale.x),
                                        mainRenderer.bounds.size.y / Math.Abs(transform.localScale.y),
                                        mainRenderer.bounds.size.z / Math.Abs(transform.localScale.z) );

        collider.size = newSize;
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

    protected IEnumerator CalmDownCoroutine()
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

    protected IEnumerator ChangeCoroutine()
    {
        yield return new WaitForSeconds(changeTime);

        if(!mad){
            mad = true;
            triggerChange = false;
            change = false;
            body.gravityScale = 1.0f;
            animator.SetBool("mad",mad);
            animator.SetBool("change",change);
            AdjustCollider();
            StartCoroutine(CalmDownCoroutine());
        } else if(mad && explodes) {
            Vanish();
        }
    }
}
