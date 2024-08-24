using System;

namespace QMud.Core
{
	public class ItemTemplate
	{
		public enum Articles
		{
			None,
			A,
			An,
			The
		}

		// Database attributes
		public int Id { get; set; }
		public Articles Article { get; set; }
		public string Name { get; set; }
		public string[] Keywords { get; private set; }
		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }

		// Runtime attributes
		public int Count { get; set; }

		public ItemTemplate ()
		{

		}

		public string DbKeywords
		{
			get { return String.Join(",", Keywords); }
			set { Keywords = value.Split(','); }
		}
	}
}
