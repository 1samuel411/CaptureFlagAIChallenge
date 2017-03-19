using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierWrapper : MonoBehaviour
{

    private Rigidbody _rigidbody;
    private Soldier _soldier;

    public IAgent soldier;

    public List<Soldier> soldiersInSight { get { return _soldier.soldiersInSight; } }
    public List<Flag> flagsInSight { get { return _soldier.flagsInSight; } }

    public Vector3 enemySpawnLocation;

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
            enemySpawnLocation = TeamManager.instance.GetTeamB().spawn.transform.position;
        if (_soldier.teamType == Team.Type.B)
            enemySpawnLocation = TeamManager.instance.GetTeamA().spawn.transform.position;
    }

    public void Update()
    {
        if (_soldier.IsDead())
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            return;
        }

        if (Input.GetKey(KeyCode.U))
        {
            _rigidbody.velocity = Vector3.forward * 2;
        }

        if (Input.GetKey(KeyCode.I))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        _soldier.currentWeapon.Shoot();
    }

    public void LookAt(Vector3 position)
    {
        if (_soldier.IsDead())
            return;

        Vector3 lastRotation = transform.eulerAngles;
        transform.LookAt(position);
        Vector3 newRotation = transform.eulerAngles;
        transform.eulerAngles = lastRotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(newRotation), _soldier.rotationSpeed * Time.deltaTime);
    }

    public void SetName(string soliderName)
    {
        _soldier.soldierName = soliderName;
    }

    public virtual void DamageCallback(Vector3 location){ }

    public virtual void FlagStatusChangedCallback(IGrabable flag) { }

    public virtual void SoldierDiedCallback(IAgent soldier) { }

}
