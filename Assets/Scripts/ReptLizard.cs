using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReptLizard : Enemy
{
    private float runTime = 0.0f;
    private float tripTime = 4.0f;

    new private void Awake()
    {
        base.Awake();     

        speedMultiplier = 1.0f;
        canJump = false;
        unflipTime = 4.0f;
        spawningTime = 0.5f;
    }

    new protected void Update()
    {
        if(!spawning && !tripped && !flippedVertical) {
            runTime += 1 * Time.deltaTime;
        }

        if(runTime >= tripTime) {
            tripped = true;
        }

        if(grounded && tripped) {
            FlipVertical();
        }

        base.Update();
    }

    protected override void Spawn() 
    {
        runTime = 0.0f;
        tripped = false;

        base.Spawn();
    }

    public override void FlipVertical()
    {
        tripped = false;
        runTime = 0.0f;
        
        base.FlipVertical();
    }

    protected override void Unflip()
    {
        FlipHorizontal();

        base.Unflip();
    }


}
