using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;

namespace FPTemplate.Actors
{
    public class CameraController : Singleton<CameraController>, ILookAdapter
	{
		public Actor Actor;
		public bool LockCameraLook { get; set; }
		public bool LockCursor { get; set; } = true;
		public bool UIEnabled { get; set; }
		public Camera Camera { get; private set; }
		public InputSystemUIInputModule InputModule { get; private set; }

		[Header("Camera")]
		public float LookSensitivity = 1;
		public Vector2 LookAngle, LastDelta;
		public Vector3 LookOffset = new Vector3(0, .5f, 0);
		private InputAction m_look;

		[Header("Proxy")]
		public float ProxyChaseSpeed = 1;
		public ICameraControllerProxy Proxy { get; set; }

		public PlayerInput Input => transform.parent.GetComponent<PlayerInput>();

		private void Start()
		{
			Camera = GetComponent<Camera>();
			InputModule = GetComponent<InputSystemUIInputModule>();
            m_look = Input.actions.Single(a => a.name == "Look");
        }

		public void SetGlobalVariables()
		{
            //Shader.SetGlobalInt("_IsPlaying", 1);
            Shader.SetGlobalVector("_WorldClipPos", Camera.transform.position);
            Shader.SetGlobalVector("_WorldClipNormal", Camera.transform.forward);

            var projection = Camera.main.nonJitteredProjectionMatrix;
            projection = GL.GetGPUProjectionMatrix(projection, true);
            Shader.SetGlobalMatrix("_RootProjectionMatrix", projection);
        }

		private void Update()
		{
			SetGlobalVariables();

            InputModule.enabled = UIEnabled;
			Cursor.lockState = LockCursor ? CursorLockMode.Locked : CursorLockMode.Confined;
			LastDelta = m_look.ReadValue<Vector2>() * LookSensitivity;
			if (LockCameraLook)
			{
				return;
			}
			LookAngle += LastDelta * Time.deltaTime;
			while (LookAngle.x < -180) LookAngle.x += 360;
			while (LookAngle.x > 180) LookAngle.x -= 360;
			LookAngle.y = Mathf.Clamp(LookAngle.y, -89, 89);

			if (Proxy != null)
			{
				Proxy.Look(Actor, LastDelta);
				if (Proxy.LookDirectionOverride.HasValue)
				{
					transform.rotation = Proxy.LookDirectionOverride.Value;
				}
				else
				{
					transform.localRotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0);
				}
				if (Proxy.LookPositionOverride.HasValue)
				{
					transform.position = Vector3.Lerp(transform.position, Proxy.LookPositionOverride.Value, Time.deltaTime * ProxyChaseSpeed);
				}
				else
				{
					transform.localPosition = LookOffset;
				}
			}
			else
			{
				transform.localRotation = Quaternion.Euler(-LookAngle.y, LookAngle.x, 0);
				transform.localPosition = LookOffset;
			}
		}

		public void LookAt(Vector3 forward)
		{
			// Change to parent local
			forward = transform.parent.worldToLocalMatrix.MultiplyVector(forward);
			LookAngle.x = Vector2.Angle(forward.Flatten(), transform.forward.Flatten());
			//LookAngle.y = Vector2.Angle(forward.yz(), transform.forward.yz());
		}

		public bool CanSee(Vector3 worldPosition)
		{
			// TODO
			return true;
		}

        private void OnDisable()
        {
            Shader.SetGlobalInt("_IsPlaying", 0);
        }
    }
}