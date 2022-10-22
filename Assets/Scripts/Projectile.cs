using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    [SerializeField] public bool trippable;
    [SerializeField] public bool throwable;

    public Rigidbody2D body;
    public BoxCollider2D myCollider;
    public Renderer mainRenderer;
    public bool thrown, grounded;
    public Obstacle parentObstacle;
    public Vector3 originalScale;
    public bool growing;

    protected void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        originalScale = transform.localScale;
        mainRenderer = GetComponent<Renderer>();
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

    public void StartGrowing()
    {
        float gTime = UnityEngine.Random.Range(1, 3);

        StartCoroutine(GrowPillarCoroutine(gTime));
    }

    public void StartFading()
    {
        StartCoroutine(YFadeCoroutine());
    }

    protected IEnumerator GrowPillarCoroutine(float growthTime)
    {
        yield return new WaitForSeconds(growthTime);

        growing = true;
        StartCoroutine(StopPillarGrowthCoroutine(growthTime));
    }

    protected IEnumerator StopPillarGrowthCoroutine(float growthTime)
    {
        yield return new WaitForSeconds(growthTime);

        growing = false;
        gameObject.transform.localScale = originalScale;
        gameObject.SetActive(false);
    }

    protected IEnumerator YFadeCoroutine()
    {
        while(transform.localScale.y > 0) {
            float newYScale = transform.localScale.y - (Math.Abs(parentObstacle.moveSpeed) * Time.deltaTime);
            
            transform.localScale = new Vector3(transform.localScale.x,
                                                newYScale,
                                                transform.localScale.z);

            yield return 0;
        }

        gameObject.transform.localScale = originalScale;
        gameObject.SetActive(false);
    }
}
