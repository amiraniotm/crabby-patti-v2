using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPincer : UsableItem
{
    public override void UseEffect()
    {
        onUse = true;
        animator.SetTrigger("attack");
        collider.enabled = true;
        useCounter -= 1;
        StartCoroutine(UsageCoroutine());
    }

    protected override void LateUpdate()
    {
        if(onInventory) {
            animator.SetBool("flipped", flippedHorizontal);
            
            Vector3 newPos = new Vector3(
                            player.gameObject.transform.position.x - (player.collider.size.x * player.transform.localScale.x),
                            player.gameObject.transform.position.y,
                            player.gameObject.transform.position.z - 3
                            );

            transform.position = newPos;
        }
    }

    protected override IEnumerator UsageCoroutine()
    {
        yield return new WaitForSeconds(useTime);

        onUse = false;
        collider.enabled = false;
        CheckUses();
    }

}
