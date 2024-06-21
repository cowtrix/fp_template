using FPTemplate.Actors;
using FPTemplate.Utilities.Extensions;
using FPTemplate.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FPTemplate.Interaction.Items
{
    public sealed class Item : Interactable
    {
        [Serializable]
        public struct IconParameters
        {
            public Texture2D Texture;
            public Vector3 Offset, FocusOffset;
        }
        public IEnumerable<ItemComponent> Components => gameObject.GetComponentsByInterface<ItemComponent>();
        public Rigidbody Rigidbody => GetComponent<Rigidbody>();
        public override string DisplayName => ItemName;
        public const int IconSize = 64;

        public string ItemName = "Unknown Item";
        public string Description = "An object of mysterious origins.";
        public IconParameters Icon;

        private int m_layer;
        private bool m_isKinematic;

        protected override void Start()
        {
            m_layer = gameObject.layer;
            var rb = Rigidbody;
            if (rb)
            {
                m_isKinematic = rb.isKinematic;
            }
            base.Start();
        }

        [ContextMenu("Generate Icon")]
        public void GenerateIcon()
        {
            var layer = gameObject.layer;
            transform.SetLayerRecursive(31);
            var rt = RenderTexture.GetTemporary(IconSize, IconSize, 16, RenderTextureFormat.ARGB32);
            var cam = new GameObject().AddComponent<Camera>();
            cam.targetTexture = rt;
            cam.transform.position = transform.position + transform.localToWorldMatrix.MultiplyVector(Icon.Offset);
            cam.transform.LookAt(transform.position + transform.localToWorldMatrix.MultiplyVector(Icon.FocusOffset));
            cam.cullingMask = 1 << 31;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.clear;
            cam.nearClipPlane = .01f;
            var ambientSettings = AmbientLightingSettings.Current;
            new AmbientLightingSettings
            {
                ambientMode = UnityEngine.Rendering.AmbientMode.Flat,
                ambientSkyColor = new Color(.8f, .8f, .8f),
            }.Apply();

            cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(IconSize, IconSize);
            tex.ReadPixels(new Rect(0, 0, IconSize, IconSize), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            File.WriteAllBytes($"Assets/Prefabs/Items/Resources/Icons/{name}_icon.png", tex.EncodeToPNG());
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            Icon.Texture = Resources.Load<Texture2D>($"Icons/{name}_icon");

            cam.targetTexture = null;
            cam.gameObject.SafeDestroy();
            transform.SetLayerRecursive(layer);
            RenderTexture.ReleaseTemporary(rt);
            this.TrySetDirty();
            ambientSettings.Apply();
        }

        public bool Implements<T>() where T : ItemComponent => Components.Any(c => c is T);

        public override IEnumerable<ActorAction> GetActions(Actor actor)
        {
            if (!CanUse(actor))
            {
                yield break;
            }
            foreach (var component in Components)
            {
                foreach (var action in component.GetActions(actor))
                {
                    yield return action;
                }
            }
        }

        public override void ReceiveAction(Actor actor, ActorAction action)
        {
            foreach (var component in Components)
            {
                if (component.ReceiveAction(actor, action))
                {
                    return;
                }
            }
            if (action.State == eActionState.End && action.Key == eActionKey.USE)
            {
                actor.State.PickupItem(this);
            }
            base.ReceiveAction(actor, action);
        }

        public void OnPickup(Actor actor)
        {
            var rb = Rigidbody;
            if (rb)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }

        public void OnDrop(Actor actor)
        {
            var rb = Rigidbody;
            if (rb)
            {
                rb.position = transform.position;
                rb.rotation = transform.rotation;
                rb.isKinematic = m_isKinematic;
                rb.detectCollisions = true;
            }
            gameObject.layer = m_layer;
            transform.SetParent(null, true);
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.matrix = transform.localToWorldMatrix;
            var origin = Icon.Offset;
            Gizmos.DrawCube(origin, Vector3.one * .1f);
            Gizmos.DrawLine(origin, Icon.FocusOffset);
        }
    }
}