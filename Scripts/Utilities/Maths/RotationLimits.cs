using System;
using UnityEngine;

namespace FPTemplate.Utilities.Maths
{
    [Serializable]
    public struct RotationLimits
    {
        public Vector2 X, Y, Z;

        public Quaternion ClampRotation(Quaternion rot) =>
            Quaternion.Euler(ClampRotation(rot.eulerAngles));

        public Vector3 ClampRotation(Vector3 rot) =>
            new Vector3(Mathf.Clamp(rot.x, X.x, X.y), Mathf.Clamp(rot.y, Y.x, Y.y), Mathf.Clamp(rot.z, Z.x, Z.y));
    }
}