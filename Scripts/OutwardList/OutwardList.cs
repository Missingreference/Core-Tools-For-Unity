using System;
using System.Collections.Generic;
//using UnityEngine;

namespace Isaac.Tools
{

    /// <summary>
    /// This class creates a list to store values on a 2D grid. Lookups are faster than a Dictionary, especially in combination with ChunkedOutwardList.
    /// Downside is setting coordinates far from (0,0) allocates a lot of memory depending on the type.
    /// </summary>
    public static class OutwardList
    {
        static public int CoordToIndex(int x, int y)
        {
            if(x == 0 && y == 0) return 0;

            m_AbsX = Math.Abs(x);
            m_AbsY = Math.Abs(y);
            /*
            int bracketIndex = Mathf.Max(Mathf.Abs(coord.x), Mathf.Abs(coord.y));
            //Bracket index is X, Level is Y
            int bracketMax = 0;
            for(int i = 1; i <= bracketIndex; i++)
            {
                bracketMax += i * 8;
            }

            //Debug.Log("Bracket Max before: " + bracketMax);
            Debug.Log("Coord: (" + coord.x + "," + coord.y + ") | Index: " + bracketIndex + " | AbsX: " + absX + " | AbsY: " + absY);

            //return 0;
            //bracketMax = Mathf.CeilToInt(bracketIndex / 2.0f) * (1 + bracketIndex) * 8;
            //Debug.Log("Bracket Max after: " + bracketMax);
            int bracketMin = bracketMax - (bracketIndex * 8) + 1;


            //Debug.Log("X: " + coord.x + ", Y: " + coord.y + " | Min: " + bracketMin + " | Max: " + bracketMax + " | Index: " + bracketIndex);
            //level is Mathf.Abs(coord.y)
            */
            if(m_AbsX > m_AbsY)
            {
                //Debug.Log("Max: " + bracketMax + " | " + ((int)(((absX / 2.0f)) * (1.0f + absX))) * 8);
                //Debug.Log("Min: " + bracketMin + " | " + (((int)((absX / 2.0f) * (1.0f + absX))*8) - (absX * 8) + 1));
                //bracketMax: ((int)((absX / 2.0f) * (1.0f + absX)) * 8)
                //bracketMin: ((int)((absX / 2.0f) * (1.0f + absX))) - (absX * 8) + 1
                //startingLevelIndex: ((int)((absX / 2.0f) * (1.0f + absX))) - (absX * 8) + 1 + 4 + ((absX-1)* 8)
                if(y == 0)
                {
                    //The first four indexes of the bracket
                    if(x > 0)
                    {
                        return (((int)((m_AbsX / 2.0f) * (1.0f + m_AbsX))) * 8) - (m_AbsX * 8) + 2; //bracketMin + 1;
                    }
                    return (((int)((m_AbsX / 2.0f) * (1.0f + m_AbsX))) * 8) - (m_AbsX * 8) + 1; //bracketMin
                }
                else
                {
                    //int startingLevelIndex = bracketMin + 4 + ((Mathf.Abs(coord.y)-1) * 8);
                    if(x > 0)
                    {
                        if(y > 0)
                            return ((((int)((m_AbsX / 2.0f) * (1.0f + m_AbsX))) * 8) - (m_AbsX * 8) + 5 + ((m_AbsY - 1) * 8)) + 1; //startingLevelIndex + 1;
                        return ((((int)((m_AbsX / 2.0f) * (1.0f + m_AbsX))) * 8) - (m_AbsX * 8) + 5 + ((m_AbsY - 1) * 8)) + 3;
                    }

                    if(y > 0)
                        return ((((int)((m_AbsX / 2.0f) * (1.0f + m_AbsX))) * 8) - (m_AbsX * 8) + 5 + ((m_AbsY - 1) * 8));
                    return ((((int)((m_AbsX / 2.0f) * (1.0f + m_AbsX))) * 8) - (m_AbsX * 8) + 5 + ((m_AbsY - 1) * 8)) + 2;
                }
            }
            else
            {
                //Debug.Log("Max: " + bracketMax + " | " + (((int)((absY / 2.0f) * (1.0f + absY)))*8));
                //Debug.Log("Min: " + bracketMin + " | " + (((int)((absY / 2.0f) * (1.0f + absY))*8) - (absY * 8) + 1));
                //bracketMax: (((int)((absY / 2.0f) * (1.0f + absY)))*8)
                //bracketMin: ((int)((absY / 2.0f) * (1.0f + absY))) - (absY * 8) + 1
                //startingLevelIndex: ((int)((absY / 2.0f) * (1.0f + absY))) - (absY * 8) + 1 + 8 + ((absY-1)* 8)

                //Bracket index is Y, Level is X
                if(x == 0)
                {
                    //The first four indexes of the bracket
                    if(y > 0)
                    {
                        return (((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - (m_AbsY * 8) + 3;//bracketMin + 2;
                    }
                    return (((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - (m_AbsY * 8) + 4;
                }
                else if(m_AbsX == m_AbsY)
                {
                    //The last four indexes of the bracket
                    if(x > 0)
                    {
                        if(y > 0)
                            return (((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - 2; //bracketMax - 2;
                        return (((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8);
                    }

                    if(y > 0)
                        return (((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - 3;
                    return (((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - 1;
                }
                else
                {
                    //int startingLevelIndex = bracketMin + 8 + ((Mathf.Abs(coord.x)-1) * 8);
                    if(x > 0)
                    {
                        if(y > 0)
                            return ((((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - (m_AbsY * 8) + 9 + ((m_AbsX - 1) * 8)) + 1;
                        return ((((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - (m_AbsY * 8) + 9 + ((m_AbsX - 1) * 8)) + 3;
                    }

                    if(y > 0)
                        return (((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - (m_AbsY * 8) + 9 + ((m_AbsX - 1) * 8);
                    return ((((int)((m_AbsY / 2.0f) * (1.0f + m_AbsY))) * 8) - (m_AbsY * 8) + 9 + ((m_AbsX - 1) * 8)) + 2;
                }
            }
        }

        //Does doing this make it not thread-safe? Regardless OutwardList internally uses List.
        static private int m_AbsX = 0;
        static private int m_AbsY = 0;
    }

    public class OutwardList<T>
    {
        private readonly List<T> m_List = new List<T>();

        public OutwardList()
        {
            /* Test Case
            string s = "";
            for(int i = 0; i < 10; i++)
            {
                Vector2Int randCoord = new Vector2Int(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3));
                s += "Input: (" + randCoord.x + "," + randCoord.y + ") | Output: " + CoordToIndex(randCoord) + "\n"; 
            }
            Debug.Log(s);
            */
        }

        /// <summary>
        /// Add the specified item to the list at the destination.
        /// Note: Will fill the internal list(allocating) until it reaches the coord's destination index.
        /// Note: Will replace anything existing at the coord.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="item"></param>
        public void SetItem(int x, int y, T item) //Should this return the index it placed it at?
        {
            SetItem(OutwardList.CoordToIndex(x, y), item);
        }

        public void SetItem(int index, T item)
        {
            while(index > m_List.Count - 1)
            {
                m_List.Add(default);
            }
            m_List[index] = item;
        }

        public T GetItem(int x, int y)
        {
            return GetItem(OutwardList.CoordToIndex(x, y));
        }

        public T GetItem(int index)
        {
            if(index > m_List.Count - 1)
                return default;
            return m_List[index];
        }
    }
}
/*
3,2

4,8,6,
[45] [41] [33] [27] [34] [42] [46]

[37] [21] [17] [11] [18] [22] [38]

[29] [13] [5]  [3]  [6]  [14] [30]

[25] [9]  [1]  [0]  [2]  [10] [26]

[31] [15] [7]  [4]  [8]  [16] [32]

[39] [23] [19] [12] [20] [24] [40]

[47] [43] [35] [28] [36] [44] [48]
 
1 - 8 - 16 - 24 - 32 - 40

Brackets:
[0] - [1-8] - [9-24] - [25-48]
 1  -   8   -   16   -   24
^Sizes

bracketTotal = 

Index =  (1+2+3...)

Levels is the different distances within a bracket ordered from shortest distance to longest
The second bracket(1-8) would have 2 levels both containing 4 indexes (1-4) & (5-8)
The third bracket (9-24) would have 3 levels, the first level has 4 indexes(9-12), the second level has 8 indexes(13-20) and the third level has 4 indexes(the corners 21-24)
Only the first and last levels in each bracket contain 4 indexes, the rest contain 8

[3] [2] [1] [0] [1] [2] [3]

[2] [2] [1] [0] [1] [2] [2]

[1] [1] [1] [0] [1] [1] [1]

[0] [0] [0] [0] [0] [0] [0]

[1] [1] [1] [0] [1] [1] [1]

[2] [2] [1] [0] [1] [2] [2]

[3] [2] [1] [0] [1] [2] [3]

*/
