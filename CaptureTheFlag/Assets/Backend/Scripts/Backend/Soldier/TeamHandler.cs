using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamHandler : MonoBehaviour
{

    public IAgent agent;

    public Material blackHelmetMaterial, blackArmsMaterial, blackBodyMaterial;
    public Material whiteHelmetMaterial, whiteArmsMaterial, whiteBodyMaterial;

    public MeshRenderer helmetMeshRenderer;
    public SkinnedMeshRenderer armsMeshRenderer, bodyMeshRenderer;

    public int teamALayer, teamBLayer;

    public void UpdateMaterials()
    {
        if (agent.GetTeam() == Team.Type.A)
        {
            helmetMeshRenderer.material = blackHelmetMaterial;
            armsMeshRenderer.material = blackArmsMaterial;
            bodyMeshRenderer.material = blackBodyMaterial;

            gameObject.layer = teamALayer;
        }
        else if (agent.GetTeam() == Team.Type.B)
        {
            helmetMeshRenderer.material = whiteHelmetMaterial;
            armsMeshRenderer.material = whiteArmsMaterial;
            bodyMeshRenderer.material = whiteBodyMaterial;

            gameObject.layer = teamBLayer;
        }
    }

    void Update()
    {
        UpdateMaterials();
    }

    void Start()
    {
        agent = GetComponent<IAgent>();
    }
}
