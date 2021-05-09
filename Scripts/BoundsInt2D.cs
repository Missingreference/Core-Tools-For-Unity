using System;
using System.Globalization;
using UnityEngine;

namespace Elanetic.Tools
{
    /// <summary>
    /// A simpler version of UnityEngine.BoundsInt.
    /// No position or z dimension. Instances are immutable similar to Vector2, Vector3, etc. Fewer unnecessary properties to make them less confusing.
    /// Properties don't call other methods for faster calls since they are calculated on instantiate. Contains function has the max as inclusive instead of exclusive.
    /// Size also includes the max. For example if min and max are the same then the size will be (1,1).
    /// </summary>
    public struct BoundsInt2D : IEquatable<BoundsInt2D>, IFormattable
    {

        public Vector2Int min;
        public Vector2Int max;

        public Vector2 center;
        public Vector2Int size;
        public int count;

        public BoundsInt2D(int xMin, int yMin, int xSize, int ySize) : this(new Vector2Int(xMin, yMin), xSize, ySize) { }
        public BoundsInt2D(int xMin, int yMin, Vector2Int max) : this(new Vector2Int(xMin, yMin), max) { }

        public BoundsInt2D(Vector2Int min, int xSize, int ySize)
        {
#if SAFE_EXECUTION
            if(xSize < 0)
                throw new ArgumentException("xSize parameter cannot be less than zero. Received '" + xSize + "'.", nameof(xSize));
            if (ySize < 0)
                throw new ArgumentException("ySize parameter cannot be less than zero. Received '" + ySize + "'.", nameof(ySize));
#endif

            this.min = min;
            this.max = new Vector2Int(min.x + Mathf.Max(0, xSize - 1), min.y + Mathf.Max(0, ySize - 1));
            this.size = new Vector2Int(xSize, ySize);
            this.center = new Vector2(min.x + (size.x / 2.0f), min.y + (size.y / 2.0f));
            this.count = size.x * size.y;
        }

        public BoundsInt2D(Vector2Int min, Vector2Int max)
        {
#if SAFE_EXECUTION  
            if(max.x < min.x && max.y < min.y)
                throw new ArgumentException("Bounds maximum position cannot be smaller than the minimum position. Min: " + min.ToString() + " Max: " + max.ToString(), nameof(max));
            if(max.x < min.x)
                throw new ArgumentException("Bounds maximum 'x' position cannot be smaller than the minimum position.", nameof(max));
            if(max.y < min.y)
                throw new ArgumentException("Bounds maximum 'y' position cannot be smaller than the minimum position.", nameof(max));
#endif

            this.min = min;
            this.max = max;
            this.size = max - min + Vector2Int.one;
            this.center = new Vector2(min.x + (size.x / 2.0f), min.y + (size.y / 2.0f));
            this.count = size.x * size.y;
        }

        /// <summary>
        /// Checks if a Vector2Int is within the bounds. More than or equal to the minimum point and less than or equal to the maximum point.
        /// Maximum is inclusive unlike BoundsInt.Contains where its exclusive.
        /// </summary>
        /// <param name="position">The point to check.</param>
        /// <returns>Whether or not the inputted position is within these bounds.</returns>
        public bool Contains(Vector2Int position)
        {
            return position.x >= min.x
                && position.y >= min.y
                && position.x <= max.x
                && position.y <= max.y;
        }

        public bool Overlaps(BoundsInt2D other)
        {
            if(count == 0 || other.count == 0) return false;
            return other.min.x <= max.x
                && other.max.x >= min.x
                && other.min.y <= max.y
                && other.max.y >= min.y;
        }

        public int GetOverlapCount(BoundsInt2D other)
        {
            if(count == 0 || other.count == 0) return 0;
            return Mathf.Max(0, Mathf.Min(max.x, other.max.x) - Mathf.Max(min.x, other.min.x) + 1) *
                   Mathf.Max(0, Mathf.Min(max.y, other.max.y) - Mathf.Max(min.y, other.min.y) + 1);
        }

        //Conversion to and from UnityEngine.BoundsInt
        public static implicit operator BoundsInt(BoundsInt2D boundsInt2D) => new BoundsInt(boundsInt2D.min.x, boundsInt2D.min.y, 0, boundsInt2D.size.x, boundsInt2D.size.y, 1);
        public static explicit operator BoundsInt2D(BoundsInt unityBounds) => new BoundsInt2D(unityBounds.xMin, unityBounds.yMin, unityBounds.size.x, unityBounds.size.y);

        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("Min: ({0}), Max: ({1}), Size: ({2})",
                min.x.ToString(format, formatProvider) + "," + min.y.ToString(format, formatProvider),
                max.x.ToString(format, formatProvider) + "," + max.y.ToString(format, formatProvider),
                size.x.ToString(format, formatProvider) + "," + size.y.ToString(format, formatProvider));
        }

        public static bool operator ==(BoundsInt2D lhs, BoundsInt2D rhs)
        {
            return lhs.min == rhs.min && lhs.max == rhs.max;
        }

        public static bool operator !=(BoundsInt2D lhs, BoundsInt2D rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            if(!(other is BoundsInt2D)) return false;

            return Equals((BoundsInt2D)other);
        }

        public bool Equals(BoundsInt2D other)
        {
            return min.Equals(other.min) && max.Equals(other.max);
        }

        public override int GetHashCode()
        {
            return min.GetHashCode() ^ (max.GetHashCode() << 2);
        }
    }
}