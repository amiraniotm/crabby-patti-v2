using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    [SerializeField] public bool trippable;
    [SerializeField] public bool throwable;

    private PlayerMovement player;
    private float distToPlayer;

    public Rigidbody2D body;
    public BoxCollider2D myCollider;
    public Renderer mainRenderer;
    public bool thrown, grounded, growing, telegraphed;
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

            if(thrown) {
                body.gravityScale = 5.0f;
                Debug.Log(distToPlayer);
                if(telegraphed && distToPlayer < 0.5f) {
                    Debug.Log("ACTIVATE");
                    myCollider.enabled = true;
                }
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
        if (throwable && (other.gameObject.tag == "FloatingPlatform" || other.gameObject.tag == "Platforms")) {
            HittableBlock platHit = other.gameObject.GetComponent<HittableBlock>(); 
            string collSide = platHit.DetectCollisionDirection(other);

            if(collSide == "upper") {
                grounded = true;

                if(trippable) {
                    StartCoroutine(VanishCoroutine());
                }
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
        yield return new WaitForSeconds(5);

        gameObject.SetActive(false);
    } 

    protected IEnumerator ColliderOnCoroutine()
    {
        yield return new WaitForSeconds(0.3f);

        myCollider.enabled = true;
    }  
}
