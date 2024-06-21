using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using FPTemplate.Interaction;
using FPTemplate.Actors;
using FPTemplate.Actors.Player;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace UI
{
    [Serializable]
    public class StringEvent : UnityEvent<string> { }

    public class HUDManager : Singleton<HUDManager>
    {
        public const int OUTLINE_LAYER = 11;

        public TextMeshProUGUI ActionLabel;
        public RectTransform FocusSprite;
        public CanvasGroup FocusCanvasGroup;
        private Camera Camera => CameraController.GetComponent<Camera>();
        public CameraController CameraController => CameraController.Instance;
        public PlayerActor PlayerActor;
        public Material InteractionMaterial;
        public StringEvent FocusedInteractableDisplayName;
        public UICircle Interact, Perceive;

        private List<TextMeshProUGUI> m_labels = new List<TextMeshProUGUI>();
        private List<MeshRenderer> m_interactionOutlineRenderers = new List<MeshRenderer>();
        private GameObject m_outlineContainer;

        private void Start()
        {
            ActionLabel.gameObject.SetActive(false);
            m_outlineContainer = new GameObject("OutlineContainer");
        }

        private void SetFocusDisplay(Interactable interactable)
        {
            FocusSprite.gameObject.SetActive(true);
            m_outlineContainer.SetActive(true);
            var settings = interactable.GetSettings();
            if (!m_interactionOutlineRenderers.SequenceEqual(settings.Renderers.Select(r => r.Renderer)))
            {
                for (var i = 0; i < settings.Renderers.Count; i++)
                {
                    var sourceRenderer = settings.Renderers[i];
                    if (!sourceRenderer.Renderer)
                    {
                        continue;
                    }
                    MeshRenderer targetRenderer = null;
                    MeshFilter meshfilter;
                    if (m_interactionOutlineRenderers.Count <= i)
                    {
                        targetRenderer = new GameObject($"OutlineRenderer_{i}").AddComponent<MeshRenderer>();
                        targetRenderer.gameObject.layer = OUTLINE_LAYER;
                        targetRenderer.transform.SetParent(m_outlineContainer.transform, false);
                        meshfilter = targetRenderer.gameObject.AddComponent<MeshFilter>();
                        m_interactionOutlineRenderers.Add(targetRenderer);
                    }
                    else
                    {
                        targetRenderer = m_interactionOutlineRenderers[i];
                        meshfilter = targetRenderer.GetComponent<MeshFilter>();
                    }
                    targetRenderer.sharedMaterial = InteractionMaterial;
                    meshfilter.sharedMesh = sourceRenderer.Mesh;
                    targetRenderer.transform.position = sourceRenderer.Renderer.transform.position;
                    targetRenderer.transform.rotation = sourceRenderer.Renderer.transform.rotation;
                    targetRenderer.transform.localScale = sourceRenderer.Renderer.transform.lossyScale;
                }

                for (var i = m_interactionOutlineRenderers.Count - 1; i >= settings.Renderers.Count; i--)
                {
                    var toDestroy = m_interactionOutlineRenderers[i];
                    m_interactionOutlineRenderers.RemoveAt(i);
                    toDestroy.gameObject.SafeDestroy();
                }
                if (m_interactionOutlineRenderers.Any())
                {
                    var center = m_interactionOutlineRenderers.First().transform.position;
                    foreach (var pos in m_interactionOutlineRenderers.Skip(1).Select(r => r.transform.position))
                    {
                        center += pos;
                    }
                    center /= (float)m_interactionOutlineRenderers.Count;
                    InteractionMaterial.SetVector("_WorldCenter", center);
                }
            }

            var screenRect = new Rect(Camera.WorldToScreenPoint(interactable.transform.position), Vector2.zero);
            Bounds objBounds = interactable.GetBounds();
            foreach (var p in objBounds.AllPoints())
            {
                screenRect = screenRect.Encapsulate(Camera.WorldToScreenPoint(p));
            }
            const float nameMargin = 0f;
            if (screenRect.yMin < nameMargin)
            {
                screenRect.yMin = nameMargin;
            }
            FocusSprite.position = screenRect.center;
            FocusSprite.sizeDelta = screenRect.size;

            var distance = PlayerActor.DistanceToFocusedInteractable;
            Interact.enabled = true;
            Perceive.enabled = true;

            var distanceToInteractable = distance - settings.MaxUseDistance;
            var t = 1 - (distanceToInteractable / (settings.MaxFocusDistance - settings.MaxUseDistance));
            if (interactable.CanUse(PlayerActor))
            {
                Perceive.rectTransform.sizeDelta = Interact.rectTransform.sizeDelta;
            }
            else
            {
                Perceive.rectTransform.sizeDelta = Interact.rectTransform.sizeDelta * (1 + distanceToInteractable);
                Perceive.color = Perceive.color.WithAlpha(t);
            }

            FocusCanvasGroup.alpha = Mathf.Clamp01(t * t);
        }

        private void ClearFocusDisplay()
        {
            Interact.enabled = false;
            Perceive.enabled = false;
            m_outlineContainer.SetActive(false);
            FocusSprite.gameObject.SetActive(false);
            FocusedInteractableDisplayName.Invoke("");
        }

        private void Update()
        {
            var interactable = PlayerActor?.FocusedInteractable;
            if (interactable != null)
            {
                if (interactable != PlayerActor.State.EquippedItem)
                {
                    SetFocusDisplay(interactable);
                }
                else
                {
                    ClearFocusDisplay();
                }

                FocusedInteractableDisplayName.Invoke(interactable.DisplayName);
            }
            else
            {
                ClearFocusDisplay();
            }

            if (interactable != null && interactable.CanUse(PlayerActor))
            {
                int actionIndex = 0;
                foreach (var action in interactable.GetActions(PlayerActor))
                {
                    if (string.IsNullOrEmpty(action.Description))
                    {
                        // Hidden action
                        continue;
                    }
                    TextMeshProUGUI label;
                    if (m_labels.Count <= actionIndex)
                    {
                        label = Instantiate(ActionLabel.gameObject).GetComponent<TextMeshProUGUI>();
                        label.transform.SetParent(ActionLabel.transform.parent);
                        m_labels.Add(label);
                    }
                    else
                    {
                        label = m_labels[actionIndex];
                    }
                    label.gameObject.SetActive(true);
                    label.text = action.ToString();
                    actionIndex++;
                }
                for (var i = actionIndex; i < m_labels.Count; ++i)
                {
                    m_labels[i].gameObject.SetActive(false);
                }

            }
            else
            {
                //Icon.sprite = null;
                foreach (var label in m_labels)
                {
                    label.gameObject.SetActive(false);
                }
            }
            //Icon.gameObject.SetActive(Icon.sprite);
        }
    }
}