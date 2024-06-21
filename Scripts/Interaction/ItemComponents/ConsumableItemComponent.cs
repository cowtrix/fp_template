using FPTemplate.Actors;
using System;
using System.Collections.Generic;

namespace FPTemplate.Interaction.Items
{
	[Serializable]
	public class IntResourceDelta
	{
		public string StateKey;
		public int Amount;
		public string Description;
	}

	public class ConsumableItemComponent : ItemComponent
	{
		public List<IntResourceDelta> Deltas;
		public bool ConsumeOnPickup;

		public override void OnPickup(Actor actor)
		{
			base.OnPickup(actor);
			if (ConsumeOnPickup)
			{
				Consume(actor);
			}
		}

		public void Consume(Actor actor)
		{
			foreach (var delta in Deltas)
			{
				actor.State.TryAdd(delta.StateKey, delta.Amount, delta.Description);
			}
			Destroy(Item.gameObject);
		}
	}
}