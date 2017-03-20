using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabable 
{

    Team.Type GetTeam();

    Vector3 GetLocation();

    bool Grabbed();
}
