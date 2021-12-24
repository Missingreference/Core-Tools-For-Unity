#if UNITY_COLLECTIONS //Defined in Elanetic.Tools assembly definition file
using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Elanetic.Tools
{

    /// <summary>
    /// Similar to OutwardList but is a struct and uses Unity's Collections for internal storage. Used for Unity DOTS.
    /// More info on implementation in OutwardList.
    /// </summary>
    public struct NativeOutwardList<T> : IDisposable where T : unmanaged
    {
        private NativeList<T> m_List;
        private Allocator m_Allocator;

        public NativeOutwardList(Allocator allocator)
        {
            m_Allocator = allocator;
            m_List = new NativeList<T>(m_Allocator);
        }

        public int CoordToIndex(int2 coord)
        {
            if(coord.x == 0 && coord.y == 0) return 0;

            int absX = math.abs(coord.x);
            int absY = math.abs(coord.y);

            if(absX > absY)
            {
                if(coord.y == 0)
                {
                    if(coord.x > 0)
                    {
                        return (((int)((absX / 2.0f) * (1.0f + absX))) * 8) - (absX * 8) + 2;
                    }
                    return (((int)((absX / 2.0f) * (1.0f + absX))) * 8) - (absX * 8) + 1;
                }
                else
                {
                    if(coord.x > 0)
                    {
                        if(coord.y > 0)
                            return ((((int)((absX / 2.0f) * (1.0f + absX))) * 8) - (absX * 8) + 5 + ((absY - 1) * 8)) + 1;
                        return ((((int)((absX / 2.0f) * (1.0f + absX))) * 8) - (absX * 8) + 5 + ((absY - 1) * 8)) + 3;
                    }

                    if(coord.y > 0)
                        return ((((int)((absX / 2.0f) * (1.0f + absX))) * 8) - (absX * 8) + 5 + ((absY - 1) * 8));
                    return ((((int)((absX / 2.0f) * (1.0f + absX))) * 8) - (absX * 8) + 5 + ((absY - 1) * 8)) + 2;
                }
            }
            else
            {
                if(coord.x == 0)
                {
                    if(coord.y > 0)
                    {
                        return (((int)((absY / 2.0f) * (1.0f + absY))) * 8) - (absY * 8) + 3;
                    }
                    return (((int)((absY / 2.0f) * (1.0f + absY))) * 8) - (absY * 8) + 4;
                }
                else if(absX == absY)
                {
                    if(coord.x > 0)
                    {
                        if(coord.y > 0)
                            return (((int)((absY / 2.0f) * (1.0f + absY))) * 8) - 2;
                        return (((int)((absY / 2.0f) * (1.0f + absY))) * 8);
                    }

                    if(coord.y > 0)
                        return (((int)((absY / 2.0f) * (1.0f + absY))) * 8) - 3;
                    return (((int)((absY / 2.0f) * (1.0f + absY))) * 8) - 1;
                }
                else
                {
                    if(coord.x > 0)
                    {
                        if(coord.y > 0)
                            return ((((int)((absY / 2.0f) * (1.0f + absY))) * 8) - (absY * 8) + 9 + ((absX - 1) * 8)) + 1;
                        return ((((int)((absY / 2.0f) * (1.0f + absY))) * 8) - (absY * 8) + 9 + ((absX - 1) * 8)) + 3;
                    }

                    if(coord.y > 0)
                        return (((int)((absY / 2.0f) * (1.0f + absY))) * 8) - (absY * 8) + 9 + ((absX - 1) * 8);
                    return ((((int)((absY / 2.0f) * (1.0f + absY))) * 8) - (absY * 8) + 9 + ((absX - 1) * 8)) + 2;
                }
            }
        }

        public void SetItem(int2 coord, T item)
        {
            SetItem(CoordToIndex(coord), item);
        }

        public void SetItem(int index, T item)
        {
            while(index > m_List.Length - 1)
            {
                m_List.Add(default);
            }
            m_List[index] = item;
        }

        public T GetItem(int2 coord)
        {
            return GetItem(CoordToIndex(coord));
        }

        public T GetItem(int index)
        {
            if(index > m_List.Length - 1)
                return default;
            return m_List[index];
        }

        public void Dispose()
        {
            m_List.Dispose();
        }
    }
}
#endif