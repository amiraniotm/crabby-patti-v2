using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamey : Enemy
{
    new private void Awake()
    {
        base.Awake();    
        
        speedMultiplier = 1.0f;
        canJump = false;
        unflipTime = 3.2f;
        getsMad = true; 
        madTime = 5f;
        explodes = true;
    }
    

    new protected void Update()
    {
        if(triggerChange && body.velocity.y < 0){
            if(!mad) {
                change = true;
                animator.SetBool("change",change);
            } else {
                explode = true;
                animator.SetBool("explode", explode);
            }
            triggerChange = false;
            StartCoroutine(ChangeCoroutine());
        }

        if((change || explode) && !spawning){
            Hold();
            body.gravityScale = 0.0f;
        }

        if(grounded && readyToBlow) {
            explode = true;
            animator.SetBool("explode", explode);
            StartCoroutine(ExplodeCoroutine());
        }

        base.Update();
    }

    protected void Vanish()
    {
        enemyCounter.currentEnemies.Remove(gameObject);
        Destroy(gameObject);
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
            StartCoroutine(CalmDownRoutine());
        } else {
            Vanish();
        }
    }
}
