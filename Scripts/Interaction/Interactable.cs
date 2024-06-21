using FPTemplate.Actors;
using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPTemplate.Interaction
{
    public abstract class Interactable : SlowUpdater
    {
        public const int INTERACTION_LAYER = 9;
        public InteractableSettings InteractionSettings = new InteractableSettings();

        public virtual IEnumerable<ActorAction> GetActions(Actor context)
        {
            if (!CanUse(context))
            {
                yield break;
            }
            yield return new ActorAction(eActionKey.USE, "Use", gameObject);
        }

        public virtual bool CanUse(Actor context)
        {
            Vector3? closestColliderPosition = null;
            float bestDistance = float.MaxValue;
            foreach (var coll in InteractionSettings.Colliders)
            {
                var closestPoint = coll.ClosestPoint(context.transform.position);
                var dist = Vector3.Distance(context.transform.position, closestPoint);
                if (dist < bestDistance)
                {
                    closestColliderPosition = closestPoint;
                    bestDistance = dist;
                }
            }
            if (closestColliderPosition != null)
            {
                return bestDistance < InteractionSettings.MaxUseDistance;
            }
            var distance = Vector3.Distance(context.transform.position, transform.position);
            return distance <= InteractionSettings.MaxUseDistance;
        }

        private void OnValidate()
        {
            if (InteractionSettings?.Renderers == null)
            {
                return;
            }
            foreach (var r in InteractionSettings?.Renderers)
            {
                if (r != null && r.Renderer)
                {
                    var mf = r.Renderer.GetComponent<MeshFilter>();
                    if (!mf)
                    {
                        continue;
                    }
                    r.Mesh = mf.sharedMesh;
                }
            }
        }

        [ContextMenu("Collect Colliders & Renderers")]
        public void CollectCollidersAndRenderers()
        {
            if (InteractionSettings.Renderers == null || !InteractionSettings.Renderers.Any())
            {
                InteractionSettings.Renderers = GetComponentsInChildren<MeshRenderer>()

                    .Select(r => new InteractableSettings.InteractionRenderer
                    {
                        Mesh = r.GetComponent<MeshFilter>()?.sharedMesh,
                        Renderer = r
                    })
                    .Where(c => c.Renderer.enabled && c.Mesh)
                    .ToList();
            }
            if (InteractionSettings.Colliders == null || !InteractionSettings.Colliders.Any())
            {
                InteractionSettings.Colliders = new List<Collider>(GetComponentsInChildren<Collider>().Where(c => c.enabled));
            }
            this.TrySetDirty();
        }

        protected virtual void Start() { }

        public Bounds Bounds
        {
            get
            {
                if (InteractionSettings.Colliders == null || InteractionSettings.Colliders.Count == 0)
                {
                    return default;
                }
                var bounds = InteractionSettings.Colliders[0].bounds;
                for (int i = 1; i < InteractionSettings.Colliders.Count; i++)
                {
                    bounds.Encapsulate(InteractionSettings.Colliders[i].bounds);
                }
                return bounds;
            }
        }

        public abstract string DisplayName { get; }

        public Mesh GetInteractionMesh()
        {
            var thisFilter = GetComponent<MeshFilter>() ?? GetComponentInChildren<MeshFilter>();
            if (thisFilter)
            {
                return thisFilter.sharedMesh;
            }
            return null;
        }

        public virtual void ExitFocus(Actor actor)
        {
            InteractionSettings.OnFocusExit.Invoke(actor);
        }

        public virtual void EnterFocus(Actor actor)
        {
            InteractionSettings.OnFocusEnter.Invoke(actor);
        }

        public virtual void ReceiveAction(Actor actor, ActorAction action)
        {
            if (action.State == eActionState.End && action.Key == eActionKey.USE)
            {
                InteractionSettings.OnUsed.Invoke(actor, action);
            }
        }

        public virtual void ExitAttention(Actor actor)
        {
            InteractionSettings.OnExitAttention.Invoke(actor);
        }

        public virtual void EnterAttention(Actor actor)
        {
            InteractionSettings.OnEnterAttention.Invoke(actor);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            var b = Bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }

        public InteractableSettings GetSettings() => InteractionSettings;

        public Bounds GetBounds() => Bounds;
    }
}