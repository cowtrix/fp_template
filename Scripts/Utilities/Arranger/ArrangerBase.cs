using System.Collections.Generic;
using UnityEngine;
using FPTemplate.Utilities.Extensions;
using System;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FPTemplate.Utilities.Arranger
{
    public abstract class ArrangerBase : ExtendedMonoBehaviour
    {
        [Serializable]
        public class SpawnConfiguration
        {
            public GameObject Prefab;
            [Range(0, 1)]
            public float SpawnChance = 1;
        }

        public IEnumerable<ArrangerModifier> Modifiers => GetComponents<ArrangerModifier>();
        public List<GameObject> Instances;
        public List<SpawnConfiguration> Prefabs;

        [ContextMenu("Invalidate")]
        public void Invalidate()
        {
#if UNITY_EDITOR
            foreach (var instance in Instances)
            {
                instance.SafeDestroy();
            }
            Instances.Clear();
            var counter = 0;
            foreach (var mat in GetMatrices())
            {
                var modifiedMatrix = mat;
                foreach(var modifier in Modifiers)
                {
                    modifiedMatrix = modifier.Mutate(counter, modifiedMatrix);
                }
                var prefab = RandomExtensions.WeightedRandom(Prefabs.Select(p => p.Prefab), Prefabs.Select(p => p.SpawnChance));
                var newInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                newInstance.transform.SetParent(transform);
                newInstance.transform.localPosition = modifiedMatrix.GetPosition();
                newInstance.transform.localScale = modifiedMatrix.GetScale();
                newInstance.transform.localRotation = modifiedMatrix.GetRotation();
                Instances.Add(newInstance);
                counter++;
            }
#endif
        }

        protected abstract IEnumerable<Matrix4x4> GetMatrices();

        private void OnDrawGizmosSelected()
        {
            foreach (var mat in GetMatrices())
            {
                Gizmos.matrix = transform.localToWorldMatrix * mat;
                Gizmos.DrawCube(default, Vector3.one);
            }
        }
    }
}