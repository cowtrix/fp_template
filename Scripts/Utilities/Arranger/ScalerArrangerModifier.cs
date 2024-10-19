using UnityEngine;

#if UNITY_EDITOR
#endif

namespace FPTemplate.Utilities.Arranger
{
    public class ScalerArrangerModifier : ArrangerModifier
    {
        public Vector3 MinScale, MaxScale;
        public bool Uniform;

        public override Matrix4x4 Mutate(int index, Matrix4x4 mat)
        {
            Random.InitState(index);
            var xScale = Random.Range(MinScale.x, MaxScale.x);
            if (Uniform)
            {
                return mat * Matrix4x4.Scale(new Vector3(xScale, xScale, xScale));
            }
            var yScale = Random.Range(MinScale.y, MaxScale.y);
            var zScale = Random.Range(MinScale.z, MaxScale.z);
            return mat * Matrix4x4.Scale(new Vector3(xScale, yScale, zScale));
        }
    }
}