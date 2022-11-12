using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    [SerializeField] public bool trippable;
    [SerializeField] public bool throwable;
    [SerializeField] private float vanishTime;

    private PlayerMovement player;
    public MercGorillaBoss boss;
    private float distToPlayer, distToBoss;

    public Rigidbody2D body;
    public BoxCollider2D myCollider;
    public Renderer mainRenderer;
    public bool thrown, grounded, growing, telegraphed, deactivated;
    public Obstacle parentObstacle;
    public Vector3 originalScale;

    protected void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        originalScale = transform.localScale;
        mainRenderer = GetComponent<Renderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    protected void Update()
    {
        if (throwable) {
            distToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if(boss) {
                distToBoss = Vector3.Distance(boss.transform.position, transform.position);
            }

            if(thrown) {
                body.gravityScale = 5.0f;

                if((!deactivated && telegraphed && distToPlayer < 5) || (deactivated && distToBoss < 5)) {
                    myCollider.enabled = true;
                }
            } else {
                Stop();
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
        if (throwable && (other.gameObject.tag == "FloatingPlatform" || other.gameObject.tag == "Platforms")) {
            HittableBlock platHit = other.gameObject.GetComponent<HittableBlock>(); 
            string collSide = platHit.DetectCollisionDirection(other);

            if(collSide == "upper") {
                grounded = true;
                deactivated = true;

                if(trippable) {
                    StartCoroutine(VanishCoroutine());
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Ground") {
            myCollider.enabled = true;
        }
    }

    protected void Stop()
    {
        Vector2 newVel = new Vector2( 0.0f, 0.0f );

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

    public void SetCollider()
    {
        StartCoroutine(ColliderOnCoroutine());
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

    protected IEnumerator VanishCoroutine()
    {
        yield return new WaitForSeconds(vanishTime);

        gameObject.SetActive(false);
    } 

    protected IEnumerator ColliderOnCoroutine()
    {
        yield return new WaitForSeconds(0.3f);

        myCollider.enabled = true;
    }  
}
