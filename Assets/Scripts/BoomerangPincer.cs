using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BoomerangPincer : UsableItem
{
    private int speedMod;
    private float currentThrowTime;
    private float initialPlayerXVelocity;
    private float throwSpeed = 8.0f;
    private float holdTime = 0.15f;
    private bool comeBack = false;
    private bool hold = false;
    private Rigidbody2D body;

    protected override void Awake()
    {
        base.Awake();

        body = GetComponent<Rigidbody2D>();
    }

    public override void UseEffect()
    {
        currentThrowTime = useTime;
        onUse = true;
        if(Math.Abs(player.body.velocity.x) > 0.5f) {
            initialPlayerXVelocity = Math.Abs(player.body.velocity.x * 1.3f);
        } else {
            initialPlayerXVelocity = throwSpeed;
        }
        animator.SetBool("attacking", true);
        collider.enabled = true;
        usesLeft -= 1;
    }

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        base.OnTriggerEnter2D(otherCollider);

        if(otherCollider.gameObject.tag == "Player" && onUse && comeBack) {
            FinishUse();
        }
    }

    protected void Update()
    {
        if(onUse) {
            currentThrowTime -= Time.deltaTime;
            initialPlayerXVelocity -= 0.05f * Time.deltaTime;
            
            if(!comeBack && !hold) {
                body.velocity = new Vector2(speedMod * (initialPlayerXVelocity + throwSpeed), body.velocity.y);
            } else if(comeBack && !hold) { 
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.5f);
            } else if(hold) {
                body.velocity = Vector2.zero;
            }
        }
    }

    protected override void LateUpdate()
    {
        if(onInventory) {
            animator.SetBool("flipped", flippedHorizontal);

            if(flippedHorizontal) {
                speedMod = -1;
            } else {
                speedMod = 1;
            }
            
            if(!onUse && player != null){
                Vector3 newPos = new Vector3(
                            player.gameObject.transform.position.x - (player.collider.size.x * player.transform.localScale.x / 3),
                            player.gameObject.transform.position.y + (player.collider.size.y * player.transform.localScale.y / 3),
                            player.gameObject.transform.position.z - 3
                        );
                
                transform.position = newPos;
            } else {
                if((currentThrowTime < (useTime / 2)) && !hold && !comeBack) {
                    hold = true;
                    StartCoroutine(ComebackCoroutine());
                }
            }
            
        }
    }

    public override void FinishUse()
    {
        onUse = false;
        collider.enabled = false;
        comeBack = false;
        hold = false;
        animator.SetBool("attacking", false);
        CheckUses();
        StopAllCoroutines();
    }

    protected override IEnumerator UsageCoroutine()
    {
        yield return 0;
    }

    protected IEnumerator ComebackCoroutine()
    {
        yield return new WaitForSeconds(holdTime);

        comeBack = true;
        hold = false;
    }
}
