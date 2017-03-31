using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoldierWrapper : MonoBehaviour
{

    private Rigidbody _rigidbody;
    private Soldier _soldier;

    public IAgent soldier;

    public List<IAgent> soldiersInSight { get { return _soldier.soldiersInSight; } }
    public List<IGrabable> flagsInSight { get { return _soldier.flagsInSight; } }

    public Vector3 enemySpawnLocation;
    public Vector3 spawnLocation;

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _soldier = GetComponent<Soldier>();

        soldier = _soldier;

        _soldier.damagedCallback += DamageCallback;
        _soldier.flagStatusChangedCallback += FlagStatusChangedCallback;
        _soldier.soldierDiedCallback += SoldierDiedCallback;
    }

    public void Start()
    {
        if (_soldier.teamType == Team.Type.A)
        {
            enemySpawnLocation = TeamManager.instance.GetTeamB().spawn.transform.position;
            spawnLocation = TeamManager.instance.GetTeamA().spawn.transform.position;
        }
        if (_soldier.teamType == Team.Type.B)
        {
            enemySpawnLocation = TeamManager.instance.GetTeamA().spawn.transform.position;
            spawnLocation = TeamManager.instance.GetTeamB().spawn.transform.position;
        }
    }

    public void Update()
    {
        if (_rigidbody.velocity.magnitude > _soldier.speed)
        {
          
        }
        if (!_soldier.hasFlag)
        {
            if (flagsInSight.Count > 0)
            {
                for (int i = 0; i < flagsInSight.Count; i++)
                {
                    GrabFlag(flagsInSight[i]);
                }
            }
        }

        if (_soldier.IsDead())
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            return;
        }
    }

    /// <summary>
    /// Returns a List of SoldierWrappers of bots in your team.
    /// </summary>
    /// <returns></returns>
    public List<SoldierWrapper> GetTeamSoldiers()
    {
        List<Soldier> soldiers = (_soldier.teamType == Team.Type.A ? TeamManager.instance.GetTeamA() : TeamManager.instance.GetTeamB()).soldiers;
        List<SoldierWrapper> soldierWrappers = new List<SoldierWrapper>();
        for (int i = 0; i < soldiers.Count; i++)
        {
            if (soldiers[i] == _soldier)
                continue;

            soldierWrappers.Add(soldiers[i].GetComponent<SoldierWrapper>());
        }
        return soldierWrappers;
    }

    /// <summary>
    /// Casts a ray in the direction given
    /// </summary>
    /// <param name="direction"></param>
    /// <returns>Null if out of our view cone</returns>
    public RaycastHit Raycast(Vector3 direction)
    {
        return Raycast(direction, _soldier.viewDistance);
    }

    /// <summary>
    /// Casts a ray in the direction given
    /// </summary>
    /// <param name="direction"></param>
    /// <returns>Null if out of our view cone</returns>
    public RaycastHit Raycast(Vector3 direction, float length)
    {
        return _soldier.Raycast(direction, length);
    }

    /// <summary>
    /// Casts a ray in the direction given
    /// </summary>
    /// <param name="direction"></param>
    /// <returns>Null if out of our view cone</returns>
    public RaycastHit Raycast(Vector3 direction, float length, LayerMask layermask)
    {
        return _soldier.Raycast(direction, length, layermask);
    }

    private float GetRotation(Vector3 direction)
    {
        return _soldier.GetRotation(direction);
    }

    /// <summary>
    /// Grabs the provided flag if the flag is on the other team and we are within distance
    /// </summary>
    /// <param name="flag"></param>
    public void GrabFlag(IGrabable flag)
    {
        _soldier.GrabFlag(flag);
    }

    /// <summary>
    /// Shoots the weapon
    /// </summary>
    public void Shoot()
    {
        if (_soldier.IsDead())
            return;

        _soldier.currentWeapon.Shoot();
    }

    /// <summary>
    /// Move in a direction which will be normalized and multiplied by the speed
    /// </summary>
    /// <param name="direction"></param>
    public void Move(Vector3 direction)
    {
        direction = Vector3.Normalize(direction);
        direction *= _soldier.speed;
        _rigidbody.velocity = direction;
    }

    /// <summary>
    /// Move towards a position directly
    /// </summary>
    /// <param name="direction"></param>
    public void MoveTowards(Vector3 position)
    {
        Move(position - transform.position);
    }

    /// <summary>
    /// Looks at a position
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Has look at been completed</returns>
    public bool LookAt(Vector3 position)
    {
        bool lookingAt = false;

        if (_soldier.IsDead())
            return false;

        // calculate gun offset needed to aim gun at target postion

        Vector3 lastRotation = transform.eulerAngles;

        Transform gun = _soldier.currentWeapon.GetMuzzle();

        Vector3 gunOffset = gun.localPosition;

        Vector3 toTarget = position - transform.position;

        float distance = toTarget.magnitude;

        float offsetAngle = Mathf.Atan(-gunOffset.x / distance) * Mathf.Rad2Deg; // x offset to angle offset  

        Quaternion lookRotation = Quaternion.LookRotation(toTarget, transform.up) * Quaternion.AngleAxis(-offsetAngle, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _soldier.rotationSpeed * Time.deltaTime);

        lookingAt = Quaternion.Angle(transform.rotation, lookRotation) < 0.1f;

        if (lookingAt)
        {
            Ray ray = new Ray(gun.position, gun.forward);

            lookingAt = !Physics.SphereCast(ray, 0.15f, distance, 1 << _soldier.obstaclesLayer);
        }

        return lookingAt;
    }

    public void SetName(string soliderName)
    {
        _soldier.soldierName = soliderName;
    }

    public virtual void DamageCallback(Vector3 location){ }

    public virtual void FlagStatusChangedCallback(IGrabable flag) { }

    public virtual void SoldierDiedCallback(IAgent soldier) { }

}
