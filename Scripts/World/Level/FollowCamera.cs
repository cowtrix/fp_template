using UnityEngine;
using FPTemplate.Utilities;
using FPTemplate.Actors;

namespace FPTemplate.World
{
	public class FollowCamera : ExtendedMonoBehaviour
	{
		public Vector3 Offset;
		protected Camera CurrentCamera => CameraController.Instance.GetComponent<Camera>();

		private void Update()
		{
			transform.position = CurrentCamera.transform.position + Offset;
		}
	}
}