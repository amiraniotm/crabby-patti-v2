using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : Item
{
    [SerializeField] public int useCounter;
    [SerializeField] public float useTime;
    [SerializeField] public bool hasEffect;
    [SerializeField] public string itemType;

    public bool onInventory = false;
    public bool flippedHorizontal;
    public bool onUse = false;
    public Inventory playerInventory;
    
    protected Animator animator;

    public abstract void UseEffect();

    protected abstract void LateUpdate();

    protected override void Awake()
    {
        base.Awake();

        playerInventory = player.gameObject.GetComponent<Inventory>();
        animator = GetComponent<Animator>();
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
        }
    }

    protected virtual void CheckUses()
    {
        if(useCounter == 0) {
            Vanish();
        }
    }

    protected abstract IEnumerator UsageCoroutine();
}
