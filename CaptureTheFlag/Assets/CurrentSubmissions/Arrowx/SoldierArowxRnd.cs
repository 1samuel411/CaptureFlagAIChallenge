using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SoldierArowxRnd : SoldierWrapper
{
 
    private NavAgentExample navAgent;
 
    public bool returningToSpawn = false;
 
    public bool damaged = false;
    public Vector3 damagedLocation;
 
    public bool enemySpotted = false;
 
    public float ignoreDamageTimer = 2;
    private float _curIgnoreDamageTimer;
 
    // The idea is to make a AI that will run random plays against an AI your working on
    private static int counter = 0;
    private static List<SoldierArowxRnd> team = new List<SoldierArowxRnd>(8);
 
    private static List<Vector3> plays = new List<Vector3>(8);
 
    public int number = 0;
 
    void Awake()
    {
        number = counter++;
 
        team.Add(this);
 
        navAgent = GetComponent<NavAgentExample>();
 
        if (plays.Count == 0)
        {
            int c = Random.Range(1, 8);
 
            float gx = (GridManager.instance.gridSize.x * 0.5f);
            float gy = (GridManager.instance.gridSize.y * 0.5f);
 
            Vector2 pos;
 
            for (int i = 0; i < c; i++)
            {
                pos.x = (float)Random.Range(-gx, gx);
                pos.y = (float)Random.Range(-gy, gy);
                plays.Add(GridManager.instance.FindClosestCell( pos, true ).GetPosition());
 
                Debug.Log("Play " + i + " " + plays[i].ToString());
            }
        }
 
        base.Awake();
 
        SetName("ArowxRnd");
 
     
    }
 
    void Start()
    {
        base.Start();
 
        int play = number % plays.Count;
        Debug.Log("Soldier " + number + " using play " + play);
        navAgent.targetCell = GridManager.instance.FindClosestCell(plays[play]);
 
        //navAgent.targetCell = GridManager.instance.FindClosestCell(enemySpawnLocation);
 
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
                MoveTowards(navAgent.pathGenerated[0]);
                LookAt(navAgent.pathGenerated[0]);
            }
        }
        else
        {
            if ((!enemySpotted) && (!soldier.HasFlag()) && !returningToSpawn)
            {
                navAgent.targetCell = GridManager.instance.FindClosestCell(enemySpawnLocation);
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