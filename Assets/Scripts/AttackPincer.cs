using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPincer : UsableItem
{
    public override void OnUse()
    {
        collider.enabled = true;
        transform.localScale *= 2f;
        useCounter -= 1;
        StartCoroutine(UsageCoroutine());
    }

    //CHANGE FOR PARTICULAR ITEM
    protected override void LateUpdate()
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

}
