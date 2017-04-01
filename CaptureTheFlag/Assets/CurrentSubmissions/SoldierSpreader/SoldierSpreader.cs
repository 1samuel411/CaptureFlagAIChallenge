using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierSpreader : SoldierBase
{

    private NavAgentExample navAgent;

    public bool returningToSpawn = false;

    public bool damaged = false;
    public Vector3 damagedLocation;

    public bool enemySpotted = false;

    public float ignoreDamageTimer = 2;
    Vector3[] poi = new Vector3[]
    {
        new Vector3(8.356801f,0,-7.617511f),
    new Vector3(12.2468f,0,-10.36751f),
    new Vector3(4.146801f,0, -3.337511f),
     new Vector3(-3.633199f,0, 4.892489f),
      new Vector3(-11.5532f,0, 11.93249f),
      new Vector3(-7.813199f,0, 8.932488f),
    };

    private float _curIgnoreDamageTimer;

    // The idea is to make a AI that will run random plays against an AI your working on
    private static int counter = 0;

    public int number = 0;
    Cell prevTarget;
    bool runAstray;

    void Awake()
    {
        number = ++counter;

        navAgent = GetComponent<NavAgentExample>();
        base.Awake();

        SetName("Spreader");


    }

    void Start()
    {
        base.Start();

        number = number % poi.Length;
        Debug.Log(number);
        if (number == 0)
            navAgent.targetCell = GridManager.instance.FindClosestCell(enemySpawnLocation);
        else
            navAgent.targetCell = GridManager.instance.FindClosestCell(poi[number]);
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
        if (soldier.IsDead())
            return;
        CheckTimer();



        // If we were damaged then look at where we damaged, spray!
        if (damaged)
        {
            if (LookAt(damagedLocation))
                Shoot();
            return;
        }

        // Check if a path exists
        if (navAgent.pathGenerated.Count > 0)
        {
            // if there's an enemy then dont keep moving, shoot them!
            MoveTowards(navAgent.pathGenerated[0]);
            if (!enemySpotted)
            {
                // Move in the direction of the next path node

                LookAt(navAgent.pathGenerated[0]);
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
                if (!runAstray)
                    prevTarget = navAgent.targetCell;
                if (navAgent.targetCell != GridManager.instance.FindClosestCell(targetSoldier.GetLocation()))
                    navAgent.targetCell = GridManager.instance.FindClosestCell(targetSoldier.GetLocation());
                runAstray = true;
                // Look at them, if we are, shoot.
                if (LookAt(targetSoldier.GetLocation()))
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
            if (navAgent.targetCell != prevTarget && prevTarget != null && runAstray)
            {
                navAgent.targetCell = prevTarget;
                runAstray = false;
            }
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
        if (agent != this.soldier)
            return;
        navAgent.moveToTarget = false;
        if (number == 0)
        {
            List<SoldierWrapper> team = GetTeamSoldiers();
            foreach (var s in team)
            {
                if (!s.soldier.IsDead())
                {
                    Debug.Log("I died, next flag getter is " + s.name);
                    s.GetComponent<NavAgentExample>().targetCell = GridManager.instance.FindClosestCell(enemySpawnLocation);
                    s.GetComponent<SoldierSpreader>().number = 0;
                    break;
                }
            }
            number = -1;
        }
    }
}
