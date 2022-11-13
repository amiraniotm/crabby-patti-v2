using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private PauseController pauseController;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private List<string> itemNames = new List<string>();
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private AudioClip itemGotSound, enemyCollisionSound, itemAppearSound;
    [SerializeField] private ObjectPool itemPool;
    [SerializeField] private float spawnTime;

    private BoxCollider2D itemZone;
    private MasterController masterController;
    private Dictionary<string,int> itemWeights = new Dictionary<string, int>();
    private Dictionary<string,int> originalWeights = new Dictionary<string, int>();
    private Dictionary<string,int> spawnedItems = new Dictionary<string, int>();
    
    private int itemLimit = 5;
    private float originalSpawnTime;
    private List<string> originalItemNames = new List<string>();
    public GameObject currentItem;
    public Item currentItemScript;

    private void Start()
    {
        itemZone = GetComponent<BoxCollider2D>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetItemController(this);

        itemWeights.Add("ExtraLife", 80);
        itemWeights.Add("ExtraTime", 80);
        itemWeights.Add("AttackPincer", 50);
        itemWeights.Add("HardShell", 50);
        itemWeights.Add("BoomerangPincer", 40);

        originalSpawnTime = spawnTime;
        originalWeights = itemWeights;
        originalItemNames = itemNames;

        StartItems(5.0f);
    }

    private void SpawnItem()
    {    
        if(itemLimit > 0) {
            StartCoroutine(SetNewItemCoroutine());
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
        itemNames = originalItemNames;
        spawnTime = originalSpawnTime;
        itemWeights = originalWeights;
        spawnedItems = new Dictionary<string, int>();
        itemLimit = 5;
    }

    public void ItemGot()
    {
        masterController.soundController.PlaySound(itemGotSound, 0.2f);
    }

    public void EnemyHit()
    {
        masterController.soundController.PlaySound(enemyCollisionSound, 0.4f);
    }

    public void StartItems(float startTime)
    {
        InvokeRepeating("SpawnItem", startTime, spawnTime);
    }

    public void StopItems()
    {
        CancelInvoke();
    }

    public void SetItemsForBoss(int levelKey) 
    {
        spawnedItems = new Dictionary<string, int>();
        
        if(levelKey == 1){
            itemWeights.Remove("ExtraLife");
            itemWeights.Remove("ExtraTime");
            itemNames.Remove("ExtraLife");
            itemNames.Remove("ExtraTime");
            itemLimit = 50;
            spawnTime = 7.0f;
        }
    }

    private IEnumerator SetNewItemCoroutine()
    {
        int runCount = 0;
        bool itemSet = false;

        while(!itemSet) {
            int itemIndex = GetWeightedRandomItem();

            float randomX = Random.Range(itemZone.bounds.min.x, itemZone.bounds.max.x);
            float randomY = Random.Range(itemZone.bounds.min.y, itemZone.bounds.max.y);

            Vector2 newItemPos = new Vector2(randomX, randomY);

            bool isColliding = Physics.CheckSphere(newItemPos, 4f, platformMask);

            if(itemZone.bounds.Contains(newItemPos) && !isColliding && !tileManager.CheckForTile(newItemPos)) {
                currentItem = itemPool.GetPooledObject(itemNames[itemIndex]);

                if(currentItem != null) {
                    currentItem.SetActive(true);
                    currentItem.transform.position = newItemPos;
                    currentItemScript = currentItem.GetComponent<Item>();
                    currentItemScript.SetInitialPosition();
                    masterController.soundController.PlaySound(itemAppearSound, 0.3f);
                    itemSet = true;
                    itemLimit -= 1;

                    if(!spawnedItems.ContainsKey(currentItemScript.itemName)){
                        spawnedItems.Add(currentItemScript.itemName, 1);
                    } else {
                        spawnedItems[currentItemScript.itemName] += 1;
                    }
                }
            }

            runCount += 1;

            if(runCount > 1000) {
                itemSet = true;
            }

            yield return 0;
        }
    }
}
