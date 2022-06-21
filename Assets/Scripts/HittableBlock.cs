using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableBlock : MonoBehaviour
{
    public string DetectCollisionDirection(Collision2D collision) 
    {
        string side;

        ContactPoint2D wallHit = collision.contacts[0];

         if(wallHit.normal.y < 0.1 || wallHit.normal.y > -0.1) {
            if(wallHit.normal.y > 0){
                side = "upper";
            }else if(wallHit.normal.y < 0){
                side = "lower";
            }else{
                side = "???";
            }
        }else{
            side = "side";
        }
        
        return side;
    }
}
