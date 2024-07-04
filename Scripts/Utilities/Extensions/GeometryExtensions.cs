using FPTemplate.Utilities.Maths;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace FPTemplate.Utilities.Extensions
{
    public static class GeometryExtensions
	{
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

        public static IEnumerable<Vector3> AllPoints(this RotationalBounds bounds)
        {
            // Calculate the 8 corner points of the bounding box
            Vector3[] points = new Vector3[8];

            Vector3 halfExtents = bounds.extents;
            Quaternion rotation = bounds.rotation;
            Vector3 center = bounds.center;

            points[0] = center + rotation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
            points[1] = center + rotation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            points[2] = center + rotation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
            points[3] = center + rotation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
            points[4] = center + rotation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
            points[5] = center + rotation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            points[6] = center + rotation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
            points[7] = center + rotation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);

            return points;
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
	}
}