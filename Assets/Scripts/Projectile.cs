using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public bool trippable;
    [SerializeField] public bool throwable;

    public Rigidbody2D body;
    public BoxCollider2D myCollider;
    public bool thrown, grounded;
    public float growSpeed;
    public Obstacle parentObstacle;
    public Vector3 originalScale;
    public bool growing;

    protected void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        originalScale = transform.localScale;
    }

    protected void Update()
    {
        if (throwable) {
            if(thrown) {
                body.gravityScale = 5.0f;
            } else {
                body.gravityScale = 0.0f;
            }
        }

        if(grounded) {
            Stop();
        }

        if(!throwable && growing) {
            Vector3 newScale = new Vector3(transform.localScale.x,
                                            transform.localScale.y + (10 * parentObstacle.moveSpeed * Time.deltaTime),
                                            transform.localScale.z);

            transform.localScale = newScale;
        }
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (throwable && other.gameObject.tag == "FloatingPlatform") {
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
