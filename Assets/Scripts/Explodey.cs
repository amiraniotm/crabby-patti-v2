using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodey : Enemy
{
    [SerializeField] private GameObject projectile;

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

    protected void Explode()
    {
        GameObject waveR = Instantiate(projectile, transform.position, Quaternion.identity);
        GameObject waveL = Instantiate(projectile, transform.position, Quaternion.identity);
        ElementWave waveScript = waveL.GetComponent<ElementWave>();
        waveScript.direction = "left";
        base.Vanish();
    }

    protected IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(changeTime / 4);

        Explode();
    }
}
