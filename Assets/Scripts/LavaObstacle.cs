using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LavaObstacle : Obstacle
{
    [SerializeField] protected int maxPillars;

    protected Vector3 originalScale;
    protected ScreenWrap screenWrap;
    protected int pillarNumber;

    protected override void Awake()
    {
        base.Awake();

        originalScale = transform.localScale;
        screenWrap = GetComponent<ScreenWrap>();
    }
    
    protected override void Attack()
    {
        attacking = true;

        aimCount += Time.deltaTime;

        playerPos = player.transform.position;
        
        if((aimCount >= aimingTime / 2) && !attackSet) {
            attackCount += 1;
            PillarAttack();
        } else if(aimCount >= aimingTime) {
            attackSet = false;
            aimCount = 0;
            ResetMoveProps();
        }   
    }

    protected override void Move()
    {
        moving = true;

        if((moveCount >= maxMoveTime / 2) && moveSpeed > 0) {
            moveSpeed *= -1;
        }

        Vector3 newScale = new Vector3(transform.localScale.x,
                                        transform.localScale.y + (moveSpeed * Time.deltaTime),
                                        transform.localScale.z);

        transform.localScale = newScale;

        moveCount += Time.deltaTime;

        if((moveCount >= maxMoveTime) || (transform.localScale.x < originalScale.x)) {
            transform.localScale = originalScale;
            moveSpeed = Math.Abs(moveSpeed);
            moveCount = 0;
            ResetMoveProps();
        }
    }

    public override void AdjustPosToSide()
    {
        transform.position = new Vector3(transform.position.x,
                                        transform.position.y - (mainRenderer.bounds.size.y / 4),
                                        transform.position.z);
    }

    public override void DropAttack()
    {
        return;
    }

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player") {
            PlayerMovement player = otherCollider.gameObject.GetComponent<PlayerMovement>();

            player.Die(); 
        }
    }

    protected override void Leave()
    {
        transform.localScale = originalScale;

        base.Leave();
    }

    protected void PillarAttack()
    {
        attackSet = true;

        GetPillarNumber();
        SetPillars();
    }

    protected void GetPillarNumber()
    {
        pillarNumber = UnityEngine.Random.Range(1, maxPillars);
    }

    protected void SetPillars()
    {
        for (int i = 0; i < pillarNumber; i++)
        {
            GameObject newPillar = projectilePool.GetPooledObject("LavaPillar");
            Projectile pillarProj = newPillar.GetComponent<Projectile>();
            newPillar.transform.SetParent(transform.parent);
            pillarProj.parentObstacle = this;
            float newPillarX = UnityEngine.Random.Range(-mainCamera.screenWidth / 2, mainCamera.screenWidth / 2);
            newPillar.transform.position = new Vector3(newPillarX, transform.position.y, transform.position.z);
            newPillar.SetActive(true);

            float gTime = UnityEngine.Random.Range(1, 3);
            StartCoroutine(GrowPillarCoroutine(pillarProj, gTime));
        }
    }

    protected override IEnumerator LeavingCoroutine()
    {        
        forceLeave = false;
        doLeave = false;

        while (screenWrap.isVisible) {
            Vector3 newPos = new Vector3(transform.position.x,
                                        transform.position.y - (Math.Abs(moveSpeed) * Time.deltaTime),
                                        transform.position.z);

            transform.position = newPos;

            yield return 0;
        }

        Leave();
    }

    protected IEnumerator GrowPillarCoroutine(Projectile pillar, float growthTime)
    {
        yield return new WaitForSeconds(growthTime);

        pillar.growing = true;
        
        StartCoroutine(StopPillarGrowth(pillar, growthTime));
    }

    protected IEnumerator StopPillarGrowth(Projectile pillar, float growthTime)
    {
        yield return new WaitForSeconds(growthTime);

        pillar.growing = false;
        pillar.gameObject.transform.localScale = pillar.originalScale;
        pillar.gameObject.SetActive(false);
    }
}
