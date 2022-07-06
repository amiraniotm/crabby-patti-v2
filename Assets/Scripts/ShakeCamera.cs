using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private Vector3 initialPosition;
    private float shakeDuration = 0.3f;
    private float currentShakeTime = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;

    public bool triggerShake;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if(currentShakeTime > 0) {
            transform.position = initialPosition + (Random.insideUnitSphere * shakeMagnitude);
            currentShakeTime -= Time.deltaTime * dampingSpeed;
        } else {
            currentShakeTime = 0f;
            transform.position = initialPosition;
        }
    }

    public void TriggerShake()
    {
        currentShakeTime = shakeDuration;
    }
}
