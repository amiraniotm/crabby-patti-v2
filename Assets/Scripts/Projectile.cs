using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public bool trippable;
    public Rigidbody2D body;
    public BoxCollider2D myCollider;

    public bool thrown, grounded;

    protected void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    protected void Update()
    {
        if (thrown) {
            body.gravityScale = 5.0f;
        } else {
            body.gravityScale = 0.0f;
        }

        if(grounded) {
            Stop();
        }
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "FloatingPlatform") {
            HittableBlock platHit = other.gameObject.GetComponent<HittableBlock>(); 
            string collSide = platHit.DetectCollisionDirection(other);

            if(collSide == "upper") {
                grounded = true;
            }
        }
    }

    protected void Stop()
    {
        Vector2 newVel = new Vector2( 0.0f, body.velocity.y);

        body.velocity = newVel;
    }
}
