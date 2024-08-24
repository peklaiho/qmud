using System;

namespace QMud.Core
{
	public class ZoneEntrance
	{
		public Room From { get; private set; }
		public Room To { get; private set; }

		public ZoneEntrance (Room from, Room to)
		{
			From = from;
			To = to;
		}
	}
}
