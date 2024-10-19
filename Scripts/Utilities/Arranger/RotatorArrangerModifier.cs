using UnityEngine;

#if UNITY_EDITOR
#endif

namespace FPTemplate.Utilities.Arranger
{
    public class RotatorArrangerModifier : ArrangerModifier
    {
        public Vector3 MinRotation, MaxRotation;

        public override Matrix4x4 Mutate(int index, Matrix4x4 mat)
        {
            Random.InitState(index);
            var rot = Matrix4x4.Rotate(Quaternion.Euler(Random.Range(MinRotation.x, MaxRotation.x), Random.Range(MinRotation.y, MaxRotation.y), Random.Range(MinRotation.z, MaxRotation.z)));
            return mat * rot;
        }
    }
}