using UnityEngine;

namespace RaindowStudio.DesignPattern
{
    public class PoolObject : MonoBehaviour
    {
        public ObjectPool Pool { get; set; }
        public string Key { get; set; }

        public virtual void Recycle()
        {
            Pool.RecycleObject(Key);
        }
    }
}
