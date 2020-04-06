using System;
using Unity.Collections;
using Unity.Mathematics;

//NOTE: Does not work. Unity Collections does not support nested arrays since NativeChunkList relys on NativeList of NativeArrays.

namespace Elanetic.Tools
{

    /*
    /// <summary>
    /// Similar to ChunkList but is a struct and uses Unity's Collections for internal storage. Used for DOTS.
    /// </summary>
    public struct NativeChunkList<T> where T : struct
    {
        private int m_ChunkSize;
        private NativeOutwardList<NativeArray<T>> m_Chunks;
        private Allocator m_Allocator;

        public NativeChunkList(int chunkSize, Allocator allocator)
        {
            if(chunkSize <= 0)
            {
                //Also if your setting your chunk size to a really low number, your missing out on the performance of this class. Too large and your allocating too much and leaving more potentially unused memory.
                throw new ArgumentException("The chunk size must be larger than zero.", nameof(chunkSize));
            }

            m_ChunkSize = chunkSize;
            m_Allocator = allocator;
            m_Chunks = new NativeOutwardList<NativeArray<T>>(m_Allocator);
        }

        public void SetItem(int2 coord, T item)
        {
            int2 m_TempChunkCoord  = CoordToChunkCoord(coord);
            int m_TempIndexer = m_Chunks.CoordToIndex(m_TempChunkCoord);
            NativeArray<T> m_TempRetrievedChunk = m_Chunks.GetItem(m_TempIndexer);
            if(m_TempRetrievedChunk == null)
            {
                m_TempRetrievedChunk = new NativeArray<T>(m_ChunkSize * m_ChunkSize, m_Allocator);
                m_Chunks.SetItem(m_TempIndexer, m_TempRetrievedChunk);
            }
            m_TempChunkCoord = coord - (m_TempChunkCoord * m_ChunkSize);
            m_TempRetrievedChunk[((m_TempChunkCoord.y < 0 ? (-m_TempChunkCoord.y) - 1 : m_TempChunkCoord.y) * m_ChunkSize) + (m_TempChunkCoord.x < 0 ? (-m_TempChunkCoord.x) - 1 : m_TempChunkCoord.x)] = item;
        }

        public T GetItem(int2 coord)
        {
            NativeArray<T> m_TempRetrievedChunk = m_Chunks.GetItem(CoordToChunkCoord(coord));
            int2 m_TempChunkCoord = CoordToChunkCoord(coord);
            if(m_TempRetrievedChunk != null)
            {
                m_TempChunkCoord = coord - (m_TempChunkCoord * m_ChunkSize);
                return m_TempRetrievedChunk[((m_TempChunkCoord.y < 0 ? (-m_TempChunkCoord.y) - 1 : m_TempChunkCoord.y) * m_ChunkSize) + (m_TempChunkCoord.x < 0 ? (-m_TempChunkCoord.x) - 1 : m_TempChunkCoord.x)];
            }
            return default;
        }

        private int2 CoordToChunkCoord(int2 coord)
        {
            return new int2(
                (coord.x < 0 ? coord.x - m_ChunkSize + 1 : coord.x) / m_ChunkSize,
                (coord.y < 0 ? coord.y - m_ChunkSize + 1 : coord.y) / m_ChunkSize);
        }
    }
    */
}