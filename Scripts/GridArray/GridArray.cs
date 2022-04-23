using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Elanetic.Tools
{
    /// <summary>
    /// This class creates an array to store values on a 2D grid. Lookups are faster than a Dictionary, especially in combination with ChunkedGridArray.
    /// Downside is setting coordinates far from (0,0) allocates a lot of memory depending on the type. Use ChunkedGridArray for better allocations management.
    /// It is recommended that if you plan to make your home coordinate far from origin that you add an offset to this as input to be as close as possible to origin to reduce allocations.
    /// REVIEW: Does this class need to exist? Can we fully replace it with ChunkedGridArray since it is the definitive version of GridArray? ChunkedGridArray manages memory way better but doesn't have the performant reusability that GridArray.GetItem(index) has.
    /// </summary>
    static public class GridArray
    {
        /// <summary>
        /// Get the index of a cell coordinate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int CellToIndex(int x, int y)
        {
            //There are 4 regions with associated indexs (0 is positive inclusive):
            //+x +y = 0,
            //-x +y = 1,
            //+x -y = 2,
            //-x -y = 3
            //For example: 10, 10 is region 0 | -10, -10 is region 3 | -1, 0 is region 1
            /*
            //If coordX is negative set as 1 otherwise 0
            int regionX = ((x & int.MinValue) >> 31) & 1;
            //If coordY is negative set as 2 otherwise 0
            int regionY = ((y & int.MinValue) >> 30) & 2;

            int regionIndex = regionX + regionY;

            regionY >>= 1;

            //Calculate tile index within region
            Vector2Int pos = new Vector2Int(FastMath.Abs(x) - regionX, FastMath.Abs(y) - regionY);
            int tileIndex = pos.y + pos.x - 1;
            //This uses the Gauss Sum formula to count the number of indices in the following triangle pattern repeating representing 1 of 4 regions:
            //[20]
            //[14][19]
            //[ 9][13][18]
            //[ 5][ 8][12][17]
            //[ 2][ 4][ 7][11][16]
            //[ 0][ 1][ 3][ 6][10][15] ...
            tileIndex = ((tileIndex + 1) * (tileIndex + 2) / 2) + pos.y;

            //Get specific index within all regions
            //Thus packing all regions into a single array ordered as so:
            //[0][0][0][0][1][1][1][1][2][2][2][2][3][3][3][3][4][4][4][4]...
            tileIndex = (tileIndex * 4) + regionIndex;

            return tileIndex;
            */

            //NOTE: The following is the exact same code as above but squished down to use less local variables and inline functions.
            //Benchmarks show replacing it with this version shows a large performance boost.
            //Another large performance boost was swapping the function parameters with 2 integers instead of 1 Vector2Int.

            int regionX = ((x & int.MinValue) >> 31) & 1;
            int regionY = ((y & int.MinValue) >> 30) & 2;

            int m = y >> 63;
            int posY = ((y + m) ^ m) - (regionY >> 1);
            m = x >> 63;
            int tileIndex = posY + (((x + m) ^ m) - regionX) - 1;
            return ((((tileIndex + 1) * (tileIndex + 2) / 2) + posY) * 4) + regionX + regionY;
        }
    }

    /// <summary>
    /// This class creates an array to store values on a 2D grid. Lookups are faster than a Dictionary, especially in combination with ChunkedGridArray.
    /// Downside is setting coordinates far from (0,0) allocates a lot of memory depending on the type.
    /// It is recommended that if you plan to make your home coordinate far from origin that you add an offset to this as input to be as close as possible to origin to reduce allocations.
    /// </summary>
    public class GridArray<T>
    {
        /// <summary>
        /// The width or height of the grid.
        /// </summary>
        public int size => m_Size;

        /// <summary>
        /// The size of the internal array.
        /// </summary>
        public int arraySize => m_Array.Length;

        public T[] m_Array;

        private int m_Size;
        private int m_SizeSquared;
        private int m_DistanceResizeAmount = 16;
        private int m_DataSize;

        /// <summary>
        /// Initialize the GridArray with the width or height of the grid.
        /// Remember: the internal array size will be initialSize * initialSize so an initialSize of 16 will be 256 allocated cells.
        /// </summary>
        /// <param name="initialSize">The width or height of the initial grid.</param>
        /// <param name="distanceResizeAmount">How far on the grid you want to resize to. Every time we need to resize we will resize the specified amount in every direction.
        /// For example: If we need to resize the internal array with a 16 x 16 grid and we have a distanceResizeAmount of 16, the new grid size will be 32 x 32. Resize again and it will be 48 x 48. And so on.
        /// </param>
        public GridArray(int initialSize=16)
        {
#if SAFE_EXECUTION
            if(initialSize < 0)
                throw new ArgumentOutOfRangeException(nameof(initialSize), "Initial size must be a positive number.");
#endif
            m_Size = initialSize;
            m_SizeSquared = m_Size * m_Size;
            m_Array = new T[m_SizeSquared];
            if(typeof(T).IsValueType)
            {
                m_DataSize = Marshal.SizeOf(typeof(T));
            }
            else if(IntPtr.Size == 8) //64 bit system
            {
                m_DataSize = 8;
            }
            else //32 bit system
            {
                m_DataSize = 4;
            }

            //Scale distance resize from 16 to 1 between 1KB and 1MB
            m_DistanceResizeAmount = 17 - FastMath.Clamp((m_SizeSquared * m_DataSize) / 1024, 1, 16);
        }

        /// <summary>
        /// Add the specified item to the array at the specified coordinates.
        /// Note: Will resize the internal array if needed.
        /// Note: Will replace anything existing at the coord.
        /// </summary>
        public void SetItem(int x, int y, T item)
        {
            SetItem(GridArray.CellToIndex(x, y), item);
        }

        /// <summary>
        /// Add the specified item to the array at the specified index.
        /// Note: Will resize the internal array if needed.
        /// Note: Will replace anything existing at the index.
        /// </summary>
        public void SetItem(int index, T item)
        {
            if(index >= m_Array.Length)
            {
                //No point allocating space if were only setting it to null/default value
                if(EqualityComparer<T>.Default.Equals(item, default))
                        return;

                while(index >= m_SizeSquared)
                {
                    m_Size += m_DistanceResizeAmount;
                    m_SizeSquared = m_Size * m_Size;
                }

#if SAFE_EXECUTION
                long memorySizeInBytes = (long)m_DataSize;
                long longSizeSquared = (long)m_SizeSquared;

                memorySizeInBytes *= longSizeSquared * longSizeSquared;

                if(memorySizeInBytes / 1024L / 1024L / 1024L > 1L)
                {
#if UNITY_EDITOR || UNITY_STANDALONE
                    UnityEngine.Debug.LogWarning("Allocating more than 1 GB of memory for a resize (" + (memorySizeInBytes / 1024L / 1024L / 1024L).ToString() + " GB).");
#endif
                }

#endif
                //Resize array
                T[] newArray = new T[m_SizeSquared];
                Array.Copy(m_Array, 0, newArray, 0, m_Array.Length);
                m_Array = newArray;

                m_DistanceResizeAmount = 17 - FastMath.Clamp((m_SizeSquared * m_DataSize) / 1024, 1, 16);
            }
#if SAFE_EXECUTION
            else if(index < 0)
                throw new IndexOutOfRangeException("Received a negative index: " + index.ToString() + ". Either GridArray.CellToIndex returned an index that overflows past Int.MaxValue or bad input.");
#endif
            m_Array[index] = item;
        }

        /// <summary>
        /// Get the item at the specified coordinates.
        /// If the index is out of range, a default for the value type or null for reference types.
        /// </summary>
        public T GetItem(int x, int y)
        {
            int index = GridArray.CellToIndex(x, y);
            if(index >= m_Array.Length)
                return default;
#if SAFE_EXECUTION
            else if(index < 0)
                throw new IndexOutOfRangeException("Received a negative index from GridArray.CellToIndex meaning that the index overflows past Int.MaxValue. Cell Coordinate: " + x.ToString() + ", " + y.ToString());
#endif
            return m_Array[index];
        }

        /// <summary>
        /// Get the item at the specified index.
        /// This is the fastest way to retrieve an item.
        /// If the index is out of range, a default for the value type or null for reference types.
        /// </summary>
        public T GetItem(int index)
        {
            if(index >= m_Array.Length)
                return default;
            return m_Array[index];
        }
    }
}