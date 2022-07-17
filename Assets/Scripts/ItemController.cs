using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private LevelDisplay levelDisplay;
    [SerializeField] private PauseController pauseController;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private AudioClip itemGotSound;
    [SerializeField] private AudioClip enemyCollisionSound;
    [SerializeField] private AudioClip itemAppearSound;

    private BoxCollider2D itemZone;
    private Dictionary<string,int> spawnedItems = new Dictionary<string, int>();
    private Dictionary<string,int> itemWeights = new Dictionary<string, int>();
    private MasterController masterController;
    private float spawnTime = 12.0f;
    
    public int itemLimit = 5;
    public GameObject currentItem;
    public Item currentItemScript;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        itemZone = GetComponent<BoxCollider2D>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetItemController(this);

        itemWeights.Add("life", 80);
        itemWeights.Add("time", 80);
        itemWeights.Add("attack_pincer", 50);
        itemWeights.Add("hard_shell", 50);
        itemWeights.Add("boomerang_pincer", 40);

        InvokeRepeating("SpawnItem", 5.0f, spawnTime);
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
                    masterController.soundController.PlaySound(itemAppearSound, 0.3f);
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

    public void ItemGot()
    {
        masterController.soundController.PlaySound(itemGotSound, 0.2f);
    }

    public void EnemyHit()
    {
        masterController.soundController.PlaySound(enemyCollisionSound, 0.4f);
    }
}
