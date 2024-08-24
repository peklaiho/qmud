using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class Party
	{
		public static List<Party> PartyList = new List<Party>();

		private static int NextId = 1;

		public int Id { get; private set; }
		public string Name { get; private set; }
		public List<Player> Players { get; private set; }
		public List<Monster> Monsters { get; private set; }

		public Party (string name)
		{
			Id = NextId++;
			Name = name;
			Players = new List<Player>();
			Monsters = new List<Monster>();

			PartyList.Add(this);
		}

		public static Party FindById (int id)
		{
			return PartyList.Find(x => x.Id == id);
		}
	}
}
