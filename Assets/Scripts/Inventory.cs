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
        if(Input.GetKeyDown(KeyCode.C) && currentItem != null && !currentItem.onUse && currentItem.hasEffect) {
            currentItem.UseEffect();
        }
    }

    public void LoseItem()
    {
        if(currentItem != null) {
            currentItem.gameObject.SetActive(false);
        }
    }
}
