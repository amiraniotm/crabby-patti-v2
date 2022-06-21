using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Enemies") {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();

            enemy.onGround = true;
        }
    }
}
