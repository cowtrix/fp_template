using System.Collections.Generic;
using System.Linq;
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

		public static Rect WorldBoundsToScreenRect(this Bounds worldBounds, Camera camera)
		{
			if (!camera)
			{
				return default;
			}
			var screenRect = new Rect(camera.WorldToScreenPoint(worldBounds.center), Vector2.zero);
			foreach (var p in worldBounds.AllPoints())
			{
				var screenP = camera.WorldToScreenPoint(p);
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

		public static bool BoundsWithinFrustrum(this Camera camera, Bounds worldBounds)
		{
			foreach (var p in worldBounds.AllPoints())
			{
				var screenP = camera.WorldToScreenPoint(p);
				if(screenP.z > 0)
				{
					return true;
				}
			}
			return false;
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