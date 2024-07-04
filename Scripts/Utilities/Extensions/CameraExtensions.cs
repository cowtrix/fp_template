using FPTemplate.Utilities.Maths;
using UnityEngine;

namespace FPTemplate.Utilities.Extensions
{
    public static class CameraExtensions
    {
        public static bool IsPointBehindCamera(this Camera camera, Vector3 point)
        {
            // Get the direction from the camera to the point
            Vector3 directionToPoint = point - camera.transform.position;

            // Get the camera's forward direction
            Vector3 cameraForward = camera.transform.forward;

            // Calculate the dot product
            float dotProduct = Vector3.Dot(directionToPoint, cameraForward);

            // If the dot product is negative, the point is behind the camera
            return dotProduct < 0;
        }

        public static Vector3 ClosestPointOnViewFrustum(this Camera camera, Vector3 point)
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            Vector3 closestPoint = point;
            float minDistance = float.MaxValue;

            foreach (Plane plane in frustumPlanes)
            {
                Vector3 pointOnPlane = plane.ClosestPointOnPlane(point);
                float distance = Vector3.Distance(point, pointOnPlane);
                if (distance < minDistance)
                {
                    closestPoint = pointOnPlane;
                    minDistance = distance;
                }
            }

            return closestPoint;
        }

        public static Rect WorldBoundsToScreenRect(this RotationalBounds worldBounds, Camera camera)
        {
            if (!camera)
            {
                return default;
            }
            
            var screenRect = new Rect(camera.WorldToScreenPoint(worldBounds.center), Vector2.zero);
            foreach (var p in worldBounds.AllPoints())
            {
                var point = p;
                if (camera.IsPointBehindCamera(point))
                {
                    var oldPoint = point;
                    point = ClosestPointOnViewFrustum(camera, point);
                    Debug.DrawLine(point, oldPoint, Color.yellow, .1f);
                }
                var screenP = camera.WorldToScreenPoint(point);
                screenRect = screenRect.Encapsulate(screenP);
            }
            return screenRect;
        }

        public static Rect ScreenRectToViewportRect(this Rect rect) =>
            new Rect(rect.x / Screen.width, rect.y / Screen.height, rect.width / Screen.width, rect.height / Screen.height);

        public static bool ScreenRectIsOnScreen(this Rect rect) =>
            rect.Overlaps(new Rect(0, 0, Screen.width, Screen.height));

        public static Rect ClipToScreen(this Rect rect)
        {
            var xMin = Mathf.Max(0, rect.xMin);
            var yMin = Mathf.Max(0, rect.yMin);

            var xMax = Mathf.Min(Screen.width, rect.xMax);
            var yMax = Mathf.Min(Screen.height, rect.yMax);

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public static bool IsLineInFrustum(this Camera camera, Vector3 pointA, Vector3 pointB)
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);

            // Check if either point is inside the frustum
            if (camera.IsPointInCameraFrustum(pointA) || camera.IsPointInCameraFrustum(pointB))
            {
                return true;
            }

            // Check if the line intersects any of the frustum planes
            foreach (Plane plane in frustumPlanes)
            {
                if (plane.GetSide(pointA) != plane.GetSide(pointB))
                {
                    var closestPointA = plane.ClosestPointOnPlane(pointA);
                    var closestPointB = plane.ClosestPointOnPlane(pointB);
                    if(camera.IsPointInCameraFrustum(closestPointA) || camera.IsPointInCameraFrustum(closestPointB))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool BoundsWithinFrustrum(this Camera camera, RotationalBounds worldBounds)
        {
            foreach (var p in worldBounds.AllPoints())
            {
                if (camera.IsPointInCameraFrustum(p))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsPointInCameraFrustum(this Camera camera, Vector3 point)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(point);
            return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                   viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                   viewportPoint.z >= camera.nearClipPlane && viewportPoint.z <= camera.farClipPlane;
        }

        public static Matrix4x4 GetWorldToViewportMatrix(Camera camera, Matrix4x4 trsMatrix)
        {
            // Get the camera's view matrix (world to camera space)
            Matrix4x4 viewMatrix = camera.worldToCameraMatrix;

            // Get the camera's projection matrix (camera space to clip space)
            Matrix4x4 projectionMatrix = camera.nonJitteredProjectionMatrix;

            // Apply the TRS matrix to the world positions before transforming to camera space
            Matrix4x4 worldToCameraMatrix = viewMatrix * trsMatrix;

            // Combine the modified world-to-camera matrix with the projection matrix
            Matrix4x4 worldToClipMatrix = projectionMatrix * worldToCameraMatrix;

            return worldToClipMatrix;
        }

        public static Matrix4x4 GetProjectionMatrix(this Camera camera)
        {
            float fovRad = camera.fieldOfView * Mathf.Deg2Rad;
            float near = camera.nearClipPlane;
            float far = camera.farClipPlane;
            float aspect = camera.aspect;

            float yScale = 1.0f / Mathf.Tan(fovRad / 2.0f);
            float xScale = yScale / aspect;

            Matrix4x4 projectionMatrix = new Matrix4x4();

            projectionMatrix[0, 0] = xScale;
            projectionMatrix[0, 1] = 0;
            projectionMatrix[0, 2] = 0;
            projectionMatrix[0, 3] = 0;

            projectionMatrix[1, 0] = 0;
            projectionMatrix[1, 1] = yScale;
            projectionMatrix[1, 2] = 0;
            projectionMatrix[1, 3] = 0;

            projectionMatrix[2, 0] = 0;
            projectionMatrix[2, 1] = 0;
            projectionMatrix[2, 2] = -(far + near) / (far - near);
            projectionMatrix[2, 3] = -(2 * far * near) / (far - near);

            projectionMatrix[3, 0] = 0;
            projectionMatrix[3, 1] = 0;
            projectionMatrix[3, 2] = -1;
            projectionMatrix[3, 3] = 0;

            return projectionMatrix;
        }
    }
}