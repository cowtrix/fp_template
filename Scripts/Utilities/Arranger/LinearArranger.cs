using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace FPTemplate.Utilities.Arranger
{
    public class LinearArranger : ArrangerBase
    {
        public int Count;
        public Vector3 Offset;

        protected override IEnumerable<Matrix4x4> GetMatrices()
        {
            var offset = Vector3.zero;
            for (var x = 0; x < Count; x++)
            {
                yield return Matrix4x4.TRS(
                            new Vector3(offset.x, offset.y, offset.z),
                            Quaternion.identity, Vector3.one);
                offset += Offset;
            }
        }
    }
}