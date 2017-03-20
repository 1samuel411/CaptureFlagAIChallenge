using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAgent
{
    Team.Type GetTeam();

    AnimationController GetAnimationController();

    Vector3 GetLocation();

    int GetHealth();
    int GetMaxHealth();

    int Damage(int damage, Vector3 damagerLocation);

    Transform GetFlagHolder();

    void DropFlag();
    void GrabFlag(Flag flag);
    bool HasFlag();
    Transform GetFlag();

    bool IsDead();
}
