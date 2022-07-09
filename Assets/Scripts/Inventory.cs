using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private PlayerMovement playerScript;
    public UsableItem currentItem;

    private void Start()
    {
        playerScript = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C) && currentItem != null) {
            currentItem.OnUse();
        }
    }

    public void LoseItem()
    {
        if(playerScript.isDead && currentItem != null) {
            Destroy(currentItem.gameObject);
        }
    }
}
