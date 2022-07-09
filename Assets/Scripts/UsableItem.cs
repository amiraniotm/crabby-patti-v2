using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : Item
{
    [SerializeField] public int useCounter;
    [SerializeField] public float useTime;

    public bool onInventory = false;
    public Inventory playerInventory;

    public abstract void OnUse();

    protected abstract void LateUpdate();

    protected override void Awake()
    {
        base.Awake();

        playerInventory = player.gameObject.GetComponent<Inventory>();
    }

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player" && !onInventory) {
            if(playerInventory.currentItem != null) {
                playerInventory.LoseItem();
            }

            playerInventory.currentItem = this;
            onInventory = true;  
            collider.enabled = false;
        } else if(otherCollider.gameObject.tag == "Enemies" && onInventory) {
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

    protected virtual IEnumerator UsageCoroutine()
    {
        yield return new WaitForSeconds(useTime);

        transform.localScale = originalScale;
        collider.enabled = false;
        CheckUses();
    }

    public override IEnumerator VanishCoroutine()
    {
        if(!onInventory){
            base.VanishCoroutine();
        }

        yield return 0;
    }
}
