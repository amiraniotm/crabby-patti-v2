using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : Character
{
    [SerializeField] protected int maxHitPoints;
    
    protected int currentHitPoints;

    protected abstract void Move();

    public abstract void TakeDamage();
}   
