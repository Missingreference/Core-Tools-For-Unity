using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

static public class RandomWeighted
{
	static public int Get(params float[] weights)
	{
		if(weights.Length == 0)
		{
			Debug.LogError("Must have at least 1 parameter in the weights.");
			return -1;
		}
		
		float total = 0;
		for(int i = 0; i < weights.Length; i++)
		{
			total += Mathf.Max(0.0f, weights[i]);
		}

		//If all the values given are 0, make all values the same weight
		if(total == 0.0f)
		{
			for(int i = 0; i < weights.Length; i++)
			{
				weights[i] = 1.0f;
				total += 1.0f;
			}
		}
		
		
		//Debug.Log("Weight Count: " + weights.Length + " Total: " + total);
		
		SortedDictionary<float, int> normalizedWeights = new SortedDictionary<float, int>();
		float tracker = 0.0f;
		for(int h = 0; h < weights.Length; h++)
		{
			tracker += weights[h] / total;
			normalizedWeights.Add(tracker, h);
		}


		float rand = Random.Range(0.0f, 1.0f);
		List<float> keys = normalizedWeights.Keys.ToList();
		for(int j = 0; j < keys.Count; j++)
		{
			//Debug.Log("Key(" + j + "): " + keys[j].ToString("F4"));
			if(keys[j] < rand)
			{
				//Debug.Log(j + " removed: " + keys[j]);
				keys.RemoveAt(j);
				j--;
			}
		}
			
		//Debug.Log("Rand: " + rand + " keys.Max: " + keys.Min());
		return normalizedWeights[keys.Min()];
	}

    //Negative values get treated with a weight of 0
    static public int Get(float value1, float value2)
	{
        if(value1 <= 0)
		{
            if(value2 <= 0) //Equal weights mean equal chances
				return Random.Range(0, 2);
			else
				return 1;
		}
        else if(value2 <= 0)
            return 0;
        if(Random.Range(0.0f, value1+value2) <= value1)
            return 0;
		return 1;
	}
}
