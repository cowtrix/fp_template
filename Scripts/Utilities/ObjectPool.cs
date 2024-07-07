using FPTemplate.Utilities.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPTemplate.Utilities
{
    public interface IPooledObject
    {
        public GameObject Source { get; set; }
    }

    public class ObjectPool : Singleton<ObjectPool>
    {
        private class Pool
        {
            public List<GameObject> ActiveInstances = new List<GameObject>();
            public List<GameObject> InactiveInstances = new List<GameObject>();
        }
        private Dictionary<GameObject, Pool> m_pools = new Dictionary<GameObject, Pool>();
        private Transform m_inactiveInstances;
        public override void Awake()
        {
            m_inactiveInstances = new GameObject("Inactive Pooled Objects").transform;
            base.Awake();
        }

        public GameObject Get(GameObject prefab, Transform parent = null)
        {
            if (!m_pools.TryGetValue(prefab, out var pool))
            {
                pool = new Pool();
                m_pools.Add(prefab, pool);
            }
            GameObject instance;
            if (!pool.InactiveInstances.Any())
            {
                instance = CreateNewInstance(prefab, pool);
            }
            else
            {
                instance = pool.InactiveInstances.First();
                pool.InactiveInstances.Remove(instance);
                pool.ActiveInstances.Add(instance);
            }
            StartCoroutine(SetParent(instance.transform, parent));
            instance.SetActive(true);
            var pooledObj = instance.GetComponentByInterface<IPooledObject>();
            if (pooledObj != null && !pooledObj.Equals(null))
            {
                pooledObj.Source = prefab;
            }
            return instance;
        }

        private GameObject CreateNewInstance(GameObject prefab, Pool pool)
        {
            var newInstance = Instantiate(prefab);
            pool.ActiveInstances.Add(newInstance);
            return newInstance;
        }

        public void Release(GameObject prefab, GameObject instance)
        {
            if (!m_pools.TryGetValue(prefab, out var pool))
            {
                pool = new Pool();
                m_pools.Add(prefab, pool);
            }
            pool.ActiveInstances.Remove(instance);
            pool.InactiveInstances.Add(instance);
            StartCoroutine(SetParent(instance.transform, m_inactiveInstances));
        }

        private IEnumerator SetParent(Transform target, Transform parent)
        {
            yield return null;
            target.SetParent(parent);
        }
    }
}