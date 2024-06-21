using UnityEngine;
using FPTemplate;
using FPTemplate.Utilities;
using FPTemplate.Actors;

namespace FPTemplate.World
{
	public class LookAtPlayer : ExtendedMonoBehaviour
	{
		public Vector3 AdditionalRotation;
		protected Camera CurrentCamera => CameraController.Instance.GetComponent<Camera>();

		private void Update()
		{
			transform.LookAt(CurrentCamera.transform);
			transform.rotation *= Quaternion.Euler(AdditionalRotation);
		}
	}
}