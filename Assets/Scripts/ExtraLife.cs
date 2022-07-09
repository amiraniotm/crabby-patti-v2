using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : ConsumableItem
{
    [SerializeField] protected int lifeAmount;

    protected override void ApplyEffect()
    {
        player.levelDisplay.livesCount += lifeAmount;
    }
}
