using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crabcatcher : Enemy
{
    new private void Awake()
    {
        base.Awake();     

        if(type == "CrabCatcher"){
            speedMultiplier = 1.0f;
            animator.SetInteger("type",0);
            canJump = false;
            unflipTime = 4.0f;
        }else if(type == "CrabCatcherPlus"){
            speedMultiplier = 1.3f;
            animator.SetInteger("type",1);
            canJump = false;
            unflipTime = 3.7f;
        }
    }    
}
