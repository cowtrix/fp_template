using FPTemplate.Actors.Player;
using UnityEngine;

namespace FPTemplate.Interaction
{
	public class InteractionColliderEvent : MonoBehaviour
	{
		public PlayerActor Player;
		private void OnTriggerEnter(Collider other)
		{
			var interactable = other.GetComponent<Interactable>();
			if (!interactable)
			{
				return;
			}
			interactable.InteractionSettings.OnEnterAttention.Invoke(Player);
		}

		private void OnTriggerExit(Collider other)
		{
			var interactable = other.GetComponent<Interactable>();
			if (!interactable)
			{
				return;
			}
			interactable.InteractionSettings.OnExitAttention.Invoke(Player);
		}
	}
}