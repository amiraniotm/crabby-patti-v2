using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaObstacle : Obstacle
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
}
