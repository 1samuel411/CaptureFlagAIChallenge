using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour, IAgent
{

    public string soldierName;
    public int index;

    public IShootable currentWeapon;

    public int health, maxHealth;
    public int GetHealth()
    {
        return health;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float speed;
    public float rotationSpeed;

    public Team.Type teamType;
    public Team.Type GetTeam()
    {
        return teamType;
    }

    public AnimationController animationController;
    public AnimationController GetAnimationController()
    {
        return animationController;
    }

    private Rigidbody _rigidbody;
    public new Rigidbody rigidbody
    {
        get
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            return _rigidbody;
        }
    }

    public Transform flagHolder;
    public Transform GetFlagHolder()
    {
        return flagHolder;
    }

    public Transform flagTransform;
    public bool hasFlag
    {
        get { return flagTransform != null; }
    }

    public Vector3 GetLocation()
    {
        return transform.position;
    }

    public int viewRadius;
    public float viewDistance;

    public LayerMask viewLayerMask;

    public int obstaclesLayer = 9;

    public float minFlagRange = 0.5f;

    void Awake()
    {
        maxHealth = health;
        currentWeapon = GetComponentInChildren<IShootable>();
    }

    void Start()
    {
        currentWeapon.SetOwner(this);
    }

    public List<IGrabable> flagsInSight;
    public List<IAgent> soldiersInSight;

    void EyeSight()
    {
        flagsInSight.Clear();
        soldiersInSight.Clear();

        RaycastHit[] hit;

        for (int i = -viewRadius; i < viewRadius; i+=2)
        {
            Vector3 direction = (new Vector3(viewDistance * Mathf.Cos(i * Mathf.PI/180), 0, viewDistance * Mathf.Sin(i * Mathf.PI / 180)));
            direction = Quaternion.AngleAxis(-90 + transform.localEulerAngles.y, Vector3.up) * direction;
            
            hit = (Physics.RaycastAll(transform.position + Vector3.up * 0.2f, direction, viewDistance, viewLayerMask));

            if (hit.Length > 0)
            {
                // hit obstacle
                bool hitObstacle = false;
                float closestDist = 100000;
                for (int x = 0; x < hit.Length; x++)
                {
                    float distance = (transform.position - hit[x].point).magnitude;
                    if (distance < closestDist)
                        closestDist = distance;
                }

                for (int x = 0; x < hit.Length; x++)
                { 
                    if (hit[x].transform.gameObject.layer == obstaclesLayer)
                    {
                        float distance = (transform.position - hit[x].point).magnitude;
                        if (distance <= closestDist)
                        {
                            hitObstacle = true;
                            break;
                        }
                    }
                }

                if (hitObstacle)
                    continue;

                for (int x = 0; x < hit.Length; x++)
                {
                    if (hit[x].transform.tag == "flag")
                    {
                        Flag newFlag = hit[x].transform.GetComponent<Flag>();
                        if(!flagsInSight.Contains(newFlag))
                            flagsInSight.Add(newFlag);
                    }

                    if (hit[x].transform.tag == "soldier")
                    {
                        Soldier newSoldier = hit[x].transform.GetComponent<Soldier>();
                        if (!soldiersInSight.Contains(newSoldier))
                            soldiersInSight.Add(newSoldier);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 direction = (new Vector3(viewDistance * Mathf.Cos(-viewRadius * Mathf.PI / 180), 0, viewDistance * Mathf.Sin(-viewRadius * Mathf.PI / 180)));
        direction = Quaternion.AngleAxis(-90 + transform.localEulerAngles.y, Vector3.up) * direction;

        Debug.DrawRay(transform.position + Vector3.up * 0.2f, direction, (teamType == Team.Type.A) ? Color.red : Color.blue);

        direction = (new Vector3(viewDistance * Mathf.Cos(viewRadius * Mathf.PI / 180), 0, viewDistance * Mathf.Sin(viewRadius * Mathf.PI / 180)));
        direction = Quaternion.AngleAxis(-90 + transform.localEulerAngles.y, Vector3.up) * direction;

        Debug.DrawRay(transform.position + Vector3.up * 0.2f, direction, (teamType == Team.Type.A) ? Color.red : Color.blue);

        direction = (new Vector3(viewDistance * Mathf.Cos(0 * Mathf.PI / 180), 0, viewDistance * Mathf.Sin(0 * Mathf.PI / 180)));
        direction = Quaternion.AngleAxis(-90 + transform.localEulerAngles.y, Vector3.up) * direction;

        Debug.DrawRay(transform.position + Vector3.up * 0.2f, direction, (teamType == Team.Type.A) ? Color.red : Color.blue);
    }

    void Update()
    {
        EyeSight();

        rigidbody.velocity = new Vector3(Mathf.Clamp(rigidbody.velocity.x, -speed, speed), rigidbody.velocity.y, Mathf.Clamp(rigidbody.velocity.z, -speed, speed));
        
        animationController.UpdateHealth(health);

        if (IsDead() && HasFlag())
        {
            DropFlag();
        }
    }

    void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    public delegate void DamagedCallback(Vector3 location);
    public DamagedCallback damagedCallback;

    private bool dead
    {
        get { return health <= 0; }
    }
    public bool IsDead()
    {
        return dead;
    }

    public bool HasFlag()
    {
        return hasFlag;
    }

    public Transform GetFlag()
    {
        return flagTransform;
    }

    public int Damage(int damage, Vector3 damagerLocation)
    {
        if (dead)
            return 0;
        health -= damage;
        animationController.Damage();

        if (health <= 0)
        {
            TeamManager.instance.SendDied(this);
            GetComponent<Collider>().enabled = false;
            rigidbody.isKinematic = true;
        }

        if (damagedCallback != null)
            damagedCallback.Invoke(damagerLocation);

        return health;
    }

    public void DropFlag()
    {
        if (!hasFlag)
            return;

        flagTransform.SetParent(null);
        flagTransform.position = transform.position;
        flagTransform.position = new Vector3(flagTransform.position.x, 1, flagTransform.position.z);
        flagTransform.localEulerAngles = new Vector3(-90, 0, 0);

        flagTransform.GetComponent<Flag>().grabbed = false;
        flagTransform.GetComponent<BoxCollider>().enabled = true;

        flagTransform = null;
    }

    public void GrabFlag(IGrabable flag)
    {
        if (flag.GetTeam() == teamType)
            return;

        if ((flag.GetTransform().position - transform.position).magnitude > minFlagRange)
            return;

        flag.GetTransform().SetParent(flagHolder);
        flag.GetTransform().localPosition = Vector3.zero;
        flag.GetTransform().localRotation = Quaternion.identity;

        flag.SetGrabbed(true);
        flag.GetTransform().GetComponent<BoxCollider>().enabled = false;
        
        flagTransform = flag.GetTransform();
    }

    public delegate void FlagStatusChangedCallback(Flag flag);
    public FlagStatusChangedCallback flagStatusChangedCallback;

    public delegate void SoldierDiedCallback(IAgent soldier);
    public SoldierDiedCallback soldierDiedCallback;

}
