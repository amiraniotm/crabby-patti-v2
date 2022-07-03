using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public string itemName;
    [SerializeField] public string itemType;
    [SerializeField] public int useCounter;
    [SerializeField] public float useTime;

    private ItemController itemController;
    private Vector2 originalScale;
    new private BoxCollider2D collider;

    public bool onInventory = false;
    public PlayerMovement player;
    
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        itemController = GameObject.FindGameObjectWithTag("ItemController").GetComponent<ItemController>();
        originalScale = transform.localScale;
    }

    private void LateUpdate()
    {
        if(onInventory) {
            Vector3 newPos = new Vector3(
                            player.gameObject.transform.position.x,
                            player.gameObject.transform.position.y,
                            player.gameObject.transform.position.z - 3
                            );

            transform.position = newPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player" && !onInventory) {
            itemController.ItemGot(this);
            collider.enabled = false;
            if(itemType == "consumable") {
                Vanish();
            }
        }else if(otherCollider.gameObject.tag == "Enemies" && onInventory) {
            Enemy enemyScript = otherCollider.gameObject.GetComponent<Enemy>();
            player.KillEnemy(enemyScript);
        }
    }

    public void onUse()
    {
        if(itemName == "pincer") {
            collider.enabled = true;
            transform.localScale *= 2f;
            useCounter -= 1;
            StartCoroutine(UsageCoroutine());
        }
    }
    
    private void CheckUses()
    {
        if(useCounter == 0) {
            Vanish();
        }
    }

    private void Vanish()
    {
        Destroy(gameObject);
    }

    private IEnumerator UsageCoroutine()
    {
        yield return new WaitForSeconds(useTime);

        transform.localScale = originalScale;
        collider.enabled = false;
        CheckUses();
    }

}
