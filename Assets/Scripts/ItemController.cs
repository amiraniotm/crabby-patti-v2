using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] LevelDisplay levelDisplay;
    [SerializeField] PauseController pauseController;
    [SerializeField] TileManager tileManager;
    [SerializeField] LayerMask platformMask;
    [SerializeField] GameObject[] itemPrefabs;
    [SerializeField] Inventory playerInventory;

    private BoxCollider2D itemZone;
    private Dictionary<string,int> spawnedItems = new Dictionary<string, int>();
    private Dictionary<string,int> itemWeights = new Dictionary<string, int>();
    private float spawnTime = 3.0f;
    private int itemLimit = 10;
    
    public GameObject currentItem;
    public Item currentItemScript;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        itemZone = GetComponent<BoxCollider2D>();

        //itemWeights.Add("life", 90);
        //itemWeights.Add("time", 90);
        //itemWeights.Add("attack_pincer", 60);
        itemWeights.Add("boomerang_pincer", 60);
        //itemWeights.Add("hard_shell", 30);

        InvokeRepeating("SpawnItem", 1.0f, spawnTime);
    }

    private void SpawnItem()
    {    
        if(itemLimit > 0) {
            bool itemSet = false;

            int itemIndex = GetWeightedRandomItem();

            while(!itemSet){
                float randomX = Random.Range(itemZone.bounds.min.x, itemZone.bounds.max.x);
                float randomY = Random.Range(itemZone.bounds.min.y, itemZone.bounds.max.y);

                Vector2 newItemPos = new Vector2(randomX, randomY);

                bool isColliding = Physics.CheckSphere(newItemPos, 4f, platformMask);

                if(itemZone.bounds.Contains(newItemPos) && !isColliding && !tileManager.CheckForTile(newItemPos)) {
                    currentItem = Instantiate(itemPrefabs[itemIndex], newItemPos,Quaternion.identity);
                    currentItemScript = currentItem.GetComponent<Item>();
                    itemSet = true;
                    itemLimit -= 1;

                    if(!spawnedItems.ContainsKey(currentItemScript.itemName)){
                        spawnedItems.Add(currentItemScript.itemName, 1);
                    } else {
                        spawnedItems[currentItemScript.itemName] += 1;
                    }

                    StartCoroutine(currentItemScript.VanishCoroutine());
                }
            }
        }  
    }

    public int GetWeightedRandomItem()
    {    
        List<float> weightList = new List<float>();

        foreach(KeyValuePair<string, int> item in spawnedItems)
        {
            if(spawnedItems[item.Key] !=0) {
                float weight = itemWeights[item.Key] / spawnedItems[item.Key]; 
                weightList.Add(weight);
            }
        }
        
        float[] adjustedWeights = weightList.ToArray();
        int[] weights = new int[itemWeights.Count];
        itemWeights.Values.CopyTo(weights, 0);

        int randomWeight = UnityEngine.Random.Range(0, weights.Sum());

        while( randomWeight >= 0) {
            for (int i = 0; i < weights.Length; ++i)
            {
                randomWeight -= weights[i];

                if (randomWeight < 0)
                {
                    return i;
                }
            }        
        }

        return 0;
    }

    public void FlushItems()
    {
        spawnedItems = new Dictionary<string, int>();
        playerInventory.LoseItem();
        itemLimit = 5;
    }
}
