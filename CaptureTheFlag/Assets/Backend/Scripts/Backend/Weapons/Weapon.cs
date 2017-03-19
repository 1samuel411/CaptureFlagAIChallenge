using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IShootable
{

    public IAgent ownerAgent;

    public Bullet bullet;

    public GameObject muzzleFlashGameObject;

    public Transform muzzleHolderTransform;
    public Transform casingHolderTransform;

    public Material muzzleMaterial;
    public Texture[] flashTextures;

    public bool canShoot = true;

    public float shootingRate = 0.6f;
    private float _curShootingRate;

    public float flashTime = 0.2f;
    private float _curFlashTime;

    public Vector2 casingForceRandomness;

    private AudioSource _source;

    public Bullet Shoot()
    {
        if (!canShoot)
            return null;

        // Set Timers
        _curFlashTime = Time.time + flashTime;
        _curShootingRate = Time.time + shootingRate;

        // Create casing
        if(PoolManager.instance.currentSystem.id != bullet.poolCasingIndex)
            PoolManager.instance.SetPath(bullet.poolCasingIndex);
        Rigidbody newCasing = PoolManager.instance.Create(casingHolderTransform).GetComponent<Rigidbody>();
        newCasing.AddRelativeForce(Vector3.right * Random.Range(casingForceRandomness.x, casingForceRandomness.y), ForceMode.Impulse);
        newCasing.AddRelativeForce(Vector3.forward * Random.Range(casingForceRandomness.x, casingForceRandomness.y)/2, ForceMode.Impulse);
        newCasing.AddTorque(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));

        // Create bullet
        PoolManager.instance.SetPath(bullet.poolIndex);
        Bullet newBullet = PoolManager.instance.Create(muzzleHolderTransform).GetComponent<Bullet>();
        newBullet.transform.position = muzzleHolderTransform.position;
        newBullet.transform.rotation = muzzleHolderTransform.rotation;
        newBullet.bulletOwner = this;

        // Change muzzle
        muzzleMaterial.mainTexture = flashTextures[Random.Range(0, flashTextures.Length)];
        muzzleFlashGameObject.transform.localEulerAngles = new Vector3(muzzleFlashGameObject.transform.localEulerAngles.x, muzzleFlashGameObject.transform.localEulerAngles.y, Random.Range(0, 360));

        // Play Sound
        _source.PlayOneShot(bullet.bulletSound);

        // Play Animation
        ownerAgent.GetAnimationController().Shoot();

        return newBullet;
    }

    public Bullet GetBullet()
    {
        return bullet;
    }

    public IAgent GetOwner()
    {
        return ownerAgent;
    }

    public Transform GetMuzzle()
    {
        return muzzleHolderTransform;
    }

    public void SetOwner(IAgent newOwner)
    {
        ownerAgent = newOwner;
    }

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    void Update()
    {
        ManageTimers();
    }

    void ManageTimers()
    {
        canShoot = (Time.time >= _curShootingRate);

        muzzleFlashGameObject.SetActive(!(Time.time >= _curFlashTime));
    }
}