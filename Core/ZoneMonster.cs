using System;

namespace QMud.Core
{
	public class ZoneMonster
	{
		public CompositeKey Id { get; set; }
		public int Count { get; private set; }

		public ZoneMonster ()
		{
			Id = new CompositeKey();
		}

		public int ZoneId
		{
			get { return Id.Value1; }
			set { Id.Value1 = value; }
		}

		public int MonsterId
		{
			get { return Id.Value2; }
			set { Id.Value2 = value; }
		}
	}
}
