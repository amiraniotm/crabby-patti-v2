using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplacementController : MonoBehaviour
{
    [SerializeField] GameObject[] stageObjects;
    [SerializeField] GameObject[] slowMoveObjects;
    [SerializeField] GameObject entryPoint;
    [SerializeField] PlayerMovement player;

    private MasterController masterController;
    private CameraMovement mainCamera;
    private FloatingPlatformController floatPlatController;

    public void SetDisplacementObjects(MasterController MCRef)
    {
        masterController = MCRef;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        floatPlatController = GameObject.FindGameObjectWithTag("FloatController").GetComponent<FloatingPlatformController>();
    }

    public void StartDisplacement()
    {
        masterController.scrollPhase = true;
        mainCamera.TriggerPan();
        entryPoint.transform.position = new Vector3(entryPoint.transform.position.x, 
                                                    entryPoint.transform.position.y + mainCamera.panningEndPoint.y, 
                                                    entryPoint.transform.position.z);

        floatPlatController.TriggerPlatforms();
    }

    private void Update()
    {
        if(masterController != null && masterController.scrollPhase) {
            MoveSlowObjects();
        }
    }

    public void MoveSlowObjects()
    {
        foreach(GameObject slowObj in slowMoveObjects) {
            if(!slowObj.activeSelf && slowObj.tag == "Walls") {
                slowObj.SetActive(true); 
            }

            Vector3 newObjPos = new Vector3(slowObj.transform.position.x,
                                            mainCamera.gameObject.transform.position.y,
                                            slowObj.transform.position.z);

            slowObj.transform.position = newObjPos;
        }
    }

    public void MoveStage()
    {
        int waterLayer = LayerMask.NameToLayer("Water");        
        player.SetLayer(waterLayer);
        
        foreach(GameObject stageObj in stageObjects) {
            Vector3 newObjPos = new Vector3(stageObj.transform.position.x, 
                                            stageObj.transform.position.y + mainCamera.panningEndPoint.y, 
                                            stageObj.transform.position.z);

            stageObj.transform.position = newObjPos;  
        }
    }

    public void EndDisplacement()
    {
        GameObject wallObject = GameObject.FindGameObjectWithTag("Walls");
        wallObject.SetActive(false);
    }
}
