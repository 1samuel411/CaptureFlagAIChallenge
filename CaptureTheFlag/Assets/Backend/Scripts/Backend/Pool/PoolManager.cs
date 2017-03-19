using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    public static PoolManager instance;

    public List<PoolSystem> poolSystems = new List<PoolSystem>();

    private bool initialized;

    void Awake()
    {
        instance = this;

        Initialize();
    }

    public void Initialize()
    {
        if (initialized)
            return;

        initialized = true;

        for (int i = 0; i < poolSystems.Count; i++)
        {
            for (int x = 0; x < poolSystems[i].initializeAmount; x++)
            {
                poolSystems[i].poolObjects.Add(GameObject.Instantiate(poolSystems[i].poolObject));
            }
        }
    }

    public PoolSystem currentSystem;
    public void SetPath(int id)
    {
        currentSystem = poolSystems.Where(t => t.id == id).ToList().FirstOrDefault();
    }

    public PoolObject Create(PoolSystem poolSystem, Transform parent)
    {
        PoolObject nonTakenObject = null;
        if (poolSystem.poolObjects != null && poolSystem.poolObjects.Count > 0)
        {
            nonTakenObject = poolSystem.poolObjects.FirstOrDefault(t => t.inUse == false);
            if (nonTakenObject == null)
            {
                nonTakenObject = GameObject.Instantiate(poolSystem.poolObject);
                poolSystem.poolObjects.Add(nonTakenObject);
            }
        }
        nonTakenObject.Create(parent);
        return nonTakenObject;
    }

    public PoolObject Create(Transform parent)
    {
        return Create(currentSystem, parent);
    }

    public void Clear(PoolSystem poolSystem)
    {
        if (poolSystem != null && poolSystem.poolObjects.Count > 0 && poolSystem.poolObjects != null)
            poolSystem.poolObjects.Where(t => t.inUse).ToList().ForEach(t => t.Hide());
    }

    public void Clear()
    {
        Clear(currentSystem);
    }
}

[System.Serializable]
public class PoolSystem
{
    public string name;

    public int id;

    public PoolObject poolObject;

    public List<PoolObject> poolObjects = new List<PoolObject>();

    public int initializeAmount;
}