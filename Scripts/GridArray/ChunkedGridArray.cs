using System;
using System.Collections;
using System.Collections.Generic;

using Debug = UnityEngine.Debug;

namespace Elanetic.Tools
{
    public class ChunkedGridArray<T>
    {
        /// <summary>
        /// Size of individual chunk.
        /// </summary>
        public int chunkSize { get; private set; }
        /// <summary>
        /// Size in chunks.
        /// </summary>
        public int size => m_Chunks.size;

        public GridArray<T[]> m_Chunks;

        public ChunkedGridArray(int chunkSize = 16, int initialSize = 8, int distanceResizeAmount = 16)
        {
#if SAFE_EXECUTION
            if(chunkSize <= 0)
            {
                //Also if your setting your chunk size to a really low number, your missing out on the performance of this class. Too large and your allocating too much and leaving more potentially unused memory.
                throw new ArgumentException("The chunk size must be larger than zero.", nameof(chunkSize));
            }
#endif

            this.chunkSize = chunkSize;
            m_Chunks = new GridArray<T[]>(initialSize, distanceResizeAmount);
        }

        public void SetItem(int x, int y, T item)
        {
            //int index = GridArray.CellToIndex((x / chunkSize) - (((x & int.MinValue) >> 31) & 1), (y / chunkSize) - (((y & int.MinValue) >> 31) & 1));
            int index = GetChunkIndex(x,y);
            T[] array = m_Chunks.GetItem(index);
            if(array == null)
            {
                array = new T[chunkSize * chunkSize];
                m_Chunks.SetItem(index, array);
            }

            array[FastMath.Abs(((y % chunkSize) * chunkSize) + (x % chunkSize))] = item;
        }

        public T GetItem(int x, int y)
        {
            //T[] chunk = m_Chunks.GetItem((x / chunkSize) - (((x & int.MinValue) >> 31) & 1), (y / chunkSize) - (((y & int.MinValue) >> 31) & 1));
            //int xChunk = (x / chunkSize) - (((x & int.MinValue) >> 31) & 1);
            //int yChunk = (y / chunkSize) - (((y & int.MinValue) >> 31) & 1);
            T[] chunk = GetChunk(x, y);
            //Debug.Log("GETITEM: " + xChunk + ", " + yChunk);
            //UnityEngine.Debug.Log(((x / chunkSize) - (((x & int.MinValue) >> 31) & 1)).ToString() + " | " + ((y / chunkSize) - (((y & int.MinValue) >> 31) & 1)).ToString());
            //UnityEngine.Debug.Log("Index: " + (((y % chunkSize) * chunkSize) + (x % chunkSize)).ToString());
            if(chunk != null)
                return chunk[FastMath.Abs(((y % chunkSize) * chunkSize) + (x % chunkSize))];
            return default;
        }

        public T[] GetChunk(int x, int y)
        {
            int negativityBoost = (((x & int.MinValue) >> 31) & 1);
            x = ((x + negativityBoost) / chunkSize) - negativityBoost;
            negativityBoost = (((y & int.MinValue) >> 31) & 1);
            y = ((y + negativityBoost) / chunkSize) - negativityBoost;
            if(x == 1 && y == -1)
            {
                Debug.Log("Getting chunk target");
            }
            return m_Chunks.GetItem(x, y);
        }

        public T[] GetChunk(int chunkIndex)
        {
            return m_Chunks.GetItem(chunkIndex);
        }

        public int GetChunkIndex(int x, int y)
        {
            int negativityBoost = (((x & int.MinValue) >> 31) & 1);
            x = ((x + negativityBoost) / chunkSize) - negativityBoost;
            negativityBoost = (((y & int.MinValue) >> 31) & 1);
            y = ((y + negativityBoost) / chunkSize) - negativityBoost;
            return GridArray.CellToIndex(x,y);
        }

        public int GetCellIndexWithinChunk(int x, int y)
        {
            return FastMath.Abs(((y % chunkSize) * chunkSize) + (x % chunkSize));
        }
    }
}