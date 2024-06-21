using FPTemplate.Utilities.Extensions;
using UnityEngine;

namespace FPTemplate.World
{
	public class SpawnPosition : MonoBehaviour
	{
		public string SpawnID;

		private void OnDrawGizmos()
		{
			GizmoExtensions.DrawWireCube(transform.position, new Vector3(1, 2, 1) * .5f, transform.rotation, Color.green);
			Gizmos.DrawLine(transform.position, transform.position + transform.forward);
		}
	}
}