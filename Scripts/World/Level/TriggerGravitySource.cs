using UnityEngine;

namespace FPTemplate.World
{
    public class TriggerGravitySource : GravitySource
    {
        public Vector3 GravityVector;
        public Collider Collider => GetComponent<Collider>();

        public override Vector3 GetGravityForce(Vector3 position, out float normalizedStrength)
        {
            if (!Collider.bounds.Contains(position))
            {
                normalizedStrength = 0f;
                return Vector3.zero;
            }
            normalizedStrength = Mathf.Abs(Collider.ClosestPoint(position).magnitude);
            return transform.localToWorldMatrix.MultiplyVector(GravityVector * normalizedStrength);
        }

        public override void SetGravity(Vector3 vector3)
        {
            GravityVector = vector3;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(default, GravityVector);
        }
    }
}