using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace Elanetic.Tools
{
    /// <summary>
    /// Resuseable version of the System.Random class. Has similiar functions to UnityEngine.Random.
    /// </summary>
    public class FastRandom
    {
        private const int MBIG = Int32.MaxValue;
        private const int MSEED = 161803398;


        private int m_INext;
        private int m_INextP;
        private int[] m_SeedArray = new int[56];

        //Starting values
        private int m_StartINext;
        private int m_StartINextP;
        private int[] m_StartSeedArray = new int[56];

        public FastRandom() : this(Environment.TickCount)
        {
        }

        public FastRandom(int seed)
        {
            SetSeed(seed);
        }

        /// <summary>
        /// Set the seed of this random generator and resets it to its initial state.
        /// </summary>
        public void SetSeed(int seed)
        {
            int ii;
            int mj, mk;

            //Initialize our Seed array.
            //This algorithm comes from Numerical Recipes in C (2nd Ed.)
            int subtraction = (seed == Int32.MinValue) ? Int32.MaxValue : FastMath.Abs(seed);
            mj = MSEED - subtraction;
            m_SeedArray[55] = mj;
            mk = 1;
            for(int i = 1; i < 55; i++)
            {  //Apparently the range [1..55] is special (Knuth) and so we're wasting the 0'th position.
                ii = (21 * i) % 55;
                m_SeedArray[ii] = mk;
                mk = mj - mk;
                if(mk < 0) mk += MBIG;
                mj = m_SeedArray[ii];
            }
            for(int k = 1; k < 5; k++)
            {
                for(int i = 1; i < 56; i++)
                {
                    m_SeedArray[i] -= m_SeedArray[1 + (i + 30) % 55];
                    if(m_SeedArray[i] < 0) m_SeedArray[i] += MBIG;
                }
            }
            m_INext = 0;
            m_INextP = 21;

            //Set starting values
            m_StartINext = m_INext;
            m_StartINextP = m_INextP;
            Array.Copy(m_SeedArray, m_StartSeedArray, 56);
        }

        /// <summary>
        /// Resets the seed to the starting values so that they can be resused. Faster than using SetSeed to reset to the starting values.
        /// </summary>
        public void Reset()
        {
            m_INext = m_StartINext;
            m_INextP = m_StartINextP;
            Array.Copy(m_StartSeedArray, m_SeedArray, 56);
        }

        /// <summary>
        /// Get a random positive integer including 0.
        /// </summary>
        public int GetInt()
        {
            return InternalSample();
        }

        /// <summary>
        /// Get a random float value from 0.0f to 1.0f.
        /// </summary>
        public float GetFloat()
        {
            return (float)GetDouble();
        }

        /// <summary>
        /// Get a random double value from 0.0f to 1.0f.
        /// </summary>
        public double GetDouble()
        {
            //Including this division at the end gives us significantly improved
            //random number distribution.
            return (InternalSample() * (1.0 / MBIG));
        }

        /// <summary>
        /// Get an integer within the specified range. Max is exclusive.
        /// </summary>
        public int Range(int minValue, int maxValue)
        {
            long range = (long)maxValue - minValue;
            if (range <= (long)Int32.MaxValue)
            {
                return ((int)(GetDouble() * range) + minValue);
            }
            else
            {
                return (int)((long)(GetSampleForLargeRange() * range) + minValue);
            }
        }

        /// <summary>
        /// Get a float within the specified range. Max is inclusive.
        /// </summary>
        public float Range(float minValue, float maxValue)
        {
            double range = (double)maxValue - minValue;
            if(range <= (double)float.MaxValue)
            {
                return ((float)(GetDouble() * range)) + minValue;
            }
            else
            {
                return (float)((GetSampleForLargeRange() * range) + minValue);
            }
        }

        /// <summary>
        /// Fill a buffer full of random bytes.
        /// </summary>
        public void GetBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(InternalSample() % (Byte.MaxValue + 1));
            }
        }

        private int InternalSample()
        {
            int retVal;
            int locINext = m_INext;
            int locINextp = m_INextP;

            if(++locINext >= 56) locINext = 1;
            if(++locINextp >= 56) locINextp = 1;

            retVal = m_SeedArray[locINext] - m_SeedArray[locINextp];

            if(retVal == MBIG) retVal--;
            if(retVal < 0) retVal += MBIG;

            m_SeedArray[locINext] = retVal;

            m_INext = locINext;
            m_INextP = locINextp;

            return retVal;
        }

        private double GetSampleForLargeRange()
        {
            // The distribution of double value returned by Sample 
            // is not distributed well enough for a large range.
            // If we use Sample for a range [Int32.MinValue..Int32.MaxValue)
            // We will end up getting even numbers only.

            int result = InternalSample();
            // Note we can't use addition here. The distribution will be bad if we do that.
            bool negative = (InternalSample() % 2 == 0) ? true : false;  // decide the sign based on second sample
            if(negative)
            {
                result = -result;
            }
            double d = result;
            d += (Int32.MaxValue - 1); // get a number in range [0 .. 2 * Int32MaxValue - 1)
            d /= 2 * (uint)Int32.MaxValue - 1;
            return d;
        }
    }
}
