using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInfo : MonoBehaviour
{
    public static WorldInfo Instance { get; private set; }

    private Vector3 _minBounds = Vector3.zero;
    private Vector3 _maxBounds = Vector3.zero;

    public event Action OnLoadAction;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMinBounds(Vector3 minBounds)
    {
        _minBounds = minBounds;
    }

    public Vector3 GetMinBounds()
    {
        return _minBounds;
    }
    
    public void SetMaxBounds(Vector3 maxBounds)
    {
        _maxBounds = maxBounds;
    }
    
    public Vector3 GetMaxBounds()
    {
        return _maxBounds;
    }
    
    public void OnLoad()
    {
        OnLoadAction?.Invoke();
    }
}
