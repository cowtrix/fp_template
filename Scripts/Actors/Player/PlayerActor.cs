using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;
using FPTemplate.Interaction;
using FPTemplate.Utilities.Helpers;
using FPTemplate.Utilities.Extensions;

namespace FPTemplate.Actors.Player
{
	[Serializable]
	public class ActorActionEvent : UnityEvent<ActorAction> { }

	public class PlayerActor : Actor
	{
		public CameraController CameraController => CameraController.Instance;
		public ActorActionEvent OnActionExecuted = new ActorActionEvent();

		protected override int TickOnThread(float dt) => 0;

		private void Update()
		{
			var cameraForward = LookAdapter.transform.forward;
			var cameraPos = LookAdapter.transform.position;
			var isHit = Physics.Raycast(cameraPos, cameraForward, out var interactionHit, 1000, InteractionMask, QueryTriggerInteraction.Collide);
			LastRaycast = interactionHit;
			if (isHit)
			{
				Debug.DrawLine(cameraPos, interactionHit.point, Color.yellow);
				DebugHelper.DrawPoint(interactionHit.point, .5f, Color.yellow, 0);
				var interactable = interactionHit.collider.GetComponent<Interactable>() ?? interactionHit.collider.GetComponent<InteractionForwarder>()?.Interactable;
				if (interactable && interactable.enabled && interactionHit.distance < interactable.InteractionSettings.MaxFocusDistance)
				{
					if (interactable != FocusedInteractable)
					{
						FocusedInteractable?.ExitFocus(this);
						FocusedInteractable = interactable;
						FocusedInteractable.EnterFocus(this);
					}
					return;
				}
			}

			FocusedInteractable?.ExitFocus(this);
			FocusedInteractable = null;
			Debug.DrawLine(cameraPos, cameraPos + cameraForward * 1000, Color.magenta.WithAlpha(.25f));
		}

		public void ActionExecuted(InputAction.CallbackContext cntxt)
		{
			// Construct action
			var action = new ActorAction(
				cntxt.GetActionKey(),
				cntxt.started ? eActionState.Start : cntxt.canceled ? eActionState.End : eActionState.Tick,
				FocusedInteractable ? FocusedInteractable.gameObject : null,
				cntxt.valueType == typeof(Vector2) ? cntxt.ReadValue<Vector2>() : default);

			//Debug.Log($"Action: {action} {action.State} {action.Context}");
			OnActionExecuted.Invoke(action);

			// Otherwise, execute the action on the focused object
			if (FocusedInteractable)
			{
				var actions = FocusedInteractable.GetActions(this);
				if (actions.Any(a => a.Key == action.Key && a.State == action.State))
				{
					FocusedInteractable.ReceiveAction(this, action);
					return;
				}
			}

			if (action.Key == eActionKey.MOVE)
			{
				// Always send mvoement to movement controller otherwise
				MovementController.MoveInput(action.VectorContext);
				return;
			}

			if (action.Key == eActionKey.JUMP && action.State == eActionState.Start)
			{
				MovementController.Jump();
				return;
			}

			// If we have an equipped item, send it the action
			if (State.EquippedItem != null
				&& State.EquippedItem.GetActions(this).Any(a => a.Key == action.Key))
			{
				State.EquippedItem.ReceiveAction(this, action);
				return;
			}
		}
	}
}