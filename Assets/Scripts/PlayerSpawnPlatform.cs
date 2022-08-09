using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPlatform : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    
    public float speed = 0.5f;
    public bool coroutineStarted;
    public Vector3 endPoint;
    public float spawnGap = 14;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(player.masterController.practiceMode) {
            spawnGap = 18;
        }
        endPoint = new Vector3(transform.position.x, transform.position.y - spawnGap, transform.position.z);
    }

    private void Update()
    {
        if(transform.position.y > endPoint.y){
            float step = speed * Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, endPoint, step);
        }else{
            player.spawning = false;
            if(!coroutineStarted){
                player.StartPlatformCoroutine();
                coroutineStarted = true;
            }
        }
    }
}
