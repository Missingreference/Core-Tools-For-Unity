using System;

namespace Elanetic.Tools
{

	static public class RandomWeighted
	{

		static private FastRandom m_FastRandom;

        /// <summary>
        /// Get the index of a randomly chosen inputted value. Each value is weighted where bigger values will have a bigger chance of being chosen.
        /// Negative values are treated with a weight of 0. If all values have the same weight(including each having a weight of 0) they will have an equal chance of being chosen.
        /// </summary>
		static public int Get(params float[] weights)
		{
#if SAFE_EXECUTION
			if(weights.Length == 0)
			{
				throw new ArgumentException("Must have more than 0 parameters inputted.", nameof(weights));
			}
#endif

			if(m_FastRandom == null) m_FastRandom = new FastRandom();

			float total = 0;
			for(int i = 0; i < weights.Length; i++)
			{
				float value = weights[i];
				if(value < 0) continue;
				total += value;
			}

			//If all the values given are 0, make all values the same weight
			if(total == 0.0f)
			{
				return m_FastRandom.Range(0, weights.Length);
			}

			total = m_FastRandom.Range(0.0f, total);

			float tracker = 0;
            for(int i = 0; i < weights.Length; i++)
            {
				if(weights[i] <= 0) continue;
				tracker += weights[i];
				if(tracker >= total) return i;
            }

			return weights.Length-1;
		}

        //If you use this, nice.
		static public int Get(float value) => 0;

		static public int Get(float value0, float value1)
		{
			if(value0 <= 0)
			{
				if(value1 <= 0) //Equal weights mean equal chances
					return m_FastRandom.Range(0, 2);
				else
					return 1;
			}
			else if(value1 <= 0)
				return 0;
			if(m_FastRandom.Range(0.0f, value0 + value1) <= value0)
				return 0;
			return 1;
		}
	}
}