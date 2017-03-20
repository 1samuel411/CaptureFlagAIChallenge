using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;

    public string speedForwardName = "ForwardVelocity";
    public string speedHorizontalName = "LeftVelocity";
    public string firingName = "Fire";
    public string healthName = "Health";

    public string damagedName = "Damaged";

    private Rigidbody _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 velocity = transform.TransformDirection(_rigidbody.velocity);
        animator.SetFloat(speedForwardName, velocity.z);
        animator.SetFloat(speedHorizontalName, velocity.x);
    }

    public void UpdateHealth(int newHealth)
    {
        animator.SetInteger(healthName, newHealth);
    }

    public void Shoot()
    {
        animator.SetTrigger(firingName);
    }

    public void Damage()
    {
        animator.SetTrigger(damagedName);
    }

}
