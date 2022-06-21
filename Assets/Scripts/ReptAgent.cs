using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReptAgent : Enemy
{
    new private void Awake()
    {
        base.Awake();     

        speedMultiplier = 1.0f;
        canJump = false;
        unflipTime = 3.2f;
        getsMad = true;
        madTime = 6f;     
    }
    
}
