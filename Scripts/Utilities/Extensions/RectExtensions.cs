using System.Collections.Generic;
using UnityEngine;

namespace FPTemplate.Utilities.Extensions
{
    public static class RectExtensions
    {
        public static Rect CropToOuterRect(this Rect inner, Rect outer)
        {
            // Ensure the inner rect is within the bounds of the outer rect
            float xMin = Mathf.Max(inner.xMin, outer.xMin);
            float yMin = Mathf.Max(inner.yMin, outer.yMin);
            float xMax = Mathf.Min(inner.xMax, outer.xMax);
            float yMax = Mathf.Min(inner.yMax, outer.yMax);

            // Calculate the cropped width and height
            float width = Mathf.Max(0, xMax - xMin);
            float height = Mathf.Max(0, yMax - yMin);

            return new Rect(xMin, yMin, width, height);
        }

        public static List<Rect> OccludeRects(List<Rect> rects)
        {
            for (int i = 0; i < rects.Count; i++)
            {
                Rect current = rects[i];
                for (int j = rects.Count - 1; j > i; j--)
                {
                    Rect top = rects[j];
                    if (top.Overlaps(current))
                    {
                        if (top.Contains(current))
                        {
                            current = Rect.zero; // Completely occluded
                            break;
                        }
                        else
                        {
                            current = ClipRect(current, top); // Partially occluded
                        }
                    }
                }
                rects[i] = current;
            }
            return rects;
        }

        public static Rect ClipRect(Rect lower, Rect upper)
        {
            if (lower.xMin < upper.xMin)
                lower.xMax = Mathf.Min(lower.xMax, upper.xMin);
            if (lower.xMax > upper.xMax)
                lower.xMin = Mathf.Max(lower.xMin, upper.xMax);
            if (lower.yMin < upper.yMin)
                lower.yMax = Mathf.Min(lower.yMax, upper.yMin);
            if (lower.yMax > upper.yMax)
                lower.yMin = Mathf.Max(lower.yMin, upper.yMax);

            return lower;
        }

        public static bool Contains(this Rect rect, Rect other)
        {
            return rect.xMin <= other.xMin && rect.xMax >= other.xMax &&
                   rect.yMin <= other.yMin && rect.yMax >= other.yMax;
        }

        public static Rect Encapsulate(this Rect rect, Rect other)
        {
            rect = rect.Encapsulate(other.min);
            rect = rect.Encapsulate(other.max);
            return rect;
        }

        public static Rect Encapsulate(this Rect rect, Vector2 point)
        {
            if (rect.Contains(point))
            {
                // We don't need to do anything
                return rect;
            }
            if (point.x < rect.xMin)
            {
                if (point.y < rect.yMin)
                {
                    var size = rect.max - point;
                    rect.Set(point.x, point.y, size.x, size.y);
                    return rect;
                }
                if (point.y > rect.yMax)
                {
                    var newPos = new Vector2(point.x, rect.y);
                    var newMax = new Vector2(rect.xMax, point.y);
                    var size = newMax - newPos;
                    rect.Set(newPos.x, newPos.y, size.x, size.y);
                    return rect;
                }
                rect.Set(point.x, rect.y, rect.xMax - point.x, rect.height);
                return rect;
            }
            if (point.x > rect.xMax)
            {
                if (point.y < rect.yMin)
                {
                    var newPos = new Vector2(rect.x, point.y);
                    var newMax = new Vector2(point.x, rect.yMax);
                    var size = newMax - newPos;
                    rect.Set(newPos.x, newPos.y, size.x, size.y);
                    return rect;
                }
                if (point.y > rect.yMax)
                {
                    var newPos = rect.position;
                    var newMax = point;
                    var size = newMax - newPos;
                    rect.Set(newPos.x, newPos.y, size.x, size.y);
                    return rect;
                }
                rect.Set(rect.x, rect.y, point.x - rect.xMin, rect.height);
                return rect;
            }
            if (point.y > rect.yMax)
            {
                rect.Set(rect.x, rect.y, rect.width, point.y - rect.yMin);
                return rect;
            }
            if (point.y < rect.yMin)
            {
                rect.Set(rect.x, point.y, rect.width, rect.yMax - point.y);
                return rect;
            }
            return rect;
        }
    }
}
