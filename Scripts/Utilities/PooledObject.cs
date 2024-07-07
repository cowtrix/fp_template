using UnityEngine;

namespace FPTemplate.Utilities
{
    public class PooledObject : ExtendedMonoBehaviour, IPooledObject
    {
        public GameObject Source { get => Prefab; set => Prefab = value; }
        public GameObject Prefab;

        private void OnDisable()
        {
            if (Prefab && ObjectPool.HasInstance())
            {
                ObjectPool.Instance.Release(Prefab, gameObject);
            }
        }
    }
}