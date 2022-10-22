using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LaserObstacle : Obstacle
{
    protected override void Attack()
    {
        attacking = true;

        aimCount += Time.deltaTime;

        if(aimCount >= aimingTime) {
            FireLaser();
            attackCount += 1;
            aimCount = 0;
            ResetMoveProps();
        } 
    }

    public override void AdjustPosToSide()
    {
        if(side == "left") {
            transform.position = new Vector3(transform.position.x + (1.5f * mainRenderer.bounds.size.x), transform.position.y + mainRenderer.bounds.size.y);
        } else if (side == "right") {
            transform.position = new Vector3(transform.position.x - (1.5f * mainRenderer.bounds.size.x), transform.position.y - mainRenderer.bounds.size.y);
        }
    }

    public override void DropAttack()
    {
        doAttack = false;
        aimCount = 0;
    }

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player") {
            DropAttack();
            PlayerMovement player = otherCollider.gameObject.GetComponent<PlayerMovement>();

            player.Kick(); 
        }
    }

    protected void FireLaser()
    {
        GameObject newLaser = projectilePool.GetPooledObject("Laser");
        Projectile laserProj = newLaser.GetComponent<Projectile>();
        newLaser.transform.SetParent(transform.parent);
        laserProj.parentObstacle = this;
        float newLaserX = 0.0f;

        if(side == "left") {
            newLaserX = transform.position.x + mainRenderer.bounds.size.x + (laserProj.mainRenderer.bounds.size.x / 2);
        } else {
            newLaserX = transform.position.x - mainRenderer.bounds.size.x - (laserProj.mainRenderer.bounds.size.x / 2);
        }

        Vector3 newLaserPos = new Vector3(newLaserX,
                                        transform.position.y,
                                        newLaser.transform.position.z);
        newLaser.transform.position = newLaserPos;
        newLaser.SetActive(true);

        laserProj.StartFading();
    }

    protected override IEnumerator LeavingCoroutine()
    {        
        forceLeave = false;
        doLeave = false;

        while (screenWrap.isVisible) {
            Vector3 newPos = Vector3.zero;

            if(side == "left") {
                newPos = new Vector3(transform.position.x - (Math.Abs(moveSpeed) * Time.deltaTime),
                                    transform.position.y,
                                    transform.position.z);
            } else {
                newPos = new Vector3(transform.position.x + (Math.Abs(moveSpeed) * Time.deltaTime),
                                    transform.position.y,
                                    transform.position.z);
            }
            
            transform.position = newPos;

            yield return 0;
        }

        Leave();
    }
}
