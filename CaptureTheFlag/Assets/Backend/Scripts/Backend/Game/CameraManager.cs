using System.Collections;
using System.Collections.Generic;
using RTS_Cam;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Camera mainCamera;

    public RTS_Camera mapSpectateComponent;

    public Vector3 mapViewPosition;
    public Vector3 mapViewRotation;

    public Vector3 spectateOffset;
    public Vector3 spectateRotation;

    void Awake()
    {
        mapViewPosition = transform.position;
        mapViewRotation = transform.localEulerAngles;
    }

    public int index = -1;

    void Update()
    {
        SetPosition();

        if (Input.GetMouseButtonDown(0))
        {
            if (mapSpectateComponent.enabled)
            {
                mapSpectateComponent.enabled = false;
                mapViewPosition = mainCamera.transform.position;
                mapViewRotation = mainCamera.transform.localEulerAngles;
            }
            else 
                Rotate();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!mapSpectateComponent.enabled)
            {
                mainCamera.transform.position = mapViewPosition;
                mainCamera.transform.localEulerAngles = mapViewRotation;
                mapSpectateComponent.enabled = true;
            }
        }
    }

    void SetPosition()
    {
        if (mapSpectateComponent.enabled)
            return;

        Transform currentTransform = (index <= TeamManager.instance.GetTeamA().soldiers.Count - 1) ? TeamManager.instance.GetTeamA().soldiers[index].transform : TeamManager.instance.GetTeamB().soldiers[index - TeamManager.instance.GetTeamA().soldiers.Count].transform;
        mainCamera.transform.position = currentTransform.transform.position - (currentTransform.forward * spectateOffset.z) + (Vector3.up * spectateOffset.y);
        mainCamera.transform.localEulerAngles = currentTransform.localEulerAngles + spectateRotation;
    }

    void Rotate()
    {
        index++;

        if (index >= TeamManager.instance.GetTeamA().soldiers.Count + TeamManager.instance.GetTeamB().soldiers.Count)
        {
            index = 0;
        }
    }
}
