using UnityEngine;

namespace Elanetic.Tools
{
    static public class RectIntExtensions
    {
        static public int GetOverlapCount(this RectInt rect, RectInt otherRect)
        {
            return Mathf.Max(0, Mathf.Min(otherRect.xMax - rect.xMin, rect.xMax - otherRect.xMin) * Mathf.Min(otherRect.yMax - rect.yMin, rect.yMax - otherRect.yMin));
        }
    }
}