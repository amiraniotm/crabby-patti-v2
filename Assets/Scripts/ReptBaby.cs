using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReptBaby : Enemy
{
    protected bool settingHover = false;
    protected bool readyToHover = false;

    new private void Awake()
    {
        base.Awake();     

        speedMultiplier = 1.0f;
        canJump = false;
        canHover = true;
        unflipTime = 2.6f;
    }

    new void Update()
    {
        base.Update();

        if(!flippedVertical && grounded && !settingHover){
            settingHover = true;
            StartCoroutine(SetHoverCoroutine());
        }

        if(readyToHover) {
            Hover();
        }

        if(grounded) {
            Hold();
        }

        animator.SetBool("flippedVertical",flippedVertical);
        animator.SetBool("grounded", grounded);
    }

    private void Hover()
    {
        StartCoroutine(EndHoverCoroutine());
        if(!flippedVertical){
            body.velocity = new Vector2(walkSpeed, topJumpSpeed);
            grounded = false;
        }
    }

    private IEnumerator SetHoverCoroutine()
    {
        yield return new WaitForSeconds(spawningTime);

        readyToHover = true;
    }

    private IEnumerator EndHoverCoroutine()
    {
        yield return new WaitForSeconds(spawningTime / 2);

        readyToHover = false;
        settingHover = false;
    }
}
