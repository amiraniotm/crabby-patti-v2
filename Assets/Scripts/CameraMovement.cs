using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] public float panSpeed;
    [SerializeField] private float panMultiplier;
    [SerializeField] private MasterController masterController;
    
    private Vector3 initialPosition;
    private float panDistance;
    private float shakeDuration = 0.3f;
    private float currentShakeTime = 0f;
    public float currentPanTime = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;
    private Camera cam;
    private Renderer backgroundRenderer;
    public float screenWidth;
    public float screenHeight;
    private GameObject backgroundImage;
    private GameObject nextBackground;
    private bool nextImageSet = false;
    public Vector3 panningEndPoint;
    private bool doPanUp;
    public float panAdjDist;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SetInitialShakePos();

        cam = GetComponent<Camera>();

        var screenBottomLeft = cam.ViewportToWorldPoint(new Vector2(0, 0));
        var screenTopRight = cam.ViewportToWorldPoint(new Vector2(1, 1));
        
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.y - screenBottomLeft.y;

        panningEndPoint = transform.position;
        panAdjDist = panMultiplier * screenHeight;
    }

    private void Update()
    {
        panDistance = transform.position.y - initialPosition.y;

        if(currentShakeTime != 0f) {
            Shake();
        }

        if(doPanUp) {
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
        doPanUp = true;
        panningEndPoint = new Vector3(transform.position.x, transform.position.y + panAdjDist, transform.position.z);
    }

    private void PanUp()
    {
        float step = panSpeed * Time.deltaTime;

        if(panningEndPoint != transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, panningEndPoint, step);
            if(panDistance > 0) {
                SetNextBackground();
                Vector3 newInitialPos = new Vector3(initialPosition.x, initialPosition.y + backgroundRenderer.bounds.size.y, initialPosition.z);
                initialPosition = newInitialPos;                
            } 
        } else {
            doPanUp = false;
            panningEndPoint = transform.position;
            masterController.EndScrollPhase();
        }
    }

    public void GetBackgroundImage()
    {
        backgroundImage = null;
        StartCoroutine(BackgroundImageCoroutine());
    }
    
    private void SetNextBackground()
    {
        nextBackground = Instantiate(backgroundImage,
                                    new Vector3(backgroundImage.transform.position.x, 
                                                backgroundImage.transform.position.y + backgroundRenderer.bounds.size.y,
                                                backgroundImage.transform.position.z ),
                                    Quaternion.identity);
        backgroundImage = nextBackground;
        nextImageSet = true;
    }

    public void SetInitialShakePos()
    {
        initialPosition = transform.position;
    }

    private IEnumerator BackgroundImageCoroutine() 
    {
        while(backgroundImage == null) {
            backgroundImage = GameObject.FindGameObjectWithTag("Background");
            yield return 0;
        }
        
        backgroundRenderer = backgroundImage.GetComponent<Renderer>();
    }
}
