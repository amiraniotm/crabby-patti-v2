using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabPlatform : MonoBehaviour
{
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player"){
            
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

            player.grounded = true;     
        }
    }
}
