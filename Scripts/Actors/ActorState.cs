using FPTemplate.Interaction.Activities;
using FPTemplate.Interaction.Items;
using System.Collections.Generic;
using UnityEngine;

namespace FPTemplate.Actors
{
    public class ActorState : StateContainer
	{
		public enum eInventoryAction
		{
			DROP, PICKUP, EQUIP,
			UNEQUIP,
            REFRESH
        }

		public InventoryStateUpdateEvent OnInventoryUpdate = new InventoryStateUpdateEvent();
		public EquippableItemComponent EquippedItem
		{
			get
			{
				return __equippedItem;
			}
			private set
			{
				if(value == __equippedItem)
				{
					return;
				}
				if (__equippedItem != null)
				{
					__equippedItem?.OnUnequip(Actor);
					OnInventoryUpdate.Invoke(Actor, eInventoryAction.UNEQUIP, __equippedItem.Item);
				}
				__equippedItem = value;
				if(__equippedItem != null)
				{
					__equippedItem?.OnEquip(Actor);
					OnInventoryUpdate.Invoke(Actor, eInventoryAction.EQUIP, __equippedItem.Item);
				}
			}
		}
		private EquippableItemComponent __equippedItem;
		public IReadOnlyCollection<Item> Inventory => GetComponentsInChildren<Item>(true);
		public Vector3 Position { get; private set; }
		public Quaternion Rotation { get; private set; }
		[StateMinMax(0, int.MaxValue)]
		public virtual int Credits { get; protected set; } = 100;
		public Transform InventoryContainer { get; private set; }

		protected override void Awake()
		{
			InventoryContainer = new GameObject($"{name}_Inventory").transform;
			InventoryContainer.SetParent(transform);
			base.Awake();
		}

		protected virtual void Update()
		{
			Position = transform.position;
			Rotation = transform.rotation;

			if(EquippedItem != null)
			{
				EquippedItem.OnEquippedThink(Actor);
			}
		}

		protected override void OnSaveDataLoaded()
		{
			transform.position = Position;
			transform.rotation = Rotation;
		}

		public void EquipItem(EquippableItemComponent equippableItem)
		{
			EquippedItem = equippableItem;
		}

		public void PickupItem(Item item, bool silent = false)
		{
			item.OnPickup(Actor);

			var rb = item.gameObject.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.Sleep();
				rb.detectCollisions = false;
			}

			if (EquippedItem == null  && (item.GetComponent<EquippableItemComponent>()?.EquipOnPickup ?? false))
			{
                item.GetComponent<EquippableItemComponent>().OnEquip(Actor);
			}
			else
			{
				item.transform.SetParent(transform);
			}
			item.gameObject.SetActive(false);
			OnInventoryUpdate.Invoke(Actor, silent ? eInventoryAction.REFRESH : eInventoryAction.PICKUP, item);
		}

		public void DropItem(Item item)
		{
			DropItem(item, transform.position, transform.rotation);
		}

		public void DropItem(Item item, Vector3 position, Quaternion rotation)
		{
			var equip = item.GetComponent<EquippableItemComponent>();
			if (equip && EquippedItem == equip)
			{
                equip.OnUnequip(Actor);
				EquippedItem = null;
			}

			item.gameObject.SetActive(true);
			item.transform.position = position;
			item.transform.rotation = rotation;
			item.transform.SetParent(null);
			item.OnDrop(Actor);

			var rb = item.gameObject.GetComponent<Rigidbody>();
			if (rb)
			{
				rb.WakeUp();
				rb.detectCollisions = true;
			}

			OnInventoryUpdate.Invoke(Actor, eInventoryAction.DROP, item);
		}
	}
}