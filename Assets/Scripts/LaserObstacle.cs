using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObstacle : Obstacle
{
    //CHANGE THIS
    protected override void Attack()
    {
        attacking = true;

        aimCount += Time.deltaTime;

        if(aimCount >= aimingTime) {
            attackCount += 1;
            aimCount = 0;
            ResetMoveProps();
        } 
    }

    protected override void Move()
    {
        Debug.Log("Move");
    }

    public override void AdjustPosToSide()
    {
        if(side == "left") {
            transform.position = new Vector3(transform.position.x + mainRenderer.bounds.size.x, transform.position.y + mainRenderer.bounds.size.y);
        } else if (side == "right") {
            transform.position = new Vector3(transform.position.x - mainRenderer.bounds.size.x, transform.position.y - mainRenderer.bounds.size.y);
        }
    }

    public override void DropAttack()
    {
        return;
    }

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player") {
            forceLeave = true;
            PlayerMovement player = otherCollider.gameObject.GetComponent<PlayerMovement>();

            player.Kick(); 
        }
    }

    protected override IEnumerator LeavingCoroutine()
    {        
        float leaveCount = 0.0f;
        forceLeave = false;
        doLeave = false;

        while (leaveCount < 1) {
            leaveCount += Time.deltaTime;

            yield return 0;
        }

        Leave();
    }
}
