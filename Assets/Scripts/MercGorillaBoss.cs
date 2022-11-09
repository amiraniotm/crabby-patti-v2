using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercGorillaBoss : Boss
{
    [SerializeField] private float leapTime, attackTime, choiceTime;
    [SerializeField] private int attackChance, moveChance, bananaChance;
    [SerializeField] private ObjectPool projPool;
    [SerializeField] private PlayerMovement player;

    private bool doRage, raging, doAttack, attacking, doMove, moving, doLeap, leaping, doSwitch, switching, switchUp, switchDown, offCam, forceMove, attackSet;
    private GameObject currentProj;
    private Projectile projScript;
    private EdgeChecker edgeChecker;
    protected CameraMovement mainCamera;
    protected Rigidbody2D currentProjBody;
    protected Vector3 playerPos;
    protected float attackCount, swMult;

    new protected void Awake()
    {
        base.Awake();

        edgeChecker = GetComponent<EdgeChecker>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
    }

    private void Update()
    {
        offCam = IsOffCamera();
        forceMove = HasToMove();

        if(grounded && forceMove) {
            Move();
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
        body.velocity = new Vector2(walkSpeed, topJumpSpeed);

        int rand = Random.Range(0,100);

        if(rand < bananaChance && !forceMove) {
            SetProjectile("Banana");
            currentProjBody.AddForce(20.0f * Vector3.up, ForceMode2D.Impulse);
            projScript.thrown = true;
            projScript.SetCollider();
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
        currentProj = projPool.GetPooledObject(projName);
        currentProj.transform.SetParent(transform.parent);
        projScript = currentProj.GetComponent<Projectile>();
        currentProj.transform.position = new Vector3(mainRenderer.bounds.center.x, 
                                                    mainRenderer.bounds.max.y, 
                                                    currentProj.transform.position.z);
        currentProj.SetActive(true);
        projScript.thrown = false;
        currentProj.layer = 0;
        projScript.myCollider.enabled = false;
        currentProjBody = currentProj.GetComponent<Rigidbody2D>();
    }

    protected void Attack()
    {
        attacking = true;

        attackCount += Time.deltaTime;

        playerPos = player.transform.position;
        Vector3 posDif = playerPos - transform.position;

        if((posDif.x <= 0 && !flippedHorizontal) || (posDif.x > 0 && flippedHorizontal)) {
            flippedHorizontal = !flippedHorizontal;
            transform.localScale *= -1;
            walkSpeed *= -1;
            runSpeed *= -1;
        } 
        
        if(attackCount >= attackTime / 3 && !attackSet) {
            SetProjectile("Coconut");
            attackSet = true;
        } else if(attackCount >= attackTime) {
            attackSet = false;
            Vector3 adjForce = 2.0f * posDif;
            projScript.telegraphed = true;
            currentProjBody.AddForce(adjForce, ForceMode2D.Impulse);
            projScript.thrown = true;
            attackCount = 0;
            ResetMoveProps();
            Invoke("MoveOrAttack", choiceTime);
        }   
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
                Invoke("MoveOrAttack", choiceTime);
            }
        }
    }

    protected void MoveOrAttack()
    {
        ResetMoveProps();
        int rand = Random.Range(0,100);

        doAttack = rand < attackChance;
        doMove = !doAttack;
        
        if(doMove && !onMid) {
            MoveOrSwitch();
        }
    }

    protected void MoveOrSwitch()
    {
        int rand = Random.Range(0,100);

        doMove = rand < moveChance;
        doSwitch = !doMove;
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
        if((!spawned && spawning) || offCam || (onMid && !onTop)) {
            return true;
        }

        return false;
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
}
