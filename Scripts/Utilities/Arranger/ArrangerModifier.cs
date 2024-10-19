using UnityEngine;

#if UNITY_EDITOR
#endif

namespace FPTemplate.Utilities.Arranger
{
    public abstract class ArrangerModifier : ExtendedMonoBehaviour
    {
        public abstract Matrix4x4 Mutate(int index, Matrix4x4 mat);
    }
}