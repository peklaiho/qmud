using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class ZoneTemplate
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public Dictionary<int, int> Monsters { get; private set; }

		public ZoneTemplate ()
		{
			Monsters = new Dictionary<int, int>();
		}

		public ZoneTemplate (int id, string name, int width, int height)
			: this()
		{
			Id = id;
			Name = name;
			Width = width;
			Height = height;
		}
	}
}
	