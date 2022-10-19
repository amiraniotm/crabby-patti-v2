using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    [SerializeField] protected float maxMoveTime, aimingTime;
    [SerializeField] protected int maxAttacks;
    [SerializeField] public bool isSided;
    [SerializeField] protected float attackChance, leaveChance;
    
    protected ObjectPool projectilePool;
    protected CameraMovement mainCamera;
    protected MapDisplacementController mapDisController;
    protected Renderer mainRenderer; 
    protected Vector3 playerPos;
    protected GameObject player;
    public string side = "left";
    protected float moveCount, aimCount;
    protected bool doAttack, attacking, doMove, moving, attackSet;
    protected int attackCount;
    public bool leaving, forceLeave, doLeave;

    protected virtual void Awake()
    {
        mainRenderer = GetComponent<Renderer>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        mapDisController = GameObject.FindGameObjectWithTag("DisplacementController").GetComponent<MapDisplacementController>();
        projectilePool = GameObject.FindGameObjectWithTag("ProjectilePool").GetComponent<ObjectPool>();
        player = GameObject.FindGameObjectWithTag("Player");
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
            doAttack = rand < attackChance;
            doMove = !doAttack;
        } else if(action == "leave") {
            doLeave = rand < leaveChance;
            doMove = !doLeave;            
        }   
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

    protected abstract void Move();

    protected abstract void OnTriggerEnter2D(Collider2D otherCollider);

    protected virtual void Leave()
    {
        ResetMoveProps();
        attackCount = 0;
        mapDisController.currentObstacles.Remove(this);
        gameObject.SetActive(false);
    }

    protected abstract IEnumerator LeavingCoroutine();
}
