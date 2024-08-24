using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class Find
	{
		public static List<Living> CombineLists(List<Player> list1, List<Monster> list2)
		{
			List<Living> output = new List<Living>();

			output.AddRange(list1);
			output.AddRange(list2);
			
			return output;
		}
		
		public static List<Item> CombineLists(List<Item> list1, List<Item> list2)
		{
			List<Item> output = new List<Item>();

			output.AddRange(list1);
			output.AddRange(list2);
			
			return output;
		}
		
		public static Item FindItem(List<Item> list, string name, Living searcher)
		{
			int skipOver = 0;
			
			if (ParseDotKeyword(ref name, ref skipOver) == false)
			{
				return null;
			}

			List<Item> candidates = list.FindAll(x => (searcher == null || searcher.CanSee(x)) &&
			                                      Utils.StartsWith(x.Template.Keywords, name));

			return skipOver < candidates.Count ? candidates[skipOver] : null;
		}
		
		public static Living FindLiving(List<Living> list, string name, Living searcher)
		{
			int skipOver = 0;
			
			if (ParseDotKeyword(ref name, ref skipOver) == false)
			{
				return null;
			}

			List<Living> candidates = list.FindAll(x => (searcher == null || searcher.CanSee(x)) &&
			                                        ((x.IsPlayer() && Utils.StartsWith(((Player)x).Name, name)) ||
			                                        (x.IsMonster() && Utils.StartsWith(((Monster)x).Template.Keywords, name))));

			return skipOver < candidates.Count ? candidates[skipOver] : null;
		}
		
		private static bool ParseDotKeyword(ref string keyword, ref int skipOver)
		{
			int tempSkipOver;
			
			if (keyword.Contains("."))
			{
				string[] tmp = keyword.Split('.');
				
				if (tmp.Length != 2 || tmp[0].Length == 0 || tmp[1].Length == 0)
				{
					return false;
				}

				if (!Int32.TryParse(tmp[0], out tempSkipOver))
				{
					return false;
				}
				
				// Check some limits
				if (tempSkipOver <= 0 || tempSkipOver > 100)
				{
					return false;
				}
				
				// All seems ok, set the new values to our reference parameters
				
				// We skip over one less than the argument so for 2.sword we skip over 1 sword etc.
				skipOver = tempSkipOver - 1;
				keyword = tmp[1];
				
				return true;
			}
			else
			{
				return true;
			}
		}
	}
}
