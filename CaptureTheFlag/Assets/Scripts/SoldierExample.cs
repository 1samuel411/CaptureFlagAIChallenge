using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierExample : SoldierWrapper
{

    private NavAgentExample navAgent;

    public bool returningToSpawn = false;

    public bool damaged = false;
    public Vector3 damagedLocation;

    public bool enemySpotted = false;

    public float ignoreDamageTimer = 2;
    private float _curIgnoreDamageTimer;

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

    void CheckTimer()
    {
        // Stop looking at the location we were damaged from after the timer
        if (Time.time >= _curIgnoreDamageTimer)
        {
            damaged = false;
        }    
    }


    public Vector3 directionTestVector3;
    void Update()
    {
        base.Update();

        CheckTimer();

        // If we were damaged then look at where we damaged, spray!
        if (damaged)
        {
            if(LookAt(damagedLocation))
                Shoot();
            return;
        }

        // Check if a path exists
        if (navAgent.pathGenerated.Count > 0)
        {
            // if there's an enemy then dont keep moving, shoot them!
            if (!enemySpotted)
            {
                // Move in the direction of the next path node
                MoveTowards(navAgent.pathGenerated[0].GetPosition());
                LookAt(navAgent.pathGenerated[0].GetPosition());
            }
        }

        if (soldiersInSight.Count > 0)
        {
            // See if any soldiers in sight are on the enemy team
            IAgent targetSoldier = null;
            for (int i = 0; i < soldiersInSight.Count; i++)
            {
                if (soldiersInSight[i].GetTeam() != soldier.GetTeam())
                {
                    targetSoldier = soldiersInSight[i];
                }
            }

            // enemy spotted?
            enemySpotted = targetSoldier != null;

            // Any enemy soldiers in sight?
            if (targetSoldier != null)
            {
                // Look at them, if we are, shoot.
                if (LookAt(soldiersInSight[0].GetLocation()))
                {
                    // pew pew
                    Shoot();
                }
            }
        }
        else
        {
            // No soldiers so no enemey could be spotted
            enemySpotted = false;
        }

        if (soldier.HasFlag() && !returningToSpawn)
        {
            returningToSpawn = true;

            navAgent.targetCell = GridManager.instance.FindClosestCell(spawnLocation);
        }
    }

    public override void DamageCallback(Vector3 location)
    {
        // We took damage

        // location is the location of the enemy when they shot

        // Stop looking at the location we were damaged from after the timer
        _curIgnoreDamageTimer = Time.time + ignoreDamageTimer;
        damagedLocation = location;
        damaged = true;
    }

    public override void FlagStatusChangedCallback(IGrabable flag)
    {
        // A flag was grabbed or ungrabbed.

        // the other team's flag was grabbed or dropped? Lets go back to our spawn and clear it up
        if (flag.GetTeam() != soldier.GetTeam())
        {
            navAgent.targetCell = GridManager.instance.FindClosestCell(spawnLocation);
        }
    }

    public override void SoldierDiedCallback(IAgent agent)
    {
        // A soldier has died
    }
}
