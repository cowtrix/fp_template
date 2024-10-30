using FPTemplate.Utilities.Maths;
using UnityEngine;

namespace FPTemplate.Utilities.Extensions
{
    public static class GizmoExtensions
    {
        public static void Label(Vector3 position, string label)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.Label(position, label);
#endif
        }

        public static void DrawArrow(Vector3 start, Vector3 end, Color color, float size)
        {
            Gizmos.color = color;

            var delta = end - start;
            var up = Vector3.up * size;
            var p1 = start + delta * .75f;

            Gizmos.DrawLine(start + up * .5f, start - up * .5f);
            Gizmos.DrawLine(start + up * .5f, p1 + up * .5f);
            Gizmos.DrawLine(start - up * .5f, p1 - up * .5f);
            Gizmos.DrawLine(p1 + up * .5f, p1 + up);
            Gizmos.DrawLine(p1 - up * .5f, p1 - up);

            Gizmos.DrawLine(p1 + up, end);
            Gizmos.DrawLine(p1 - up, end);
        }


        public static void DrawWireCube(Vector3 origin, Vector3 extents, Quaternion rotation, Color color)
        {
            var verts = new[]
            {
            // Top square
            origin + rotation*new Vector3(extents.x, extents.y, extents.z),
            origin + rotation*new Vector3(-extents.x, extents.y, extents.z),
            origin + rotation*new Vector3(extents.x, extents.y, -extents.z),
            origin + rotation*new Vector3(-extents.x, extents.y, -extents.z),

            // Bottom square
            origin + rotation*new Vector3(extents.x, -extents.y, extents.z),
            origin + rotation*new Vector3(-extents.x, -extents.y, extents.z),
            origin + rotation*new Vector3(extents.x, -extents.y, -extents.z),
            origin + rotation*new Vector3(-extents.x, -extents.y, -extents.z)
        };

            Gizmos.color = color;

            // top square
            Gizmos.DrawLine(verts[0], verts[2]);
            Gizmos.DrawLine(verts[1], verts[3]);
            Gizmos.DrawLine(verts[1], verts[0]);
            Gizmos.DrawLine(verts[2], verts[3]);

            // bottom square
            Gizmos.DrawLine(verts[4], verts[6]);
            Gizmos.DrawLine(verts[5], verts[7]);
            Gizmos.DrawLine(verts[5], verts[4]);
            Gizmos.DrawLine(verts[6], verts[7]);

            // connections
            Gizmos.DrawLine(verts[0], verts[4]);
            Gizmos.DrawLine(verts[1], verts[5]);
            Gizmos.DrawLine(verts[2], verts[6]);
            Gizmos.DrawLine(verts[3], verts[7]);

            Gizmos.color = Color.white;
        }

        public static void DrawCapsule(Vector3 start, Vector3 end, float radius, Quaternion rotation, Color color)
        {
            // TODO - top cap slightly overdraws semi-circle (ie. greater than 180 degrees), investigate
            // Draw top cap
            DrawCircle(start, radius, rotation, 0, Mathf.PI, color);
            DrawCircle(start, radius, rotation * Quaternion.LookRotation(Vector3.right), 0, Mathf.PI, color);
            DrawCircle(start, radius, rotation * Quaternion.LookRotation(Vector3.up), color);
            // Draw bottom cap
            DrawCircle(end, radius, rotation, Mathf.PI + 0.01f, Mathf.PI * 2, color);
            DrawCircle(end, radius, rotation * Quaternion.LookRotation(Vector3.right), Mathf.PI + 0.01f, Mathf.PI * 2, color);
            DrawCircle(end, radius, rotation * Quaternion.LookRotation(Vector3.up), color);
            // Draw connectors

            Gizmos.color = color;
            Gizmos.DrawLine(start + rotation * (Vector3.right * radius), end + rotation * (Vector3.right * radius));
            Gizmos.DrawLine(start + rotation * (-Vector3.right * radius), end + rotation * (-Vector3.right * radius));
            Gizmos.DrawLine(start + rotation * (Vector3.forward * radius), end + rotation * (Vector3.forward * radius));
            Gizmos.DrawLine(start + rotation * (-Vector3.forward * radius), end + rotation * (-Vector3.forward * radius));
            Gizmos.color = Color.white;
        }

        public static void DrawSphere(Vector3 origin, Quaternion rotation, float radius, Color color)
        {
#if !UNITY_EDITOR
        return;
#else
            // Draw top cap
            DrawCircle(origin, radius, rotation, color);
            DrawCircle(origin, radius, rotation * Quaternion.LookRotation(Vector3.right), color);
            DrawCircle(origin, radius, rotation * Quaternion.LookRotation(Vector3.up), color);
#endif
        }

        public static void DrawCircle(Vector3 origin, float radius, Quaternion rotation, Color color, float resolution = 24)
        {
#if !UNITY_EDITOR
        return;
#else
            DrawCircle(origin, radius, rotation, float.MinValue, float.MaxValue, color, resolution);
#endif
        }

        public static void DrawCone(Vector3 origin, Vector3 normal, float angle, float distance, Color color, float resolution = 24)
        {
            var radius = Mathf.Tan(angle) * distance;
            var rotation = Quaternion.LookRotation(normal);
            DrawCircle(origin + normal * distance, radius, rotation, color, resolution: resolution);
            Gizmos.color = color;
            for (var i = 0; i <= resolution; ++i)
            {
                float a = (i / resolution) * Mathf.PI * 2;
                float x = Mathf.Cos(a);
                float y = Mathf.Sin(a);
                var thisPoint = new Vector3(x, y, 0);
                thisPoint = origin + normal * distance + rotation * (thisPoint * radius);
                Gizmos.DrawLine(origin, thisPoint);
            }
            Gizmos.color = Color.white;
        }

        public static void DrawCircle(Vector3 origin, float radius, Quaternion rotation, float startAngle, float endAngle, Color color, float resolution = 24)
        {
#if !UNITY_EDITOR
        return;
#else
            Gizmos.color = color;
            Vector3 lastPoint = Vector3.zero;
            for (var i = 0; i <= resolution; ++i)
            {
                float angle = (i / resolution) * Mathf.PI * 2;

                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);
                var thisPoint = new Vector3(x, y, 0);
                thisPoint = origin + rotation * (thisPoint * radius);
                if (i > 0)
                {
                    if (Mathfx.BetweenInclusive(angle, startAngle, endAngle))
                    {
                        Gizmos.DrawLine(lastPoint, thisPoint);
                    }
                }
                lastPoint = thisPoint;
            }
            Gizmos.color = Color.white;
#endif
        }
    }
}