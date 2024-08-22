using Fusion;
using UnityEngine;

public abstract class FusionPoolObject<TClass, TFusionObjectPoolClass> : NetworkBehaviour
    where TFusionObjectPoolClass : FusionObjectPool<TFusionObjectPoolClass, TClass>
    where TClass : FusionPoolObject<TClass, TFusionObjectPoolClass>
{
    public TFusionObjectPoolClass Pool => _pool ??= FusionObjectPool<TFusionObjectPoolClass, TClass>.Instance;

    protected TFusionObjectPoolClass _pool;
    
    [Networked(OnChanged = nameof(OnActiveChanged))] public bool Active { get; set; }

    protected static void OnActiveChanged(Changed<FusionPoolObject<TClass, TFusionObjectPoolClass>> changed)
    {
        var self = changed.Behaviour;
        TFusionObjectPoolClass pool = self.Pool;
        
        if (self.Active)
        {
            if (!pool.ActiveObjects.Contains(self as TClass))
            {
                pool.ActiveObjects.Add(self as TClass);
            }
            if (pool.InactiveObjects.Contains(self as TClass))
            {
                pool.InactiveObjects.Remove(self as TClass);
            }

        }
        else
        {
            if (pool.ActiveObjects.Contains(self as TClass))
            {
                pool.ActiveObjects.Remove(self as TClass);
            }
            if (!pool.InactiveObjects.Contains(self as TClass))
            {
                pool.InactiveObjects.Add(self as TClass);
            }
        }

        self.OnActiveChanged(self.Active);
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
