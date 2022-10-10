using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardShell : UsableItem
{
    public override void UseEffect()
    {
        onUse = true;
        animator.SetTrigger("hit");
        StartCoroutine(UsageCoroutine());
    }

    protected override void LateUpdate()
    {
        if(onInventory) {
            Vector3 newPos = transform.position;

            if(!onUse){
                newPos = new Vector3(
                            player.gameObject.transform.position.x,
                            player.gameObject.transform.position.y + (player.collider.size.y / 2),
                            player.gameObject.transform.position.z - 3
                            );
            }else{
                newPos = transform.position + new Vector3(0,0.1f,0);
            }

            transform.position = newPos;
        }
    }

    protected override IEnumerator UsageCoroutine()
    {
        yield return new WaitForSeconds(useTime);

        gameObject.SetActive(false);
    }
}
