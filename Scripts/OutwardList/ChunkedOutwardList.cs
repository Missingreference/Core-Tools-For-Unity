using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

namespace Isaac.Tools
{
    public class ChunkedOutwardList<T>
    {
        public int chunkSize => m_ChunkSize;

        private int m_ChunkSize = 0;

        private OutwardList<T[]> m_Chunks;

        public ChunkedOutwardList(int chunkSize = 16)
        {
            if(chunkSize <= 0)
            {
                //Also if your setting your chunk size to a really low number, your missing out on the performance of this class. Too large and your allocating too much and leaving more potentially unused memory.
                throw new ArgumentException("The chunk size must be larger than zero.", nameof(chunkSize));
            }

            m_ChunkSize = chunkSize;
            m_Chunks = new OutwardList<T[]>();
        }

        private int m_TempIndexer = 0;
        //private Vector2Int m_TempChunkCoord = Vector2Int.zero;
        private int m_TempChunkX, m_TempChunkY;
        private T[] m_TempRetrievedChunk = null;
        public void SetItem(int x, int y, T item)
        {
            m_TempChunkX = CoordToChunkCoord(x);
            m_TempChunkY = CoordToChunkCoord(y);
            m_TempIndexer = OutwardList.CoordToIndex(m_TempChunkX, m_TempChunkY);
            m_TempRetrievedChunk = m_Chunks.GetItem(m_TempIndexer);
            if(m_TempRetrievedChunk == null)
            {
                m_TempRetrievedChunk = new T[chunkSize * chunkSize];
                m_Chunks.SetItem(m_TempIndexer, m_TempRetrievedChunk);
            }
            m_TempChunkX = x - (m_TempChunkX * m_ChunkSize);
            m_TempChunkY = y - (m_TempChunkY * m_ChunkSize);
            //m_TempChunkCoord = coord - (m_TempChunkCoord * m_ChunkSize);
            m_TempRetrievedChunk[((m_TempChunkY < 0 ? (-m_TempChunkY) - 1 : m_TempChunkY) * m_ChunkSize) + (m_TempChunkX < 0 ? (-m_TempChunkX) - 1 : m_TempChunkX)] = item;
        }

        public T GetItem(int x, int y)
        {
            m_TempChunkX = CoordToChunkCoord(x);
            m_TempChunkY = CoordToChunkCoord(y);
            m_TempRetrievedChunk = m_Chunks.GetItem(m_TempChunkX, m_TempChunkY);
            if(m_TempRetrievedChunk != null)
            {
                m_TempChunkX = x - (m_TempChunkX * m_ChunkSize);
                m_TempChunkY = y - (m_TempChunkY * m_ChunkSize);
                return m_TempRetrievedChunk[((m_TempChunkY < 0 ? (-m_TempChunkY) - 1 : m_TempChunkY) * m_ChunkSize) + (m_TempChunkX < 0 ? (-m_TempChunkX) - 1 : m_TempChunkX)];
            }
            return default;
        }

        private int CoordToChunkCoord(int coordAxisValue) => (coordAxisValue < 0 ? coordAxisValue - m_ChunkSize + 1 : coordAxisValue) / m_ChunkSize;

        /*
        private Vector2Int CoordToChunkCoord(Vector2Int coord)
        {
            return new Vector2Int(
                (coord.x < 0 ? coord.x - m_ChunkSize + 1 : coord.x) / m_ChunkSize,
                (coord.y < 0 ? coord.y - m_ChunkSize + 1 : coord.y) / m_ChunkSize);
        }
        */

        private int ChunkCoordToCoord(int chunkCoordAxisValue)
        {
            if(chunkCoordAxisValue < 0)
                return ((chunkCoordAxisValue + 1) * m_ChunkSize) - 1;
            return chunkCoordAxisValue * m_ChunkSize;
        }

        /*
        private Vector2Int ChunkCoordToCoord(Vector2Int chunkCoord)
        {
            if(chunkCoord.x < 0) chunkCoord = new Vector2Int(((chunkCoord.x+1)*m_ChunkSize)-1, chunkCoord.y);
            else chunkCoord = new Vector2Int(chunkCoord.x * m_ChunkSize, chunkCoord.y);
            if(chunkCoord.y < 0) chunkCoord = new Vector2Int(chunkCoord.x, ((chunkCoord.y+1)*m_ChunkSize)-1);
            else chunkCoord = new Vector2Int(chunkCoord.x, chunkCoord.y * m_ChunkSize);
            return chunkCoord;
        }
        */
    }
}