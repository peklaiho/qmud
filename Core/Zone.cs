using System;
using System.Linq;
using System.Collections.Generic;

namespace QMud.Core
{
	public abstract class Zone
	{
		public ZoneTemplate Template { get; private set; }

		public Room StartingRoom { get; protected set; }
		public Room[,] Rooms { get; private set; }

		public void FromTemplate (ZoneTemplate template)
		{
			Template = template;

			Rooms = new Room[Template.Width, Template.Height];

			for (int x = 0; x < Template.Width; x++)
			{
				for (int y = 0; y < Template.Height; y++)
				{
					Rooms[x, y] = new Room(this, x, y);
				}
			}

			Initialize();
		}

		public abstract void Initialize ();

		public Room GetRoom(int x, int y)
		{
			if (x >= 0 && y >= 0 && x < Template.Width && y < Template.Height)
			{
				return Rooms[x, y];
			}

			return null;
		}

		public Room GetRandomRoom()
		{
			return GetRoom(Random.Range(0, Template.Width - 1), Random.Range(0, Template.Height - 1));
		}
	}
}
