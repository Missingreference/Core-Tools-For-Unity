using System;
using System.Collections;
using System.Collections.Generic;

namespace Elanetic.Tools
{
    /// <summary>
    /// This version of GridArray is a 2 step over previous versions to manage memory significantly better. First way is to have each index reference a "chunk" which is essentially a chunk*chunk sized array.
    /// The second prong is that the underlying gridarray CellToIndex implementation is a integer that will reference a master array of every chunk seperated by 4 regions. All 4 regions will be resized independently of eachother and instead of allocating an array reference for each empty cell(8 bytes) and instead using Int32(4 bytes) indexs.
    /// Visually it goes: Input x,y coordinate -> REGIONS(4 arrays of integers(indexs)) -> REGION(array of integers(indexs) -> Integer(4 bytes) index of chunk actual -> Master array of Chunk Actuals(Array of all chunks) -> Individual Chunk Actual[chunkSize*chunkSize] -> Cell(T)
    /// This results in optimal memory usage since older versions were capable of reaching gigabytes of usage around the (5000,5000) coordinates in any direction. However this implementation produces poorer performance in set/get operations due to multiple if statements and array lookups.
    /// </summary>
    public class ChunkedGridArray<T>
    {
        /// <summary>
        /// Size of individual chunk.
        /// </summary>
        public int chunkSize => m_ChunkSize;

        private int[][] m_Regions = new int[4][];
        private int[] m_RegionsSizes = new int[4];
        private int m_ChunkSize;

        //Reference the actual chunks. It is organised this way to optimize memory usage
        private T[][] m_ChunkActuals;
        private int m_ChunkActualCount;

        public ChunkedGridArray(int chunkSize = 16, int initialSize = 8)
        {
#if DEBUG
            if(chunkSize <= 0)
            {
                //Also if your setting your chunk size to a really low number, your missing out on the optimizations of this class. Too large and your allocating too much and leaving more potentially unused memory.
                throw new ArgumentException("The chunk size must be larger than zero.", nameof(chunkSize));
            }
            if(initialSize < 0)
                throw new ArgumentException("The initial size must be more than or equal to zero.", nameof(initialSize));
#endif

            int size = ((initialSize-1) * initialSize / 2) + initialSize;

            m_Regions[0] = new int[size];
            m_Regions[1] = new int[size];
            m_Regions[2] = new int[size];
            m_Regions[3] = new int[size];

            m_RegionsSizes[0] = size;
            m_RegionsSizes[1] = size;
            m_RegionsSizes[2] = size;
            m_RegionsSizes[3] = size;

            m_ChunkSize = chunkSize;
            m_ChunkActuals = new T[FastMath.Max(1,size * 4)][];
        }

        public void SetItem(int x, int y, T item)
        {
            //Get the region that the position is specified at. At the same time determines with the inputted x,y coordinate is negative or not
            int regionX = ((x & int.MinValue) >> 31) & 1;
            int regionY = (y & int.MinValue) >> 30;
            int yIsNegative = (regionY >> 1) & 1;
            regionY &= 2;

            int regionIndex = regionX + regionY;
            int[] region = m_Regions[regionIndex];

            //Determine the absolute x,y coordinate within the region
            int posX = FastMath.Abs(x) - regionX;
            int posY = FastMath.Abs(y) - yIsNegative;

            //Determine the absolute chunk coordinate within the region
            int chunkPosX = posX / chunkSize;
            int chunkPosY = posY / chunkSize;

            int size = chunkPosX + chunkPosY + 1;
#if DEBUG
            //In the event of an overflow, the inputted position has reached too far. For example in testing trying to set 375000,375000 to a value triggers this overflow however where 370000,370000 does not.
            //In addition as you approach these coordinates you are allocating more than a gigabyte of memory in integers alone anyways.
            int targetRegionIndex = checked(((size - 1) * size / 2) + chunkPosY);
#else

            int targetRegionIndex = ((size - 1) * size / 2) + chunkPosY;
#endif
            int currentRegionSize = m_RegionsSizes[regionIndex];
            T[] chunk;

            //Retrieve specific chunk
            if(targetRegionIndex >= currentRegionSize)
            {
                //Region is of insufficient size for the specified item coordinate. Resize.

                //No point allocating space if were only setting it to null/default value
                if(EqualityComparer<T>.Default.Equals(item, default))
                    return;

                //Custom resize amount. Once it gets to around a quarter of a Megabyte per region, resize by 1 coordinate. Scaled from 8 to 1 distance.
                const int targetLimit = 362;
                const int steps = 8;
                const int divisible = targetLimit / steps;
                int amountToResize = (targetLimit - FastMath.ClampFast(currentRegionSize, divisible, targetLimit)) / divisible + 1;
                int newSize = size + amountToResize;
                //Actual array amount based on size. Size is single axis distance on the Y-axis because the Y-axis is a higher index than X.
                newSize = ((newSize - 1) * newSize / 2) + newSize;

                int[] newRegion = new int[newSize];
                Array.Copy(region, newRegion, region.Length);
                region = newRegion;
                m_Regions[regionIndex] = newRegion;
                m_RegionsSizes[regionIndex] = newSize;

                //Add chunk. We know that the chunk doesn't exist so create chunk.
                if(m_ChunkActuals.Length == m_ChunkActualCount)
                {
                    //Resize array
                    T[][] newArray = new T[m_ChunkActualCount * 2][];
                    Array.Copy(m_ChunkActuals, 0, newArray, 0, m_ChunkActualCount);
                    m_ChunkActuals = newArray;
                }

                chunk = new T[chunkSize * chunkSize];
                m_ChunkActuals[m_ChunkActualCount] = chunk;
                m_ChunkActualCount++;

                region[targetRegionIndex] = m_ChunkActualCount;
            }
            else
            {
                int chunkActualIndex = region[targetRegionIndex] - 1;
                if(chunkActualIndex < 0) //Chunk index does not exist so a new chunk must be created
                {
                    //No point allocating space if were only setting it to null/default value
                    if(EqualityComparer<T>.Default.Equals(item, default))
                        return;

                    //Add chunk
                    if(m_ChunkActuals.Length == m_ChunkActualCount)
                    {
                        //Resize array
                        T[][] newArray = new T[m_ChunkActualCount * 2][];
                        Array.Copy(m_ChunkActuals, 0, newArray, 0, m_ChunkActualCount);
                        m_ChunkActuals = newArray;
                    }

                    chunk = new T[chunkSize * chunkSize];
                    m_ChunkActuals[m_ChunkActualCount] = chunk;
                    m_ChunkActualCount++;
                    region[targetRegionIndex] = m_ChunkActualCount;
                }
                else
                {
                    chunk = m_ChunkActuals[chunkActualIndex];
                }
            }

            int localCellX = posX % m_ChunkSize;
            int localCellY = posY % m_ChunkSize;
            chunk[(localCellY * m_ChunkSize) + localCellX] = item;
        }

        public T GetItem(int x, int y)
        {
            //Get the region that the position is specified at. At the same time determines with the inputted x,y coordinate is negative or not
            int regionX = ((x & int.MinValue) >> 31) & 1;
            int regionY = (y & int.MinValue) >> 30;
            int yIsNegative = (regionY >> 1) & 1;
            regionY &= 2;

            int regionIndex = regionX + regionY;
            int[] region = m_Regions[regionIndex];

            //Determine the absolute x,y coordinate within the region
            int posX = FastMath.Abs(x) - regionX;
            int posY = FastMath.Abs(y) - yIsNegative;

            //Determine the absolute chunk coordinate within the region
            int chunkPosX = posX / chunkSize;
            int chunkPosY = posY / chunkSize;

            int size = chunkPosX + chunkPosY + 1;
            int targetRegionIndex = ((size - 1) * size / 2) + chunkPosY;


            int currentRegionSize = m_RegionsSizes[regionIndex];


            if(targetRegionIndex >= currentRegionSize) //Target region index is out of bounds so no value exists
                return default;

            int chunkActualIndex = region[targetRegionIndex] - 1;
            if(chunkActualIndex < 0) //Chunk does not exist so no value exists
                return default;

            return m_ChunkActuals[chunkActualIndex][((posY % m_ChunkSize) * m_ChunkSize) + (posX % m_ChunkSize)];
        }

        /*
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

        /// <summary>
        /// Get index of chunk.
        /// </summary>
        public int GetChunkIndex(int x, int y)
        {
            int negativityBoost = (((x & int.MinValue) >> 31) & 1);
            x = ((x + negativityBoost) / chunkSize) - negativityBoost;
            negativityBoost = (((y & int.MinValue) >> 31) & 1);
            y = ((y + negativityBoost) / chunkSize) - negativityBoost;
            return GridArray.CellToIndex(x,y);
        }

        /// <summary>
        /// Get cell index with chunk from global cell position.
        /// </summary>
        public int GetCellIndexWithinChunk(int x, int y)
        {
            int negativityBoost = (((x & int.MinValue) >> 31) & 1);
            int chunkX = ((x + negativityBoost) / chunkSize) - negativityBoost;
            negativityBoost = (((y & int.MinValue) >> 31) & 1);
            int chunkY = ((y + negativityBoost) / chunkSize) - negativityBoost;
            int localCellX = x - (chunkX * chunkSize);
            int localCellY = y - (chunkY * chunkSize);
            return Utils.CoordToIndex(FastMath.Abs(localCellX), FastMath.Abs(localCellY), chunkSize);
        }
        */
    }
}