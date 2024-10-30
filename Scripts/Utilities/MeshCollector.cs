using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPTemplate.Utilities
{
    public class MeshCollector : ExtendedMonoBehaviour
    {
        public MeshRenderer MeshRenderer => GetComponent<MeshRenderer>();
        public MeshCollider MeshCollider => GetComponent<MeshCollider>();
        public MeshFilter MeshFilter => GetComponent<MeshFilter>();
        public Mesh OutputMesh;

        [ContextMenu("Bake Meshes")]
        public void BakeMeshes()
        {
            var meshes = GetComponentsInChildren<MeshFilter>(true)
                .Where(mf => mf != MeshFilter)
                .ToList();
            var combine = new CombineInstance[meshes.Count];
            var mats = new List<Material>();            
            var helper = new Dictionary<int, List<CombineInstance>>(); // Key: shared mesh instance ID, Value: arguments to combine meshes

            // Build combine instances for each type of mesh
            foreach (var m in meshes)
            {
                List<CombineInstance> tmp;
                if (!helper.TryGetValue(m.sharedMesh.GetInstanceID(), out tmp))
                {
                    tmp = new List<CombineInstance>();
                    helper.Add(m.sharedMesh.GetInstanceID(), tmp);
                }

                var ci = new CombineInstance();
                ci.mesh = m.sharedMesh;
                ci.transform = transform.worldToLocalMatrix * m.transform.localToWorldMatrix;
                tmp.Add(ci);

                var mr = m.GetComponent<MeshRenderer>();
                if(mr && mr.sharedMaterial && !mats.Contains(mr.sharedMaterial))
                {
                    mats.Add(mr.sharedMaterial);
                }
            }

            // Combine meshes and build combine instance for combined meshes
            var list = new List<CombineInstance>();
            foreach (var e in helper)
            {
                var m = new Mesh();
                m.CombineMeshes(e.Value.ToArray());
                var ci = new CombineInstance();
                ci.mesh = m;
                list.Add(ci);
            }

            // And now combine everything
            if (!OutputMesh)
            {
                OutputMesh = new Mesh();
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.CreateAsset(OutputMesh, $"Assets/{name}_BakedMesh_{combine.GetHashCode()}.asset");
#endif
            }
            OutputMesh.Clear();
            OutputMesh.CombineMeshes(list.ToArray(), false, false);

            if (MeshFilter)
            {
                MeshFilter.sharedMesh = OutputMesh;
            }
            if (MeshRenderer)
            {
                MeshRenderer.sharedMaterials = mats.ToArray();
            }
        }
    }
}