using System;
using System.Runtime.CompilerServices;

namespace Elanetic.Tools
{
    /// <summary>
    /// Usage looks and acts like a normal array however the difference is that there are performance benefits when removing items unlike normal arrays or lists,
    /// especially from the start of the array where shifting items does not occur or significantly less shifting when removing items from the first half of the array.
    /// However general retrieval of items always incurs a math operation to retrieve the appropriate index: actualIndex = (m_StartIndex + index) % m_Array.Length
    /// </summary>
    public class CircleArray<T>
    {
        public int count => m_Count;
        public int capacity => m_Array.Length;

        public int rawStartIndex => m_StartIndex;
        public T[] rawArray => m_Array;
        
        private int m_StartIndex = 0;
        private T[] m_Array;
        private int m_Count = 0;
        
        public CircleArray(int length)
        {
            m_Array = new T[length];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddItem(T value)
        {
#if DEBUG
            if(m_Count == m_Array.Length)
                throw new IndexOutOfRangeException("Circle array has reached the capacity of the array");
#endif
            m_Array[IndexToAdjustedIndex(m_Count)] = value;
            m_Count++;
        }

        public void AddItems(params T[] values)
        {
#if DEBUG
            if(m_Count + values.Length > m_Array.Length)
                throw new IndexOutOfRangeException("Circle array does not have enough capacity for '" + values.Length.ToString() + "' items.");
#endif
            int destinationIndex = m_StartIndex + m_Count;
            for(int i = 0; i < values.Length; i++)
            {
                m_Array[(destinationIndex + i) % m_Array.Length] = values[i];
            }
            m_Count += values.Length;
        }

        public void RemoveAt(int index) => RemoveAt(index, 1);

        public void RemoveAt(int index, int count)
        {
#if DEBUG
            if(index < 0 || index >= m_Count)
                throw new IndexOutOfRangeException("Inputted index must be from 0 to less than the count of the circle array.");
            if(count <= 0)
                throw new ArgumentException("Count must be more than 0.");
            if(index + count > m_Count)
                throw new ArgumentException("Inputted index + count surpasses the number of items in the circle array.");
#endif
            
            if(index == 0) //Remove front
            {
                m_StartIndex = (m_StartIndex + count) % m_Array.Length;
            }
            else if(index == m_Count - count) //Remove end
            {
                //No shifting needed
            }
            else
            {
                //Shifting needed
                
                int leftSize = index;
                int rightSize = m_Count - (index + count);
                
                if(leftSize < rightSize)
                {
                    int sourceIndex = m_StartIndex;
                    int destinationIndex = m_StartIndex + count;
                    for(int i = 0; i < leftSize; i++)
                    {
                        m_Array[(destinationIndex + i) % m_Array.Length] = m_Array[(sourceIndex + i) % m_Array.Length];
                    }
                    m_StartIndex = (m_StartIndex + count) % m_Array.Length;
                }
                else
                {
                    int sourceIndex = m_StartIndex + index + count;
                    int destinationIndex = m_StartIndex + index;
                    for(int i = 0; i < rightSize; i++)
                    {
                        m_Array[(destinationIndex + i) % m_Array.Length] = m_Array[(sourceIndex + i) % m_Array.Length];
                    }
                }
            }

            m_Count -= count;
        }

        public void Clear()
        {
            //m_StartIndex = 0; //Does this have any benefit?
            m_Count = 0;
        }
        
        /// <summary>
        /// Move the start index to another index of the items
        /// </summary>
        public void Shift(int count)
        {
            if(m_Count < m_Array.Length)
            {
                int fixedAmountToMove = ((count % m_Count) + m_Count) % m_Count;
                if(fixedAmountToMove < m_Count / 2)
                {
                    //Shift first half
                    for(int i = 0; i < fixedAmountToMove; i++)
                    {
                        m_Array[IndexToAdjustedIndex(m_Count + i)] = m_Array[IndexToAdjustedIndex(i)];
                    }
                    m_StartIndex = IndexToAdjustedIndex(fixedAmountToMove);
                }
                else
                {
                    //Shift second half
                    int countToMove = m_Count - fixedAmountToMove;
                    int destinationIndex = m_Array.Length - countToMove;
                    for(int i = countToMove - 1; i >= 0; i--)
                    {
                        m_Array[IndexToAdjustedIndex(destinationIndex + i)] = m_Array[IndexToAdjustedIndex(fixedAmountToMove + i)];
                    }
                    m_StartIndex = IndexToAdjustedIndex(destinationIndex);
                }
            }
            else
            {
                //No shifting needed, just a start index move using math
                m_StartIndex = (((m_StartIndex+count) % m_Array.Length) + m_Array.Length) % m_Array.Length;
            }
        }

        /// <summary>
        /// Replace internal array with a new sized array and copies the tracked contents to the new array.
        /// </summary>]
        /* TODO
        public void SetCapacity(int newCapcity)
        {
#if DEBUG
            if(newCapcity <= 0)
                throw new ArgumentException("Inputted capacity must be more than zero.");
#endif
            
            T[] newArray = new T[newCapcity];
            if(newCapcity < count)
            {
                Array.Copy(m_Array, m_StartIndex, newArray, 0, newCapcity);
                
            }
            else
            {
                Array.Copy(m_Array, m_StartIndex, newArray, 0, count);
            }
            
            m_Array = newArray;
            m_StartIndex = 0;
        }
        */

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if DEBUG
                if(m_Count == 0)
                    throw new IndexOutOfRangeException("Cannot retrieve item. Circle array has not items.");
                if(index < 0 || index >= m_Count)
                    throw new IndexOutOfRangeException("Inputted index must be from 0 to less than the count of the circle array.");
#endif
                return m_Array[IndexToAdjustedIndex(index)];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
#if DEBUG
                if(m_Count == 0)
                    throw new IndexOutOfRangeException("Cannot set item. Circle array has not items.");
                if(index < 0 || index >= m_Count)
                    throw new IndexOutOfRangeException("Inputted index must be from 0 to less than the count of the circle array.");
#endif
                m_Array[IndexToAdjustedIndex(index)] = value;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int IndexToAdjustedIndex(int index)
        {
            return (m_StartIndex + index) % m_Array.Length;
        }
    }
}