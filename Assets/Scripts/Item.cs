using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] public string itemName;

    protected ItemController itemController;
    protected Vector3 initialPosition;
    protected float vanishTime = 5.0f;
    new protected BoxCollider2D collider;
    protected MasterController masterController; 

    public PlayerMovement player;
    
    protected virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        itemController = GameObject.FindGameObjectWithTag("ItemController").GetComponent<ItemController>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        SetInitialPosition();
    }

    protected abstract void OnTriggerEnter2D(Collider2D otherCollider);

    protected virtual void Vanish()
    {
        gameObject.SetActive(false);
    }

    public virtual void SetInitialPosition()
    {
        initialPosition = transform.position;
    }

    public virtual IEnumerator VanishCoroutine()
    {
        yield return new WaitForSeconds(vanishTime);

        if(transform.position == initialPosition) {
            Vanish();
        }
    }

}
