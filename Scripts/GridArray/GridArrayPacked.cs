//Interpolation Search implementation sourced from https://geeksforgeeks.org/interpolation-search/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Elanetic.Tools
{
    /// <summary>
    /// Makes it so that there is no wasted space unlike regular GridArray. The goal for this version is to remove the wasted space of a GridArray while still having items ordered in the grid pattern implemented in GridArray.
    /// This comes at the cost of lookups being very slow due to searching as well as adding and removing items cause ranges of item shifts.
    /// Basically meaning SetItem and GetItem are slow but the internal array is ordered by the grid pattern(GridArray.CellToIndex).
    /// Should only be used in small amounts of data. The more elements exist, the slower it performs.
    /// </summary>
    public class GridArrayPacked<T>
    {

        public int count { get; private set; } = 0;
        public int allocatedSize { get; private set; }

        public PackedElement[] m_Array;
        private int m_ResizeAmount = 16;

        public GridArrayPacked(int initialSize=16, int resizeAmount=16)
        {
            allocatedSize = initialSize;
            m_Array = new PackedElement[initialSize];
            m_ResizeAmount = resizeAmount;
        }

        /// <summary>
        /// Set the item at the grid index. The index must come from GridArray.CellToIndex. Overwrites any existing element.
        /// Setting an item to null does not remove it from the internal array. Use GridArrayPacked.RemoveItem.
        /// </summary>
        public void SetItem(int gridIndex, T item)
        {
            //Interpolation Search

            int min = 0;
            int max = count - 1;

            int minValue = 0; 
            int maxValue = 0;
            int positionValue;

            if(count > 0)
            {
                minValue = m_Array[min].gridIndex;
                maxValue = m_Array[max].gridIndex;
            }

            while(min <= max && gridIndex >= minValue && gridIndex <= maxValue)
            {
                if(min == max)
                {
                    if(minValue == gridIndex)
                    {
                        SetItemDirectly(min, item);
                        return;
                    }

                    //In testing this has never been hit
                    throw new NotImplementedException("Unimplemented case. Min: " + min.ToString() + " Max: " + max.ToString() + " Grid Index: " + gridIndex.ToString() + " Min Value: " + minValue.ToString() + " Max Value: " + maxValue.ToString() + " Array size: " + count.ToString());
                }

                int position = min + (((max - min) / (maxValue - minValue)) * (gridIndex - minValue));
                positionValue = m_Array[position].gridIndex;
                if(positionValue == gridIndex)
                {
                    SetItemDirectly(position, item);
                    return;
                }

                if(positionValue < gridIndex)
                    min = position + 1;
                else
                    max = position - 1;

                minValue = m_Array[min].gridIndex;
                maxValue = m_Array[max].gridIndex;
            }
            if(min > max || gridIndex < minValue)
            {
                InsertItem(min, gridIndex, item);
            }
            else if(gridIndex > maxValue)
            {
                InsertItem(max + 1, gridIndex, item);
            }
        }

        /// <summary>
        /// Set the item at the grid coordinate. Overwrites any existing element.
        /// Setting an item to null does not remove it from the internal array. Use GridArrayPacked.RemoveItem.
        /// </summary>
        public void SetItem(int cellX, int cellY, T item)
        {
            SetItem(GridArray.CellToIndex(cellX, cellY), item);
        }

        public void SetItemDirectly(int index, T item)
        {
            int gridIndex = m_Array[index].gridIndex;
            m_Array[index] = new PackedElement(gridIndex, item);
        }

        private void InsertItem(int index, int gridIndex, T item)
        {
            if(count + 1 > m_Array.Length)
            {
                //Resize
                PackedElement[] newArray = new PackedElement[count + m_ResizeAmount];
                Array.Copy(m_Array, 0, newArray, 0, m_Array.Length);
                m_Array = newArray;
            }
            
            for(int i = count; i > index; i--)
            {
                m_Array[i] = m_Array[i-1];
            }
            m_Array[index] = new PackedElement(gridIndex, item);

            count++;
        }

        public void RemoveItem(int gridIndex)
        {
            int min = 0;
            int max = count - 1;

            int minValue = 0;
            int maxValue = 0;
            int positionValue;

            if(count > 0)
            {
                minValue = m_Array[min].gridIndex;
                maxValue = m_Array[max].gridIndex;
            }

            while(min <= max && gridIndex >= minValue && gridIndex <= maxValue)
            {
                if(min == max)
                {
                    if(minValue == gridIndex)
                    {
                        RemoveItemDirectly(min);
                        return;
                    }

                    return; //Not found
                }

                int position = min + (((max - min) / (maxValue - minValue)) * (gridIndex - minValue));
                positionValue = m_Array[position].gridIndex;
                if(positionValue == gridIndex)
                {
                    RemoveItemDirectly(position);
                    return;
                }    

                if(positionValue < gridIndex)
                    min = position + 1;
                else
                    max = position - 1;

                minValue = m_Array[min].gridIndex;
                maxValue = m_Array[max].gridIndex;
            }

            //Not found
        }

        public void RemoveItem(int cellX, int cellY)
        {
            RemoveItem(GridArray.CellToIndex(cellX, cellY));
        }

        public void RemoveItemDirectly(int index)
        {
            count--;
            for(int i = index; i < count; i++)
            {
                m_Array[i] = m_Array[i + 1];
            }
        }

        /// <summary>
        /// Inputted gridIndex argument must be an index result from GridArray.CellToIndex.
        /// </summary>
        public T GetItem(int gridIndex)
        {
            int itemIndex = GetItemIndex(gridIndex);
            if(itemIndex < 0)
                return default;
            return m_Array[itemIndex].item;
        }


        public T GetItem(int cellX, int cellY)
        {
            return GetItem(GridArray.CellToIndex(cellX, cellY));
        }

        /// <summary>
        /// Get the direct index of an item from the internal array. Calling SetItem will invalidate the index and will point to another element or null reference.
        /// Returns -1 if not found.
        /// </summary>
        public int GetItemIndex(int gridIndex)
        {
            //Interpolation Search

            int min = 0;
            int max = count - 1;

            int minValue = 0;
            int maxValue = 0;
            int positionValue;

            if(count > 0)
            {
                minValue = m_Array[min].gridIndex;
                maxValue = m_Array[max].gridIndex;
            }

            while(min <= max && gridIndex >= minValue && gridIndex <= maxValue)
            {
                if(min == max)
                {
                    if(minValue == gridIndex)
                        return min;
                    return -1;
                }

                int position = min + (((max - min) / (maxValue - minValue)) * (gridIndex - minValue));
                positionValue = m_Array[position].gridIndex;
                if(positionValue == gridIndex)
                    return position;

                if(positionValue < gridIndex)
                    min = position + 1;
                else
                    max = position - 1; 

                minValue = m_Array[min].gridIndex;
                maxValue = m_Array[max].gridIndex;
            }
            return -1;
        }

        /// <summary>
        /// Get the item from the internal array at the specified index. Direct array call and nothing else.
        /// </summary>
        public T GetItemDirectly(int index)
        {
            return m_Array[index].item;
        }

        public string PrintArray()
        {
            string s = "Item Count: " + count.ToString() + "\n";
            for(int i = 0; i < count; i++)
            {
                s += "[" + i.ToString() + "][" + m_Array[i].gridIndex.ToString() + " => " + m_Array[i].item.ToString() + "]\n";
            }
            return s;
        }

        public struct PackedElement
        {
            public PackedElement(int gridIndex, T item)
            {
                this.gridIndex = gridIndex;
                this.item = item;
            }
            public int gridIndex;
            public T item;
        }
    }
}