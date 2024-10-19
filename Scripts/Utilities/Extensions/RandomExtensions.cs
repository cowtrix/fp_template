using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPTemplate.Utilities.Extensions
{
    public static class RandomExtensions
	{
		public static T WeightedRandom<T>(IEnumerable<T> values, IEnumerable<float> weights)
		{
			var max = weights.Sum();
			var roll = Random.Range(0, max);
			var accum = 0f;
			var index = 0;
			foreach (var w in weights)
			{
				accum += w;
				if(accum >= roll)
				{
					break;
				}
				index++;
            }
			return values.ElementAt(index);
		}

        public static bool Flip()
		{
			return Random.value > .54f;
		}

    }
}