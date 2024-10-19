using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace FPTemplate.Utilities.Arranger
{
    public class SphereArranger : ArrangerBase
    {
        public float SphereSize;
        public int Count;

        protected override IEnumerable<Matrix4x4> GetMatrices()
        {
            float offset = 2f / Count;
            float increment = Mathf.PI * (3 - Mathf.Sqrt(5)); // Golden angle in radians

            for (int i = 0; i < Count; i++)
            {
                float y = i * offset - 1 + (offset / 2);
                float r = Mathf.Sqrt(1 - y * y);

                float phi = i * increment;

                float x = Mathf.Cos(phi) * r;
                float z = Mathf.Sin(phi) * r;

                // Calculate the position on the sphere by scaling with the sphere size
                Vector3 position = new Vector3(x, y, z) * SphereSize;

                // Create a transformation matrix based on the position
                Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                yield return matrix;
            }
        }
    }
}