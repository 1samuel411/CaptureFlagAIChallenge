using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShootable
{

    Bullet Shoot();

    Bullet GetBullet();

    IAgent GetOwner();

    void SetOwner(IAgent newOwner);

    Transform GetMuzzle();

}
