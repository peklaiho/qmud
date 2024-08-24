using System;
using System.Collections;

namespace QMud.Core
{
	public class Random
	{
		private static System.Random rng = new System.Random();
		
		public static bool Chance(int successPercentage)
		{
			if (successPercentage >= 100)
			{
				return true;
			}
			else if (successPercentage <= 0)
			{
				return false;
			}
			
			int randVal = Range(0, 99);
			
			// Always 1 percent chance to fail, represented by 0
			if (randVal == 0)
			{
				return false;
			}
			
			return (randVal <= successPercentage);
		}
		
		public static int Range(int min, int max)
		{
			if (min == max)
			{
				return min;
			}
				
			return rng.Next(min, max + 1);
		}
	}
}
