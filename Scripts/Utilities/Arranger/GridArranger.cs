using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

#if UNITY_EDITOR
#endif

namespace FPTemplate.Utilities.Arranger
{
    public class GridArranger : ArrangerBase
    {
        public Vector3Int GridSize;
        public Vector3 GridOffset;

        protected override IEnumerable<Matrix4x4> GetMatrices()
        {
            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    for (var z = 0; z < GridSize.z; z++)
                    {
                        yield return Matrix4x4.TRS(
                            new Vector3(x * GridOffset.x, y * GridOffset.y, z * GridOffset.z),
                            Quaternion.identity, Vector3.one);
                    }
                }
            }
        }
    }
}