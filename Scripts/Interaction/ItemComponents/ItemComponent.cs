using FPTemplate.Actors;
using FPTemplate.Utilities;
using System.Collections.Generic;

namespace FPTemplate.Interaction.Items
{
    public abstract class ItemComponent : ExtendedMonoBehaviour
    {
        public Item Item => gameObject.GetComponent<Item>();

        public virtual IEnumerable<ActorAction> GetActions(Actor actor)
        {
            yield break;
        }

        public virtual void OnDrop(Actor actor)
        {
        }

        public virtual void OnPickup(Actor actor)
        {
        }

        public virtual bool ReceiveAction(Actor actor, ActorAction action)
        {
            return false;
        }
    }
}