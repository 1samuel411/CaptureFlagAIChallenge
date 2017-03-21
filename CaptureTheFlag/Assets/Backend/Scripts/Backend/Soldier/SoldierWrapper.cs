using System.Collections;
using System.Collections.Generic;
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
        length = Mathf.Clamp(length, -_soldier.viewDistance, _soldier.viewDistance);

        Debug.DrawRay(transform.position + Vector3.up * 1.2f, direction * length, Color.black);

        float angleDif = (transform.localEulerAngles.y - GetRotation(direction)) % 360;
        if (angleDif < 0)
            angleDif = 360 + angleDif;

        RaycastHit raycastHit = new RaycastHit();

        // Check if its out of our sight
        if (angleDif > _soldier.viewRadius && angleDif < (360 - _soldier.viewRadius))
        {
            return raycastHit;
        }

        Physics.Raycast(transform.position + Vector3.up * 1.2f, direction, out raycastHit, _soldier.viewDistance);

        return raycastHit;
    }

    /// <summary>
    /// Casts a ray in the direction given
    /// </summary>
    /// <param name="direction"></param>
    /// <returns>Null if out of our view cone</returns>
    public RaycastHit Raycast(Vector3 direction, float length, LayerMask layermask)
    {

        length = Mathf.Clamp(length, -_soldier.viewDistance, _soldier.viewDistance);

        Debug.DrawRay(transform.position + Vector3.up * 1.2f, direction * length, Color.black);

        float angleDif = (transform.localEulerAngles.y - GetRotation(direction)) % 360;
        if (angleDif < 0)
            angleDif = 360 + angleDif;

        RaycastHit raycastHit = new RaycastHit();

        // Check if its out of our sight
        if (angleDif > _soldier.viewRadius && angleDif < (360 - _soldier.viewRadius))
        {
            return raycastHit;
        }

        Physics.Raycast(transform.position + Vector3.up * 1.2f, direction, out raycastHit, _soldier.viewDistance, layermask);

        return raycastHit;
    }

    private float GetRotation(Vector3 direction)
    {
        return ((((180 * Mathf.Atan2(direction.x, direction.z)) / Mathf.PI) + 0) % 360) - 0;
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

        Vector3 lastRotation = transform.eulerAngles;
        transform.LookAt(position);
        Vector3 newRotation = transform.eulerAngles;
        transform.eulerAngles = lastRotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(newRotation), _soldier.rotationSpeed * Time.deltaTime);

        lookingAt = (Mathf.Abs(transform.eulerAngles.y - newRotation.y)) < 0.5f;

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
