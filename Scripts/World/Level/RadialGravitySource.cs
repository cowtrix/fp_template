using FPTemplate.Utilities.Extensions;
using UnityEngine;

namespace FPTemplate.World
{
	public class RadialGravitySource : GravitySource
	{
		private Vector3 m_targetGravityDirection;
		public float GravityTransitionSpeed = 1;
		public Vector3 GravityDirection = Vector3.down;
		public float GravityStrength = 1000;
		[Header("Radial Gravity")]
		public float Radius = 100;

		public AnimationCurve Falloff;

		private Vector3 m_lastPosition;
		private Matrix4x4 m_lastTransformMatrix;

		private void Start()
		{
			m_targetGravityDirection = GravityDirection;
		}

		public override Vector3 GetGravityForce(Vector3 position, out float normalizedStrength)
		{
			var localPos = m_lastTransformMatrix.MultiplyPoint3x4(position);
			var grav = m_lastTransformMatrix.GetPosition() - position;
			var dist = grav.sqrMagnitude / (Radius * Radius);
            normalizedStrength = Falloff.Evaluate(dist);
			return grav.normalized * normalizedStrength * GravityStrength;
		}

		public override void SetGravity(Vector3 dir)
		{
			if (!dir.IsOnAxis())
			{
				return;
			}
			m_targetGravityDirection = dir;
		}

		private void Update()
		{
			m_lastTransformMatrix = transform.localToWorldMatrix;
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawLine(transform.position, transform.position + m_targetGravityDirection);
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(default, Radius);
		}
	}
}