using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private PlayerMovement playerScript;
    public Item currentItem;

    private void Start()
    {
        playerScript = GetComponent<PlayerMovement>();
    }

    public void LoseItem()
    {
        if(playerScript.isDead && currentItem != null) {
            Destroy(currentItem.gameObject);
        }
    }
}
