using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{

    public bool inUse;

    public float autoSelfHideTimer = -1;
    private float _curTimer;

    void Awake()
    {
        Hide();
    }

    void Update()
    {
        if (Time.time >= _curTimer && inUse && autoSelfHideTimer != -1)
        {
            Hide();
        }
    }

    public void Create(Transform parent, bool setParent = false)
    {
        inUse = true;

        if (setParent)
            transform.SetParent(parent, false);

        if (parent != null)
        {
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }

        _curTimer = Time.time + autoSelfHideTimer;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        inUse = false;
        gameObject.SetActive(false);
        transform.SetParent(null);
    }

}