using FPTemplate.Interaction;
using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace FPTemplate.Actors
{
    [RequireComponent(typeof(ActorState))]
    public class Actor : SlowUpdater
    {
        // Adapters
        public ILookAdapter LookAdapter { get; protected set; }
        public IMovementController MovementController { get; private set; }
        public ActorPerceiver Perceiver { get; protected set; }

        public Transform EquippedItemTransform;

        /// <summary>
        /// The interactable item that the player is currently focused on, i.e. in the crosshairs
        /// </summary>
        public Interactable FocusedInteractable { get; protected set; }
        public List<Interactable> Interactables { get; private set; } = new List<Interactable>();
        public Animator Animator { get; private set; }
        public ActorState State => GetComponent<ActorState>();
        public virtual string DisplayName => ActorName;
        public RaycastHit? LastRaycast { get; protected set; }

        public LayerMask InteractionMask;
        public string ActorName = "Unnamed Entity";

        protected virtual void Awake()
        {
            MovementController = gameObject.GetComponentByInterfaceInChildren<IMovementController>();
            LookAdapter = gameObject.GetComponentByInterfaceInChildren<ILookAdapter>();
            Animator = GetComponentInChildren<Animator>(true);
            Perceiver = gameObject.GetOrAddComponent<ActorPerceiver>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.GetComponent<Interactable>() ?? other.GetComponentInParent<Interactable>();
            if (!interactable || Interactables.Contains(interactable))
            {
                return;
            }
            Interactables.Add(interactable);
            interactable.EnterAttention(this);
        }

        private void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponent<Interactable>() ?? other.GetComponentInParent<Interactable>();
            if (!interactable)
            {
                return;
            }
            Interactables.Remove(interactable);
            interactable.ExitAttention(this);
        }

        protected override int TickOnThread(float dt)
        {
            return 0;
        }
    }
}