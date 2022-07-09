using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] public string itemName;

    protected ItemController itemController;
    protected Vector2 originalScale;
    protected float vanishTime = 4.0f;
    new protected BoxCollider2D collider;

    public PlayerMovement player;
    public Coroutine currentVanishCoroutine;
    
    protected virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        itemController = GameObject.FindGameObjectWithTag("ItemController").GetComponent<ItemController>();
        originalScale = transform.localScale;
    }

    protected abstract void OnTriggerEnter2D(Collider2D otherCollider);

    protected virtual void Vanish()
    {
        StopCoroutine(currentVanishCoroutine);
        Destroy(gameObject);
    }

    public virtual IEnumerator VanishCoroutine()
    {
        yield return new WaitForSeconds(vanishTime);

        Vanish();
    }

}
