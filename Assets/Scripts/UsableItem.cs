using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : Item
{
    [SerializeField] public int maxUses;
    [SerializeField] public float useTime;
    [SerializeField] public bool hasEffect;
    [SerializeField] public string itemType;

    public bool onInventory = false;
    public bool flippedHorizontal;
    public bool onUse = false;
    public int usesLeft;
    public Inventory playerInventory;
    public Vector3 originalScale;
    
    protected Animator animator;

    public abstract void UseEffect();

    protected abstract void LateUpdate();

    protected override void Awake()
    {
        base.Awake();

        playerInventory = player.gameObject.GetComponent<Inventory>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
        usesLeft = maxUses;
    }

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player" && !onInventory) {
            itemController.ItemGot();
            if(playerInventory.currentItem != null) {
                playerInventory.LoseItem();
            }

            playerInventory.currentItem = this;
            onInventory = true;  
            collider.enabled = false;
        } else if(otherCollider.gameObject.tag == "Enemies" && onInventory) {
            itemController.EnemyHit();
            Enemy enemyScript = otherCollider.gameObject.GetComponent<Enemy>();
            player.KillEnemy(enemyScript);
        } else if(otherCollider.gameObject.tag == "Bosses" && onInventory) {
            itemController.EnemyHit();
            Boss bossScript = otherCollider.gameObject.GetComponent<Boss>();
            bossScript.TakeDamage();
        }
    }

    public override void SetInitialPosition()
    {
        base.SetInitialPosition();

        transform.localScale = originalScale;
        onInventory = false;
        onUse = false;
        usesLeft = maxUses;
        collider.enabled = true;
    }

    protected virtual void CheckUses()
    {
        if(usesLeft == 0) {
            playerInventory.LoseItem();
            Vanish();
        }
    }

    public abstract void FinishUse();

    protected abstract IEnumerator UsageCoroutine();
}
