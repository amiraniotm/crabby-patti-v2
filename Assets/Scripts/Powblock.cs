using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powblock : HittableBlock
{
    [SerializeField] public List<Sprite> spriteList = new List<Sprite>();
    [SerializeField] private AudioClip powSound;
    [SerializeField] private ShakeCamera mainCamera;
    
    private SoundController soundController;
    private EnemyCounter enemyCounter;
    public int powCount = 2;
    private PlayerMovement player;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        enemyCounter = GameObject.FindGameObjectWithTag("EnemyCounter").GetComponent<EnemyCounter>();
    }

    private void Update()
    {
        if(powCount < 0) {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidingObject = collision.gameObject;

        if(collidingObject.tag == "Player" && player.spawned){

            string collisionSide = DetectCollisionDirection(collision);

            if(collisionSide == "upper" && powCount >= 0) {
                enemyCounter.FlipAll();
                if(!player.masterController.practiceMode){
                    powCount -= 1;
                }
                mainCamera.TriggerShake();
                soundController.PlaySound(powSound, 0.4f);
                if(powCount >= 0){
                    ChangeSprite();
                }
            }
        }

    }

    public void ChangeSprite()
    {
        spriteRenderer.sprite = spriteList[powCount]; 
    }
}
