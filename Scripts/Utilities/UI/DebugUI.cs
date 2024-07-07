using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPTemplate.Utilities
{
    public class DebugUI : Singleton<DebugUI>
    {
        private Canvas m_debugCanvas;
        private Dictionary<string, TextMeshProUGUI> m_texts = new Dictionary<string, TextMeshProUGUI>();

        public void DebugLabel(string id, string text, Vector2 pos, Color color)
        {
            SetupDebugCanvas();

            if (!m_texts.TryGetValue(id, out var textMesh))
            {
                textMesh = new GameObject("debugText").AddComponent<TextMeshProUGUI>();
                textMesh.fontSize = 12;
                textMesh.verticalAlignment = VerticalAlignmentOptions.Bottom;
                var rt = textMesh.rectTransform;
                rt.SetParent(m_debugCanvas.transform);
                rt.pivot = Vector2.zero;
                rt.anchorMax = Vector2.zero;
                rt.anchorMin = Vector2.zero;
                m_texts[id] = textMesh;
            }

            textMesh.rectTransform.anchoredPosition = pos;
            textMesh.text = text;
            textMesh.color = color;
        }

        private void SetupDebugCanvas()
        {
            if (!m_debugCanvas)
            {
                m_debugCanvas = new GameObject("PortalDebug").AddComponent<Canvas>();
                m_debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                m_debugCanvas.gameObject.AddComponent<CanvasScaler>();
                var rt = m_debugCanvas.GetComponent<RectTransform>();
                rt.pivot = Vector2.zero;
            }
        }
    }
}