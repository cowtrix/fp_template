using FPTemplate.Utilities.Extensions;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace FPTemplate.Utilities.Arranger
{
    public class FlipArrangerModifier : ArrangerModifier
    {
        public bool FlipX, FlipY, FlipZ;

        public override Matrix4x4 Mutate(int index, Matrix4x4 mat)
        {
            Random.InitState(index);
            var xScale = FlipX ? RandomExtensions.Flip() ? 1 : -1 : 1;
            var yScale = FlipY ? RandomExtensions.Flip() ? 1 : -1 : 1;
            var zScale = FlipZ ? RandomExtensions.Flip() ? 1 : -1 : 1;
            var scale = Matrix4x4.Scale(new Vector3(xScale, yScale, zScale));
            return mat * scale;
        }
    }
}