using System;
using Fusion;
using UnityEngine;

public abstract class FusionPoolObject<TClass, TFusionObjectPoolClass> : NetworkBehaviour
    where TFusionObjectPoolClass : FusionObjectPool<TFusionObjectPoolClass, TClass>
    where TClass : FusionPoolObject<TClass, TFusionObjectPoolClass>
{
    public TFusionObjectPoolClass Pool => _pool ??= FusionObjectPool<TFusionObjectPoolClass, TClass>.Instance;

    protected TFusionObjectPoolClass _pool;

    public Action<TClass, bool> afterActiveChangeAction;
    
    [Networked(OnChanged = nameof(OnActiveChanged))] public bool Active { get; set; }
    
    public static void OnActiveChanged(Changed<FusionPoolObject<TClass, TFusionObjectPoolClass>> changed)
    {
        var self = changed.Behaviour;
        TFusionObjectPoolClass pool = self.Pool;
        
        pool.PoolListOperation(self as TClass, self.Active);

        self.OnActiveChanged(self.Active);
        
        self.afterActiveChangeAction?.Invoke(self as TClass, self.Active);
        self.afterActiveChangeAction = null;
    }

    public virtual void OnActiveChanged(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual bool Recycle()
    {
        return Pool.RecyclePoolObject(this as TClass);
    }
}
