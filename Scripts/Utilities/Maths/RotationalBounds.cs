using FPTemplate.Utilities.Extensions;
using System;
using UnityEngine;

namespace FPTemplate.Utilities.Maths
{
    [Serializable]
    public struct RotationalBounds
    {
        public Quaternion rotation;
        public Vector3 center;
        public Vector3 extents;

        public RotationalBounds(Vector3 center, Vector3 size, Quaternion rotation)
        {
            this.rotation = rotation;
            this.center = center;
            this.extents = size / 2;
        }

        public bool Overlaps(RotationalBounds b)
        {
            return Geometry3D.RectangularPrismOverlaps(center, extents, rotation, b.center, b.extents, b.rotation);
        }

        public Bounds GetAxisBounds()
        {
            var p1 = center + rotation * new Vector3(extents.x, 0, 0);
            var p2 = center + rotation * new Vector3(-extents.x, 0, 0);
            var p3 = center + rotation * new Vector3(0, extents.y, 0);
            var p4 = center + rotation * new Vector3(0, -extents.y, 0);
            var p5 = center + rotation * new Vector3(0, 0, extents.z);
            var p6 = center + rotation * new Vector3(0, 0, -extents.z);
            var b = new Bounds(p1, Vector3.zero);
            b.Encapsulate(p2);
            b.Encapsulate(p3);
            b.Encapsulate(p4);
            b.Encapsulate(p5);
            b.Encapsulate(p6);
            return b;
        }

        public override string ToString()
        {
            return string.Format("Position: {0} | Size: {1} | Rotation: {2}", center, extents * 2, rotation.eulerAngles);
        }

        public static RotationalBounds operator *(Matrix4x4 matrix, RotationalBounds bounds)
        {
            // Transform the center
            Vector3 newCenter = matrix.MultiplyPoint(bounds.center);

            // Extract the rotation matrix and apply it to the current rotation
            Quaternion newRotation = matrix.rotation * bounds.rotation;

            // Apply the absolute values of the matrix scale to the extents
            Vector3 newExtents = Vector3.Scale(bounds.extents, new Vector3(
                Mathf.Abs(matrix.lossyScale.x),
                Mathf.Abs(matrix.lossyScale.y),
                Mathf.Abs(matrix.lossyScale.z)
            ));

            return new RotationalBounds(newCenter, newExtents * 2, newRotation);
        }

        public Vector3[] AllPoints()
        {
            Vector3[] points = new Vector3[8];

            Vector3 halfExtents = extents;
            Quaternion rot = rotation;
            Vector3 center = this.center;

            points[0] = center + rot * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
            points[1] = center + rot * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            points[2] = center + rot * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
            points[3] = center + rot * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
            points[4] = center + rot * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
            points[5] = center + rot * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            points[6] = center + rot * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
            points[7] = center + rot * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);

            return points;
        }

        public Vector3 min
        {
            get
            {
                var points = AllPoints();
                Vector3 min = points[0];

                for (int i = 1; i < points.Length; i++)
                {
                    min = Vector3.Min(min, points[i]);
                }

                return min;
            }
        }

        public Vector3 max
        {
            get
            {
                Vector3[] points = AllPoints();
                Vector3 max = points[0];

                for (int i = 1; i < points.Length; i++)
                {
                    max = Vector3.Max(max, points[i]);
                }

                return max;
            }
        }

    }
}