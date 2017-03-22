using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour, IAttachments
{

    private string attachmentName = "Laser";

    public LayerMask layerMask;

    public string GetName()
    {
        return attachmentName;
    }

    public Transform laserPoint;
    public LineRenderer laserLineRenderer;
    public Transform laserEndPoint;

    void LateUpdate()
    {
        // cast line from point to forward
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, 1000, layerMask))
        {
            laserEndPoint.gameObject.SetActive(true);
            laserEndPoint.transform.rotation = Quaternion.FromToRotation(-Vector3.back, hit.normal);
            laserEndPoint.transform.position = hit.point + transform.forward * -0.015f;
            laserLineRenderer.SetPositions(new []{laserPoint.position, hit.point });
        }
        else
        {
            laserEndPoint.gameObject.SetActive(false);
            laserLineRenderer.SetPositions(new []{laserPoint.position, laserPoint.forward * -100 });
        }
    }
}
