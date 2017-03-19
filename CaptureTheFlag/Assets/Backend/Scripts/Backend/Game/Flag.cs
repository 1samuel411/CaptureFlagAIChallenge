using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour, IGrabable
{

    public SkinnedMeshRenderer meshRenderer;
    public Material teamAMaterial;
    public Material teamBMaterial;

    public Team.Type teamFlag;

    private Transform originalHolder;

    private BoxCollider _boxCollider;

    private bool _grabbed;
    public bool grabbed
    {
        get { return _grabbed; }
        set
        {
            if(_grabbed != value)
               TeamManager.instance.SendFlagMessage(this);
            _grabbed = value;
        }
    }

    void Start()
    {
        originalHolder = transform.parent;
    }

    void Update()
    {
        UpdateMaterials();
    }

    void UpdateMaterials()
    {
        if (teamFlag == Team.Type.A)
            meshRenderer.material = teamAMaterial;
        if (teamFlag == Team.Type.B)
            meshRenderer.material = teamBMaterial;
    }

    public void ResetFlag()
    {
        grabbed = false;
        transform.SetParent(originalHolder);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        GetComponent<BoxCollider>().enabled = true;
    }

    void OnDrawGizmos()
    {
        if (_boxCollider == null)
            _boxCollider = GetComponent<BoxCollider>();

        if (teamFlag == Team.Type.A)
            Gizmos.color = Color.red;
        else if (teamFlag == Team.Type.B)
            Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(_boxCollider.center + transform.position, _boxCollider.size / 2);
    }


    public Team.Type GetTeam()
    {
        return teamFlag;
    }

    public Vector3 GetLocation()
    {
        return transform.position;
    }
}
