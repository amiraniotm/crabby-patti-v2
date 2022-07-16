using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //MANAGE PROPS THROUGH SCRIPTABLE OBJECT LATER!
    private float activeTime = 2.0f;
    public string itemType = "life";
    private MasterController masterController; 
    
    void Start()
    {
        StartCoroutine(VanishCoroutine());

        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player") {
            masterController.livesCount += 1;
            Vanish();
        }
    }

    private void Vanish()
    {
        Destroy(gameObject);
    }

    private IEnumerator VanishCoroutine()
    {
        yield return new WaitForSeconds(activeTime);

        Vanish();
    }
}
