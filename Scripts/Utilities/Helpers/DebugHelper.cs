using FPTemplate.Utilities.Extensions;
using FPTemplate.Utilities.Maths;
using FPTemplate.World.Portals;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPTemplate.Utilities.Helpers
{
    public static class DebugHelper
    {
        public static void DrawScreenRect(Camera camera, Rect rect, Color color, float duration = 0)
        {
            var topLeft = camera.ScreenToWorldPoint(new Vector3(rect.xMin, rect.yMin, camera.nearClipPlane * 1.1f));
            var topRight = camera.ScreenToWorldPoint(new Vector3(rect.xMax, rect.yMin, camera.nearClipPlane * 1.1f));
            var bottomRight = camera.ScreenToWorldPoint(new Vector3(rect.xMax, rect.yMax, camera.nearClipPlane * 1.1f));
            var bottomLeft = camera.ScreenToWorldPoint(new Vector3(rect.xMin, rect.yMax, camera.nearClipPlane * 1.1f));

            Debug.DrawLine(topLeft, topRight, color, duration);
            Debug.DrawLine(topRight, bottomRight, color, duration);
            Debug.DrawLine(bottomRight, bottomLeft, color, duration);
            Debug.DrawLine(bottomLeft, topLeft, color, duration);
        }

        public static void DrawScreenLine(Camera camera, Vector2 start, Vector2 end, Color color, float duration = 0)
        {
            Vector3 startVec = camera.ScreenToWorldPoint(new Vector3(start.x, start.y, 0));
            Vector3 endVec = camera.ScreenToWorldPoint(new Vector3(end.x, end.y, 0));

            Debug.DrawLine(startVec, endVec, color, duration);
        }

        public static void DrawScreenPoint(Camera camera, Vector2 point, Color color, float duration = 0, float size = 5)
        {
            Vector3 pointVec = camera.ScreenToWorldPoint(new Vector3(point.x, point.y, camera.nearClipPlane * 1.1f));
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;

            Debug.DrawLine(pointVec - up, pointVec + up, color, duration);
            Debug.DrawLine(pointVec - right, pointVec + right, color, duration);
        }

        public static void DrawPoint(Vector3 origin, float size, Color color, float duration)
        {
#if !UNITY_EDITOR
        return;
#else
            DrawCube(origin, Vector3.one * size, Quaternion.identity, color, duration);
#endif
        }

        public static void DrawCube(Vector3 origin, Vector3 extents, Quaternion rotation, Color color, float duration)
        {
#if !UNITY_EDITOR
        return;
#else
            var verts = new Vector3[]
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
            origin + rotation*new Vector3(-extents.x, -extents.y, -extents.z),
            };

            // top square
            Debug.DrawLine(verts[0], verts[2], color, duration);
            Debug.DrawLine(verts[1], verts[3], color, duration);
            Debug.DrawLine(verts[1], verts[0], color, duration);
            Debug.DrawLine(verts[2], verts[3], color, duration);

            // bottom square
            Debug.DrawLine(verts[4], verts[6], color, duration);
            Debug.DrawLine(verts[5], verts[7], color, duration);
            Debug.DrawLine(verts[5], verts[4], color, duration);
            Debug.DrawLine(verts[6], verts[7], color, duration);

            // connections
            Debug.DrawLine(verts[0], verts[4], color, duration);
            Debug.DrawLine(verts[1], verts[5], color, duration);
            Debug.DrawLine(verts[2], verts[6], color, duration);
            Debug.DrawLine(verts[3], verts[7], color, duration);
#endif
        }

        public static void DrawCapsule(Vector3 start, Vector3 end, float radius, Quaternion rotation, Color color, float duration)
        {
#if !UNITY_EDITOR
        return;
#else
            // TODO - top cap slightly overdraws semi-circle (ie. greater than 180 degrees), investigate
            // Draw top cap
            DrawCircle(start, radius, rotation, 0, Mathf.PI, color, duration);
            DrawCircle(start, radius, rotation * Quaternion.LookRotation(Vector3.right), 0, Mathf.PI, color, duration);
            DrawCircle(start, radius, rotation * Quaternion.LookRotation(Vector3.up), color, duration);
            // Draw bottom cap
            DrawCircle(end, radius, rotation, Mathf.PI + 0.01f, Mathf.PI * 2, color, duration);
            DrawCircle(end, radius, rotation * Quaternion.LookRotation(Vector3.right), Mathf.PI + 0.01f, Mathf.PI * 2, color, duration);
            DrawCircle(end, radius, rotation * Quaternion.LookRotation(Vector3.up), color, duration);
            // Draw connectors
            Debug.DrawLine(start + rotation * (Vector3.right * radius), end + rotation * (Vector3.right * radius), color, duration);
            Debug.DrawLine(start + rotation * (-Vector3.right * radius), end + rotation * (-Vector3.right * radius), color, duration);
            Debug.DrawLine(start + rotation * (Vector3.forward * radius), end + rotation * (Vector3.forward * radius), color, duration);
            Debug.DrawLine(start + rotation * (-Vector3.forward * radius), end + rotation * (-Vector3.forward * radius), color, duration);
#endif
        }

        public static void DrawSphere(Vector3 origin, Quaternion rotation, float radius, Color color, float duration)
        {
#if !UNITY_EDITOR
        return;
#else
            // Draw top cap
            DrawCircle(origin, radius, rotation, color, duration);
            DrawCircle(origin, radius, rotation * Quaternion.LookRotation(Vector3.right), color, duration);
            DrawCircle(origin, radius, rotation * Quaternion.LookRotation(Vector3.up), color, duration);
#endif
        }

        public static void DrawCircle(Vector3 origin, float radius, Quaternion rotation, Color color, float duration)
        {
#if !UNITY_EDITOR
        return;
#else
            DrawCircle(origin, radius, rotation, float.MinValue, float.MaxValue, color, duration);
#endif
        }

        public static void DrawCircle(Vector3 origin, float radius, Quaternion rotation, float startAngle, float endAngle, Color color, float duration)
        {
#if !UNITY_EDITOR
        return;
#else
            float resolution = 24;
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
                        Debug.DrawLine(lastPoint, thisPoint, color, duration);
                    }
                }
                lastPoint = thisPoint;
            }
#endif
        }

        static Queue<Action> _queuedActions = new Queue<Action>();
        public static void QueueForMainThread(Action action)
        {
            lock (_queuedActions)
            {
                _queuedActions.Enqueue(action);
            }
        }

        public static void PurgeQueuedActions()
        {
            lock (_queuedActions)
            {
                while (_queuedActions.Count > 0)
                {
                    _queuedActions.Dequeue().Invoke();
                }
            }
        }
    }
}