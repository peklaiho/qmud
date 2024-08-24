using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class Item
	{
		public ItemTemplate Template { get; private set; }
		
		// Only one of the following associations can be set at any one time
		public Item InItem { get; set; }
		public Room InRoom { get; set; }
		public Living CarriedBy { get; set; }
		public Living WornBy { get; set; }
		
		public List<Item> Contents { get; private set; }
		
		public Item (ItemTemplate newTemplate)
			: base()
		{
			Template = newTemplate;
			
			Contents = new List<Item>();
		}

		public Item FindItem(string keyword, Living searcher)
		{
			return Find.FindItem(Contents, keyword, searcher);
		}

		public bool IsContainer()
		{
			return true;
		}

		public bool IsNoPickup()
		{
			return false;
		}

		public string Location()
		{
			if (InItem != null)
			{
				return "in container";
			}
			else if (InRoom != null)
			{
				return "on ground";
			}
			else if (CarriedBy != null)
			{
				return "carried";
			}
			else if (WornBy != null)
			{
				return "worn";
			}
			
			return "unknown";
		}
	}
}
