using System;
using System.Collections;
using System.Collections.Generic;

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
            int negativityBoost = (((x & int.MinValue) >> 31) & 1);
            int chunkX = ((x + negativityBoost) / chunkSize) - negativityBoost;
            negativityBoost = (((y & int.MinValue) >> 31) & 1);
            int chunkY = ((y + negativityBoost) / chunkSize) - negativityBoost;

            int chunkIndex = GridArray.CellToIndex(chunkX, chunkY);

            T[] array = m_Chunks.GetItem(chunkIndex);
            if(array == null)
            {
                array = new T[chunkSize * chunkSize];
                m_Chunks.SetItem(chunkIndex, array);
            }

            int localCellX = x - (chunkX * chunkSize);
            int localCellY = y - (chunkY * chunkSize);

            array[Utils.CoordToIndex(FastMath.Abs(localCellX), FastMath.Abs(localCellY), chunkSize)] = item;
        }

        public T GetItem(int x, int y)
        {
            int negativityBoost = (((x & int.MinValue) >> 31) & 1);
            int chunkX = ((x + negativityBoost) / chunkSize) - negativityBoost;
            negativityBoost = (((y & int.MinValue) >> 31) & 1);
            int chunkY = ((y + negativityBoost) / chunkSize) - negativityBoost;

            int chunkIndex = GridArray.CellToIndex(chunkX, chunkY);

            T[] chunk = m_Chunks.GetItem(chunkIndex);

            if(chunk != null)
            {
                int localCellX = x - (chunkX * chunkSize);
                int localCellY = y - (chunkY * chunkSize);
                return chunk[Utils.CoordToIndex(FastMath.Abs(localCellX), FastMath.Abs(localCellY), chunkSize)];
            }

            return default;
        }

        /// <summary>
        /// Get chunk by cell position.
        /// </summary
        public T[] GetChunk(int x, int y)
        {
            int negativityBoost = (((x & int.MinValue) >> 31) & 1);
            x = ((x + negativityBoost) / chunkSize) - negativityBoost;
            negativityBoost = (((y & int.MinValue) >> 31) & 1);
            y = ((y + negativityBoost) / chunkSize) - negativityBoost;
            return m_Chunks.GetItem(x, y);
        }

        /// <summary>
        /// Get chunk from chunk index. Can return null.
        /// </summary>
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