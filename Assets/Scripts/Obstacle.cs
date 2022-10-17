using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    [SerializeField] protected float moveSpeed, maxMoveTime, aimingTime;
    [SerializeField] protected int maxAttacks;
    [SerializeField] public bool isSided;

    protected CameraMovement mainCamera;
    protected MapDisplacementController mapDisController;
    protected Renderer mainRenderer; 
    public string side = "left";
    protected float moveCount, aimCount;
    protected bool doAttack, attacking, doMove, moving, doLeave, attackSet;
    public int attackCount;
    public bool leaving, forceLeave;

    protected virtual void Awake()
    {
        mainRenderer = GetComponent<Renderer>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        mapDisController = GameObject.FindGameObjectWithTag("DisplacementController").GetComponent<MapDisplacementController>();
    }

    public virtual void SetSide()
    {
        int randSide = Random.Range(0,100);

        if(randSide >= 50) {
            side = "right";
        } else if(randSide < 50) {
            side = "left";
        }
    }

    protected virtual void Update()
    {
        if(!moving && !attacking && !leaving){ 
            if (attackCount < maxAttacks ) {
                MoveOrAction("attack");
            } else {
                MoveOrAction("leave");
            }
        } 
        
        if (doMove && (moveCount < maxMoveTime) && !forceLeave) {
            Move();
        } else if ((doAttack && (aimCount < aimingTime) && !forceLeave)) {
            Attack();
        } else if(doLeave || forceLeave) {
            leaving = true;
            StartCoroutine(LeavingCoroutine());
        } 
    }

    protected virtual void MoveOrAction(string action)
    {
        ResetMoveProps();
        int rand = Random.Range(0,100);

        if(action == "attack") {
            doAttack = rand > 50;
        } else if(action == "leave") {
            doLeave = rand > 50;
        }
        
        doMove = rand <= 50;
    }

    protected virtual void ResetMoveProps()
    {
        doAttack = false;
        attacking = false;
        doMove = false;
        moving = false;
        doLeave = false;
        leaving = false;
        forceLeave = false;
    }

    protected abstract void Attack();

    public abstract void DropAttack();

    public abstract void AdjustPosToSide();

    protected virtual void Move()
    {
        moving = true; 

        float nextY = transform.position.y + (moveSpeed * Time.deltaTime);

        Vector3 lowerCorner = mainCamera.GetCurrentCorner("lowerleft");
        Vector3 upperCorner = mainCamera.GetCurrentCorner("upperright");

        if(nextY + (mainRenderer.bounds.size.y / 2) > (upperCorner.y) ||
            nextY - (mainRenderer.bounds.size.y / 2) < (lowerCorner.y)) {
                moveSpeed *= -1;
            }

        Vector3 newPos = new Vector3(transform.position.x,
                                    transform.position.y + (moveSpeed * Time.deltaTime),
                                    transform.position.z);

        transform.position = newPos;

        moveCount += Time.deltaTime;

        if(moveCount >= maxMoveTime) {
            moveCount = 0;
            ResetMoveProps();
        }
    }

    protected virtual IEnumerator LeavingCoroutine()
    {        
        float leaveCount = 0.0f;
        forceLeave = false;
        doLeave = false;

        while (leaveCount < 1) {
            leaveCount += Time.deltaTime;

            yield return 0;
        }

        ResetMoveProps();
        attackCount = 0;
        mapDisController.currentObstacles.Remove(this);
        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player") {
            forceLeave = true;
            PlayerMovement player = otherCollider.gameObject.GetComponent<PlayerMovement>();

            player.Kick(); 
        }
    }
}
