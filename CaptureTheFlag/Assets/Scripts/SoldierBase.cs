using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBase : SoldierWrapper
{

    void Awake()
    {
        base.Awake();

        // Name your team
        SetName("Base");
        
        // End of Awake Method, Not a good idea to add code, some things are not initialized. Put it in Start if possible
    }

    void Start()
    {
        base.Start();

        // Add your code
    }

    void Update()
    {
        base.Update();

        // Add your code
    }

    /// <summary>
    /// Damage was taken on this unit
    /// </summary>
    /// <param name="location">Location of the damage</param>
    public override void DamageCallback(Vector3 location)
    {
        // turn to that direction and shoot em up!
    }

    /// <summary>
    /// A Flag was grabbed or dropped
    /// </summary>
    /// <param name="flag"></param>
    public override void FlagStatusChangedCallback(IGrabable flag)
    {
        //Team.Type flagTeam = flag.GetTeam();

        //Vector3 location = flag.GetLocation();
    }

    /// <summary>
    /// A Soldier has died
    /// </summary>
    /// <param name="soldier"></param>
    public override void SoldierDiedCallback(IAgent soldier)
    {
        //Team.Type soldierTeam = soldier.GetTeam();
        
        //Vector3 location = soldier.GetLocation();
    }
}
