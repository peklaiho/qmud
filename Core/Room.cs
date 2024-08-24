using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace QMud.Core
{
	public class Room
	{
		// Runtime attributes
		public Zone Zone { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public bool[] Exits { get; private set; }

		public List<ZoneEntrance> Entrances { get; private set; }
		public List<Player> Players { get; private set; }
		public List<Monster> Monsters { get; private set; }
		public List<Item> Items { get; private set; }

		public Room (Zone zone, int x, int y)
		{
			Zone = zone;
			X = x;
			Y = y;
			Exits = new bool[EnumNames.DirNames.Length];

			Entrances = new List<ZoneEntrance>();
			Players = new List<Player>();
			Monsters = new List<Monster>();
			Items = new List<Item>();
		}

		public Item FindItem(string keyword, Living searcher)
		{
			return Find.FindItem(Items, keyword, searcher);
		}

		public Living FindLiving(string name, Living searcher)
		{
			return Find.FindLiving(Find.CombineLists(Players, Monsters), name, searcher);
		}

		public string GetLocationString()
		{
			return "[" + X + "," + Y + "]";
		}

		public Room AdjacentRoom(Directions dir)
		{
			switch (dir)
			{
				case Directions.North:
					return Zone.GetRoom(X, Y - 1);
				case Directions.East:
					return Zone.GetRoom(X + 1, Y);
				case Directions.South:
					return Zone.GetRoom(X, Y + 1);
				case Directions.West:
					return Zone.GetRoom(X - 1, Y);
			}

			return null;
		}
	}
}
