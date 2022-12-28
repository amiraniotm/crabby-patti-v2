using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercGorillaBoss : Boss
{
    [SerializeField] private float leapTime, attackTime, choiceTime, tantrumTime;
    [SerializeField] private int attackChance, moveChance, bananaChance, monkeChance;
    [SerializeField] private ObjectPool projPool, monkePool;
    [SerializeField] private PlayerMovement player;
    [SerializeField] protected MapDisplacementController mapDisController;

    private bool doRage, raging, doAttack, attacking, doMove, moving, doLeap, leaping, doSwitch, switching, isHurt;
    private bool switchUp, switchDown, offCam, forceMove, attackSet, callTried;
    private GameObject currentProj;
    private Projectile projScript;
    private EdgeChecker edgeChecker;
    protected CameraMovement mainCamera;
    protected Rigidbody2D currentProjBody;
    protected Vector3 playerPos, vectToPlayer;
    protected float attackCount, swMult, adjustedChoiceTime;
    protected string lastAction;

    new protected void Awake()
    {
        base.Awake();

        edgeChecker = GetComponent<EdgeChecker>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        adjustedChoiceTime = choiceTime;
        adjustedWalkSpeed = walkSpeed;
        currentHitPoints = maxHitPoints;
    }

    private void Update()
    {
        offCam = IsOffCamera();
        forceMove = HasToMove();

        if(isHurt) {
            body.gravityScale = 3.0f;
        } else {
            body.gravityScale = 2.0f;
        }

        if(grounded && forceMove && !moving) {
            if((!flippedHorizontal && edgeChecker.frontEdgeHit) || (flippedHorizontal && edgeChecker.backEdgeHit)) {
                Move();
            } else {
                Leap();
            }
        }

        if(tripped) {
            Hold();
        }

        if(!spawned && spawning) {            
            if(screenWrap.isVisible) {
                spawned = true;
                spawning = false;
            }
        }

        if(grounded && edgeChecker.platChecked && !spawning) {
            if(edgeChecker.frontEdgeHit) {
                if(doMove && !moving) {
                    Move();
                } else if(doAttack) {
                    Attack();
                } else if (doSwitch) {
                    Switch();
                }
            } else if(!edgeChecker.frontEdgeHit && !leaping && onTop) {
                Hold();

                if(!doLeap) {
                    doLeap = true;
                    Invoke("Leap", leapTime);
                }
            }
        }
    }

    protected override void Move()
    {
        moving = true;
        body.velocity = new Vector2(adjustedWalkSpeed, topJumpSpeed);

        int rand = Random.Range(0,100);

        if((rand < bananaChance && !forceMove) || isHurt) {
            if(!isHurt) {
                SetProjectile("Banana");
                currentProjBody.AddForce(20.0f * Vector3.up, ForceMode2D.Impulse);
                projScript.SetCollider();
            } else {
                SetProjectile("choose");
                currentProjBody.AddForce(20.0f * Vector3.up, ForceMode2D.Impulse);
                projScript.deactivated = false;
                projScript.telegraphed = true;
            }
            
            projScript.thrown = true;
        } 
    }

    protected void Switch()
    {
        Jump();
        switching = true;
        SetSwitchDirection();
        
        collider.enabled = false;

        StartCoroutine(ColliderBackCoroutine());
    }

    protected void SetSwitchDirection()
    {
        if(!onGround && !onTop) {
            int rand = Random.Range(0,100);

            swMult = 2.5f;
            switchUp = rand < 50;
            switchDown = !switchUp;
        } else if(onGround) {
            swMult = 1.0f;
            switchUp = true;
            switchDown = false;
        } else if(onTop) {
            switchUp = false;
            switchDown = true;
        }
    }

    protected void SetProjectile(string projName)
    {
        if(projName == "choose") {
            int rand = Random.Range(0,100);

            if(rand > 50) {
                projName = "Coconut";
            } else {
                projName = "Banana";
            }
        }
        
        currentProj = projPool.GetPooledObject(projName);
        projScript = currentProj.GetComponent<Projectile>();
        currentProj.SetActive(true);
        projScript.deactivated = false;
        projScript.grounded = false;
        projScript.myCollider.enabled = false;
        currentProj.transform.SetParent(transform.parent);
        currentProj.transform.position = new Vector3(mainRenderer.bounds.center.x, 
                                                    mainRenderer.bounds.max.y, 
                                                    currentProj.transform.position.z);
        projScript.boss = this;
        projScript.thrown = false;
        currentProj.layer = 0;
        currentProjBody = currentProj.GetComponent<Rigidbody2D>();
    }

    protected void Attack()
    {
        attacking = true;
        attackCount += Time.deltaTime;

        if(!callTried) {
            AttackOrCall();
        }

        SetDistToPlayer();

        if(attackCount >= attackTime / 3 && !attackSet) {
            SetProjectile("Coconut");
            attackSet = true;
        } else if(attackCount >= attackTime) {
            ThrowProjectile();
            attackCount = 0;
            ResetMoveProps();
            Invoke("MoveOrAttack", adjustedChoiceTime);
        }   
    }

    protected void AttackOrCall()
    {
        int rand = Random.Range(0, 100);
        callTried = true;

        if(rand < monkeChance && (mapDisController.currentObstacles.Count < monkePool.objectsToPool)) {
            Jump();

            mapDisController.SetObstacle(gameObject);

            attackCount = 0;
        }
    }

    private void SetDistToPlayer()
    {
        playerPos = player.transform.position;
        vectToPlayer = playerPos - transform.position;

        if((vectToPlayer.x <= 0 && !flippedHorizontal) || (vectToPlayer.x > 0 && flippedHorizontal)) {
            FlipHorizontal();
        } 
    }

    private void ThrowProjectile()
    {
        attackSet = false;
        float forceMult = 3.0f;
        Vector3 adjForce = forceMult * vectToPlayer;

        if(adjForce.magnitude < 35) {
            forceMult = 5.0f;
            adjForce = forceMult * vectToPlayer;
        } 
        projScript.telegraphed = true;
        currentProjBody.AddForce(adjForce, ForceMode2D.Impulse);
        projScript.thrown = true;
    }

    protected void Leap()
    {
        doLeap = false;
        leaping = true;
        body.velocity = new Vector2(runSpeed, topJumpSpeed);
    }

    new protected void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if(collision.gameObject.tag == "Platforms") {
            Hold();
            if(!spawning && spawned && !forceMove) {
                ResetMoveProps();
                Invoke("MoveOrAttack", adjustedChoiceTime);
            } else if(forceMove) {
                moving = false;
            }
        } else if(collision.gameObject.tag == "Projectiles") {
            Projectile hitProj = collision.gameObject.GetComponent<Projectile>();

            if(hitProj.grounded) {
                collision.gameObject.SetActive(false);
            }    

            if(hitProj.trippable && hitProj.grounded) {
                tripped = true;
                StartCoroutine(UntripCoroutine());
            } else if(!hitProj.trippable && hitProj.deactivated && !hitProj.grounded) {
                TakeDamage();
            }
        } else if (collision.gameObject.tag == "Player") {
            Hold();
        }
    }

    protected void MoveOrAttack()
    {
        ResetMoveProps();
        int rand = Random.Range(0,100);

        doAttack = rand < attackChance;

        if(doAttack && lastAction == "attack") {
            doAttack = false;
        }
        
        doMove = !doAttack;

        if(doAttack) {
            lastAction = "attack";
        }
        
        if(doMove && !onMid) {
            MoveOrSwitch();
        }
    }

    protected void MoveOrSwitch()
    {
        int rand = Random.Range(0,100);

        doMove = rand < moveChance;

        if(doSwitch && lastAction == "switch") {
            doMove = true;
        }
        
        doSwitch = !doMove;

        if(doSwitch) {
            lastAction = "switch";
        } else {
            lastAction = "move";
        }
    }

    protected void ResetMoveProps()
    {
        doAttack = false;
        doMove = false;
        doSwitch = false;
        doRage = false;
        
        attacking = false;
        moving = false;
        leaping = false;
        switching = false;
        raging = false;

        callTried = false;
    }

    protected bool IsOffCamera()
    {
        Vector3 maxCamCorner = mainCamera.GetCurrentCorner("upperright");
        Vector3 minCamCorner = mainCamera.GetCurrentCorner("lowerleft");

        if(mainRenderer.bounds.max.x > maxCamCorner.x || mainRenderer.bounds.min.x < minCamCorner.x) {
            return true;
        }

        return false;
    }

    protected bool HasToMove()
    {
        if((!spawned && spawning) || offCam || (onMid && !onTop) || isHurt) {
            return true;
        }

        return false;
    }

    public override void TakeDamage()
    {
        StopAllCoroutines();
        Hold();
        DropAttack();
        doSwitch = false;
        currentHitPoints -= 1;

        if(currentHitPoints > 0) {
            StartCoroutine(TantrumCoroutine());
        } else {
            Jump();
            Die();
        }
    }

    private void Die()
    {
        collider.enabled = false;
    }

    private void DropAttack()
    {
        doAttack = false;

        if(currentProj != null) {
            currentProj.SetActive(false);
        }
    }

    private IEnumerator ColliderBackCoroutine()
    {       
        if(switchUp) {
            adjustedJumpSpeed = 5 * topJumpSpeed;

            float trPos = transform.position.y + (swMult * mainRenderer.bounds.size.y);

            while( transform.position.y < trPos ) {
                Jump();

                yield return 0;
            }

        } else if (switchDown) {    
            float trPos = transform.position.y - (1.5f * mainRenderer.bounds.size.y);

            while( transform.position.y > trPos ) {
                yield return 0;
            }
        }

        adjustedJumpSpeed = topJumpSpeed;
        collider.enabled = true;
        ResetMoveProps();
    }

    private IEnumerator TantrumCoroutine()
    {
        float startTime = 1.0f;

        while(startTime > 0) {
            Hold();
            startTime -= Time.deltaTime;

            yield return 0;
        }

        adjustedWalkSpeed *= 1.5f;
        adjustedChoiceTime = 0.0f;
        ResetMoveProps();
        isHurt = true;

        StartCoroutine(EndTantrumCoroutine());
    }

    private IEnumerator EndTantrumCoroutine()
    {
        yield return new WaitForSeconds(tantrumTime);

        adjustedChoiceTime = choiceTime;

        if(currentHitPoints <= 4) {
            adjustedWalkSpeed = 1.3f * walkSpeed;
            adjustedJumpSpeed = 1.3f * topJumpSpeed;
            choiceTime = 0.15f;
        } else if (currentHitPoints <= 2) {
            adjustedWalkSpeed = 2f * walkSpeed;
            adjustedJumpSpeed = 1.6f * topJumpSpeed;
            choiceTime = 0.1f;
        }

        isHurt = false;
    }
}
