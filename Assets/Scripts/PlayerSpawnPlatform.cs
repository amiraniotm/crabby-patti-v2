using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPlatform : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] public float maxRespawnTime;
    
    public float speed = 0.5f;
    public bool coroutineStarted;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float spawnGap = 14;
    public float currentRespawnTime = 0.0f;
    public bool respawning;

    private void Awake()
    {
        startPoint = gameObject.transform.position;
        endPoint = new Vector3(transform.position.x, transform.position.y - spawnGap, transform.position.z);
    }

    private void Update()
    {
        if (player.masterController.scrollPhase) {
            endPoint = new Vector3(transform.position.x, transform.position.y - spawnGap, transform.position.z);
            
            if(currentRespawnTime > 0) {
                currentRespawnTime -= Time.deltaTime;
            }
        }

        if(transform.position.y > endPoint.y && currentRespawnTime >= 0){
            float step = speed * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, endPoint, step);
        } else {
            player.spawning = false;
            if(!coroutineStarted){
                player.StartPlatformCoroutine();
                coroutineStarted = true;
            }
        }
    }
}
