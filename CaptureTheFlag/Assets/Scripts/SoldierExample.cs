using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierExample : SoldierWrapper
{

    private NavAgentExample navAgent;

    void Awake()
    {
        base.Awake();

        SetName("Example");

        navAgent = GetComponent<NavAgentExample>();
    }

    void Start()
    {
        base.Start();

        navAgent.targetCell = GridManager.instance.FindClosestCell(enemySpawnLocation);
    }

    void Update()
    {
        base.Update();

        if (soldiersInSight.Count > 0)
        {
            LookAt(soldiersInSight[0].GetLocation());
            Shoot();
        }
    }

    public override void DamageCallback(Vector3 location)
    {
        // We took damage

        // location is the location of the enemy when they shot
    }

    public override void FlagStatusChangedCallback(IGrabable flag)
    {
        // A flag was grabbed or ungrabbed.
    }

    public override void SoldierDiedCallback(IAgent agent)
    {
        // A soldier has died
    }
}
