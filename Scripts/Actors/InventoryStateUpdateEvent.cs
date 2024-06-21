using FPTemplate.Interaction.Items;
using System;
using UnityEngine.Events;

namespace FPTemplate.Actors
{
    [Serializable]
	public class InventoryStateUpdateEvent : UnityEvent<Actor, ActorState.eInventoryAction, Item> { }
}