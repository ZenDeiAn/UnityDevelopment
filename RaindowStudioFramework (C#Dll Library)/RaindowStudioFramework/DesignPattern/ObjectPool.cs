using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RaindowStudio.DesignPattern
{
    public class ObjectPool : MonoBehaviour
    {
        public int initialSize = 10;
        public GameObject poolingObject;

        private Queue<GameObject> inactiveObjects = new Queue<GameObject>();
        private Dictionary<string, GameObject> activedObjects = new Dictionary<string, GameObject>();

        public List<GameObject> ActivedObjects =>
            activedObjects.Values.ToList();

        public bool ContainsActiveObject(string key) =>
            activedObjects.ContainsKey(key);

        public GameObject GetObject(string key = null)
        {
            if (key != null)
            {
                if (activedObjects.TryGetValue(key, out var o))
                {
                    return o;
                }
            }

            GameObject go = inactiveObjects.Count > 0 ? inactiveObjects.Dequeue() : Instantiate(poolingObject);

            if (key == null)
            {
                key = go.GetInstanceID().ToString();
            }

            activedObjects.Add(key, InitializeObject(go, true, key));

            return go;
        }

        public bool RecycleObject(GameObject target)
        {
            if (activedObjects.ContainsKey(target.name))
            {
                activedObjects.Remove(target.name);
                inactiveObjects.Enqueue(target);

                target.SetActive(false);

                return true;
            }

            return false;
        }

        public bool RecycleObject(string key)
        {
            if (activedObjects.ContainsKey(key))
            {
                GameObject target = activedObjects[key];

                activedObjects.Remove(target.name);
                inactiveObjects.Enqueue(target);

                target.SetActive(false);

                return true;
            }

            return false;
        }

        public void RecycleAll()
        {
            List<string> keys = new List<string>(activedObjects.Keys);
            foreach (var key in keys)
            {
                RecycleObject(key);
            }
        }

        private GameObject InitializeObject(GameObject target, bool active, string key)
        {
            if (!target.TryGetComponent(out PoolObject poolObject))
            {
                poolObject = target.AddComponent<PoolObject>();
            }

            poolObject.Key = key;
            poolObject.Pool = this;

            Transform tf = target.transform;
            tf.SetParent(transform);
            tf.localPosition = Vector3.zero;
            tf.rotation = Quaternion.identity;
            tf.localScale = Vector3.one;

            target.name = key;
            target.SetActive(active);

            return target;
        }

        private void Start()
        {
            if (activedObjects.Count != 0)
                return;

            for (int i = 0; i < initialSize; ++i)
            {
                GameObject go = Instantiate(poolingObject);
                inactiveObjects.Enqueue(
                    InitializeObject(go, false, go.GetInstanceID().ToString()));
            }
        }
    }

}
