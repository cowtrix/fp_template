using FPTemplate.Utilities;
using UnityEngine;

namespace FPTemplate.World
{
    public abstract class GravitySource : TrackedObject<GravitySource>
    {
        public bool Exclusive = true;
        public abstract Vector3 GetGravityForce(Vector3 position, out float normalizedStrength);
        public abstract void SetGravity(Vector3 vector3);
    }

}