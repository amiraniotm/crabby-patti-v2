using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float panSpeed;
    [SerializeField] private float panDuration;
    
    private Vector3 initialPosition;
    private float shakeDuration = 0.3f;
    private float currentShakeTime = 0f;
    private float currentPanTime = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;
    private Camera cam;
    private float screenWidth;
    private float screenHeight;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        initialPosition = transform.position;

        cam = GetComponent<Camera>();

        var screenBottomLeft = cam.ViewportToWorldPoint(new Vector2(0, 0));
        var screenTopRight = cam.ViewportToWorldPoint(new Vector2(1, 1));
        
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.y - screenBottomLeft.y;
    }

    private void Update()
    {
        if(currentShakeTime != 0f) {
            Shake();
        }

        if(currentPanTime != 0f) {
            PanUp();
        }
    }

    public void TriggerShake()
    {
        currentShakeTime = shakeDuration;
    }

    private void Shake()
    {
        if(currentShakeTime > 0) {
            transform.position = initialPosition + (Random.insideUnitSphere * shakeMagnitude);
            currentShakeTime -= Time.deltaTime * dampingSpeed;
        } else {
            currentShakeTime = 0f;
            transform.position = initialPosition;
        }
    }

    public void TriggerPan()
    {
        currentPanTime = panDuration;
    }

    private void PanUp()
    {
        if(currentPanTime > 0) {
            Vector3 nextCameraPos = transform.position;
            nextCameraPos.y += panSpeed * Time.deltaTime;
            transform.position = nextCameraPos;
            currentPanTime -= Time.deltaTime;
        } else {
            currentPanTime = 0f;
        }
    }
}
