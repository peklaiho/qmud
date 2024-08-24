using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;

using QMud.Core;

namespace QMud.Database
{
	public class DatabaseWorld
	{
		public static void LoadWorld(ISession session)
		{
			Log.Info("Loading zones from database.");

			LoadZones(session);

			foreach (KeyValuePair<int, Zone> keyVal in World.Zones)
			{
				keyVal.Value.CreateRooms();
			}
		}

		private static void LoadZones(ISession session)
		{
			List<Zone> zones = (List<Zone>) session.QueryOver<Zone>().List<Zone>();
			zones.ForEach(x => World.Zones.Add(x.Id, x));
		}
	}
}
