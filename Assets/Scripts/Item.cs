using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public string itemType;

    private ItemController itemController;
    
    private void Awake()
    {
        itemController = GameObject.FindGameObjectWithTag("ItemController").GetComponent<ItemController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player") {
            itemController.ItemGot(itemType);
            Vanish();
        }
    }

    private void Vanish()
    {
        Destroy(gameObject);
    }

}
