using FPTemplate.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace FPTemplate.World.Portals
{
    public static class PortalUtilities
    {
        private static Canvas m_debugCanvas;
        public static void DebugScreenRect(PortalRenderer renderer, PortalRenderer parent)
        {
            if (!m_debugCanvas)
            {
                m_debugCanvas = new GameObject("PortalDebug").AddComponent<Canvas>();
                m_debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                m_debugCanvas.gameObject.AddComponent<CanvasScaler>();
                var rt = m_debugCanvas.GetComponent<RectTransform>();
                rt.pivot = Vector2.zero;
            }
            var shouldRender = renderer.ShouldRender(parent, renderer.Bounds, out var screenRect);
            var name = $"sr_{renderer.name}_{parent?.name ?? "root"}";
            var img = m_debugCanvas.transform.Find(name)?.GetComponent<Image>();
            if(img == null)
            {
                img = new GameObject(name).AddComponent<Image>();
                img.transform.SetParent(m_debugCanvas.transform, false);
                img.rectTransform.pivot = Vector2.zero;
                img.rectTransform.anchorMax = Vector2.zero;
                img.rectTransform.anchorMin = Vector2.zero;
            }
            img.color = (shouldRender ? Color.green : Color.red).WithAlpha(.02f);
            img.rectTransform.sizeDelta = screenRect.size;
            img.rectTransform.anchoredPosition = screenRect.position;
        }

        public static Matrix4x4 GetReflectionMatrix(Vector3 planeNormal)
        {
            // Ensure the normal is normalized
            planeNormal.Normalize();
            float a = planeNormal.x;
            float b = planeNormal.y;
            float c = planeNormal.z;

            // Reflection matrix across the plane, modified to preserve the y-axis
            Matrix4x4 reflectionMatrix = new Matrix4x4(
                new Vector4(1 - 2 * a * a, 0, -2 * a * c, 0),
                new Vector4(-2 * a * b, 1 - 2 * b * b, -2 * b * c, 0),
                new Vector4(-2 * a * c, 0, 1 - 2 * c * c, 0),
                new Vector4(0, 0, 0, 1)
            );

            // Create a rotation matrix to counter the mirroring effect
            // This is a 180-degree rotation around the axis perpendicular to the plane normal
            Quaternion rotationCorrection = Quaternion.AngleAxis(180, planeNormal);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotationCorrection);

            // Combine the reflection and the rotation matrices
            return rotationMatrix * reflectionMatrix;
        }

        public static Matrix4x4 GetPortalMatrix(Transform inTransform, Transform outTransform, PortalConfiguration config)
        {
            var inPos = inTransform.position;
            var inRot = inTransform.rotation;
            var outPos = outTransform.position;
            var outRot = outTransform.rotation;

            var trs = Matrix4x4.Rotate(Quaternion.Inverse(inRot));
            trs *= Matrix4x4.Translate(-inPos);
            // In local space now

            var reflectionMatrix = GetReflectionMatrix(config.Normal);
            trs = reflectionMatrix * trs;
            trs = new Matrix4x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, -1, 0, 0), // Negate the y-axis
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1)
            ) * trs;
            //trs = Matrix4x4.Rotate(new Vector3(position.x, position.y, -position.z)) * trs;
            //trs = Matrix4x4.Translate(reflectedPosition) * trs;

            trs = Matrix4x4.Rotate(outRot) * trs;
            trs = Matrix4x4.Translate(outPos) * trs;
            return trs;
        }
    }
}