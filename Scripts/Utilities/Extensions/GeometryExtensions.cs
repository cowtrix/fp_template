using FPTemplate.Utilities.Maths;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace FPTemplate.Utilities.Extensions
{
    public static class GeometryExtensions
	{
        public static Matrix4x4 FlipX(this Matrix4x4 matrix)
        {
            // Define a flip matrix that flips the x-axis
            Matrix4x4 flipMatrix = Matrix4x4.identity;
            flipMatrix.m00 = -1;

            // Multiply the original matrix by the flip matrix
            return flipMatrix * matrix;
        }

        public static Bounds GetEncompassingBounds(this IEnumerable<Bounds> enumerable)
        {
            if (enumerable == null || !enumerable.Any())
            {
                return default;
            }
            var b = enumerable.First();
            foreach (var b2 in enumerable.Skip(1))
            {
                b.Encapsulate(b2);
            }
            return b;
        }

        public static bool BetweenPlanes(Vector3 worldPos, Plane startPlane, Plane endPlane)
		{
			return startPlane.GetSide(worldPos) && endPlane.GetSide(worldPos);
		}

		public static Bounds TranslateBounds(Bounds bounds, Matrix4x4 trs)
        {
			return new Bounds(trs.MultiplyPoint3x4(bounds.center), trs.MultiplyVector(bounds.size));
        }

		public static IEnumerable<Vector3> AllPoints(this Bounds b)
		{
			yield return new Vector3(b.min.x, b.min.y, b.min.z);
			yield return new Vector3(b.min.x, b.min.y, b.max.z);
			yield return new Vector3(b.min.x, b.max.y, b.min.z);
			yield return new Vector3(b.min.x, b.max.y, b.max.z);

			yield return new Vector3(b.max.x, b.min.y, b.min.z);
			yield return new Vector3(b.max.x, b.min.y, b.max.z);
			yield return new Vector3(b.max.x, b.max.y, b.min.z);
			yield return new Vector3(b.max.x, b.max.y, b.max.z);
		}

        public static Bounds GetBounds(this IEnumerable<Renderer> renderers)
		{
			if(renderers == null)
			{
				return default;
			}
			return renderers
				.Where(r => r)
				.Select(r => r.bounds).GetEncompassingBounds();
		}

        public static Vector3[] GetQuadCorners(Vector3 planeNormal, Vector3 planePoint, Vector2 size)
        {
            // Ensure the normal is normalized
            planeNormal.Normalize();

            // Calculate the right and up vectors based on the plane normal
            Vector3 right = Vector3.Cross(planeNormal, Vector3.up);
            if (right == Vector3.zero)
            {
                right = Vector3.Cross(planeNormal, Vector3.right);
            }
            right.Normalize();

            Vector3 up = Vector3.Cross(right, planeNormal);
            up.Normalize();

            // Calculate the half extents
            Vector3 halfWidth = right * size.x * 0.5f;
            Vector3 halfHeight = up * size.y * 0.5f;

            // Calculate the four corners of the quad
            Vector3[] corners = new Vector3[4];
            corners[0] = planePoint - halfWidth - halfHeight;
            corners[1] = planePoint + halfWidth - halfHeight;
            corners[2] = planePoint + halfWidth + halfHeight;
            corners[3] = planePoint - halfWidth + halfHeight;

            return corners;
        }
    }
}
