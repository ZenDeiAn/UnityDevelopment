using System;
using System.Collections.Generic;
using Airpass.DesignPattern;
using Tools.Utility;
using UnityEngine;

public abstract class FusionObjectPool<TClass, TFusionPoolObjectClass> : SingletonUnityEternal<TClass>
    where TClass : FusionObjectPool<TClass, TFusionPoolObjectClass>
    where TFusionPoolObjectClass : FusionPoolObject<TFusionPoolObjectClass, TClass>
{
    public List<TFusionPoolObjectClass> ActiveObjects => Instance._activeObjects;
    public List<TFusionPoolObjectClass> InactiveObjects => Instance._inactiveObjects;
    
    [SerializeField] protected GameObject _poolObject;
    [SerializeField, Range(1, 999)] protected int maxSize;
    
    protected List<TFusionPoolObjectClass> _inactiveObjects = new();
    protected List<TFusionPoolObjectClass> _activeObjects = new();

    public virtual bool GetPoolObject(out TFusionPoolObjectClass poolObject,
        Action<TFusionPoolObjectClass> afterAuthorized = null,
        Action<TFusionPoolObjectClass> afterActive = null,
        Vector3 position = default, Quaternion rotation = default)
    {
        poolObject = null;
        
        if (_inactiveObjects.Count == 0)
        {
            if (_activeObjects.Count >= maxSize)
                return false;

            // else
            poolObject = FusionManager.NetworkRunner.Spawn(_poolObject, position, rotation).GetComponent<TFusionPoolObjectClass>();
            InitializeAuthority(poolObject, afterAuthorized, afterActive);
            
            return true;
        }

        // else
        poolObject = _inactiveObjects[0];
        InitializeAuthority(poolObject, afterAuthorized, afterActive);
        return true;
    }

    public virtual bool RecyclePoolObject(TFusionPoolObjectClass poolObject)
    {
        if (!_activeObjects.Contains(poolObject))
        {
            return false;
        }

        _activeObjects.Remove(poolObject);
        _inactiveObjects.Add(poolObject);
        
        return true;
    }


    public virtual void RecycleAllPoolObjects()
    {
        foreach (var poolObject in _activeObjects)
        {
            RecyclePoolObject(poolObject);
        }
    }

    protected void InitializeAuthority(TFusionPoolObjectClass poolObject,
        Action<TFusionPoolObjectClass> afterAuthorized,
        Action<TFusionPoolObjectClass> afterActive)
    {
        if (!poolObject.Object.HasStateAuthority)
        {
            poolObject.Object.RequestStateAuthority();
        }

        Instance.WaitUntilToDo(() => poolObject.HasStateAuthority,
            () =>
            {
                afterAuthorized?.Invoke(poolObject);
                poolObject.Active = true;
                Instance.WaitUntilToDo(() => poolObject.Active,
                    () => afterActive?.Invoke(poolObject));
            });
    }
}