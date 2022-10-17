using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeObstacle : Obstacle
{
    [SerializeField] private float throwMg;

    private ObjectPool projectilePool;
    private GameObject player;
    private GameObject currentProjectile;
    private Vector3 playerPos;
    private Projectile projScript;

    protected override void Awake()
    {
        base.Awake();

        projectilePool = GameObject.FindGameObjectWithTag("ProjectilePool").GetComponent<ObjectPool>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void Attack()
    {
        attacking = true;

        aimCount += Time.deltaTime;

        playerPos = player.transform.position;
        
        if (aimCount > (2 * aimingTime / 3) && !attackSet) {
            AttackOrSlack();
        } else if(aimCount >= aimingTime) {
            attackCount += 1;
            Vector3 adjForce = throwMg * (playerPos - transform.position); 
            Rigidbody2D projBody = currentProjectile.GetComponent<Rigidbody2D>();
            projScript.thrown = true;
            projBody.AddForce(adjForce, ForceMode2D.Impulse);
            attackSet = false;            
            aimCount = 0;
            ResetMoveProps();
        }     
    }

    private void AttackOrSlack()
    {
        int decider = Random.Range(0,100);

        if (decider <= 70) {
            SetAttack();
        } else {
            aimCount = 0;
            ResetMoveProps();
        }
    } 
    
    private void SetAttack()
    {
        ChooseProjectile();

        if( side == "left") {
            currentProjectile.transform.position = new Vector3(mainRenderer.bounds.max.x, 
                                                    mainRenderer.bounds.max.y, 
                                                    currentProjectile.transform.position.z);
        } else {
            currentProjectile.transform.position = new Vector3(mainRenderer.bounds.min.x, 
                                                    mainRenderer.bounds.max.y, 
                                                    currentProjectile.transform.position.z);
        }

        if(!projScript.myCollider.enabled) {
            projScript.myCollider.enabled = true;
        }
        projScript.thrown = false;
        currentProjectile.SetActive(true);
        currentProjectile.transform.SetParent(transform.parent);
        attackSet = true;
    }   

    public override void AdjustPosToSide()
    {
        if(side == "left") {
            transform.position = new Vector3(transform.position.x + mainRenderer.bounds.size.x, transform.position.y + mainRenderer.bounds.size.y);
        } else if (side == "right") {
            transform.position = new Vector3(transform.position.x - mainRenderer.bounds.size.x, transform.position.y - mainRenderer.bounds.size.y);
        }
    }

    public override void DropAttack()
    {
        if(currentProjectile != null && currentProjectile.activeSelf) {
            currentProjectile.SetActive(false);
        }
    }

    public void ChooseProjectile()
    {
        float decider = Random.Range(0,100);

        if(decider <= 50) {
            currentProjectile = projectilePool.GetPooledObject("Coconut");
        } else {
            currentProjectile = projectilePool.GetPooledObject("Banana");
        }

        projScript = currentProjectile.GetComponent<Projectile>();
    }
}
