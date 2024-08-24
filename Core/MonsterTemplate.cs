using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class MonsterTemplate
	{
		// Flags
		public const int Aggro = 0;		// a
		public const int Scavenger = 1; // b
		public const int Sentinel = 2;	// c

		// Attributes in main table
		public int Id { get; set; }
		public MonsterClass Class { get; set; }
		public BitArray Flags { get; private set; }
		public string Name { get; set; }
		public Genders Sex { get; set; }
		public string[] Keywords { get; private set; }
		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }

		// Runtime attributes
		public int Count { get; set; }

		public MonsterTemplate ()
		{
			Flags = new BitArray();
		}

		public string DbKeywords
		{
			get { return String.Join(",", Keywords); }
			set { Keywords = value.Split(','); }
		}

		public string DbFlags
		{
			get { return Flags.GetStringValue(); }
			set { Flags.SetStringValue(value); }
		}
	}
}
