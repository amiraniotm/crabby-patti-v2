using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float moveSpeed, maxMoveTime, aimingTime;
    [SerializeField] private int maxAttacks;

    private CameraMovement mainCamera;
    protected new BoxCollider2D collider;
    private string side = "left";
    private int attackCount;
    private float moveCount, aimCount;
    private bool doAttack, attacking, doMove, moving, doLeave, leaving;

    protected void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        
        SetSide();
    }

    protected void SetSide()
    {
        int randSide = Random.Range(0,1);

        if(randSide == 1) {
            side = "right";
        } else {
            side = "left";
        }
    }

    protected void Update()
    {
        if(!moving && !attacking && !leaving){ 
            if (attackCount < maxAttacks ) {
                MoveOrAction("attack");
            } else {
                MoveOrAction("leave");
            }
        } 
        
        if (doMove && (moveCount < maxMoveTime)) {
            Move();
        } else if (doAttack && (aimCount < aimingTime)) {
            Attack();
        } else if(doLeave) {
            leaving = true;
            CancelInvoke();
            StartCoroutine(LeavingCoroutine());
        } 
    }

    protected void MoveOrAction(string action)
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

    protected void ResetMoveProps()
    {
        doAttack = false;
        attacking = false;
        doMove = false;
        moving = false;
        doLeave = false;
        leaving = false;
    }

    protected void Attack()
    {
        attacking = true;

        aimCount += Time.deltaTime;

        if(aimCount >= aimingTime) {
            attackCount += 1;
            aimCount = 0;
            ResetMoveProps();
        } 
    }

    protected void Move()
    {
        moving = true; 

        float nextY = transform.position.y + (moveSpeed * Time.deltaTime);

        if(nextY + (collider.bounds.size.y / 2) > (mainCamera.gameObject.transform.position.y + mainCamera.screenHeight / 2) ||
            nextY - (collider.bounds.size.y / 2) < (mainCamera.gameObject.transform.position.y - mainCamera.screenHeight / 2)) {
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

    protected IEnumerator LeavingCoroutine()
    {        
        float leaveCount = 0.0f;

        while (leaveCount < 1) {
            Debug.Log("LEaving!");
            leaveCount += Time.deltaTime;

            yield return 0;
        }

        ResetMoveProps();
        gameObject.SetActive(false);
    }
}
