using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;

using QMud.Core;

namespace QMud.Database
{
	public class DatabaseZone
	{
		public static void LoadZones(ISession session)
		{
			Log.Info("Loading zones from database.");

			LoadZoneTemplates(session);
			LoadZoneMonsters(session);
		}

		private static void LoadZoneTemplates(ISession session)
		{
			List<ZoneTemplate> items = (List<ZoneTemplate>) session.QueryOver<ZoneTemplate>().List<ZoneTemplate>();
			items.ForEach(x => World.ZoneTemplates.Add(x.Id, x));
		}

		private static void LoadZoneMonsters(ISession session)
		{
			List<ZoneMonster> monsters = (List<ZoneMonster>) session.QueryOver<ZoneMonster>().List<ZoneMonster>();

			ZoneTemplate template;
			foreach (ZoneMonster zm in monsters)
			{
				if (World.ZoneTemplates.TryGetValue(zm.ZoneId, out template))
				{
					if (World.MonsterTemplates.ContainsKey(zm.MonsterId))
					{
						template.Monsters.Add(zm.MonsterId, zm.Count);
					}
					else
					{
						Log.Warning("Unable to find monster {0} for zone {1}.", zm.MonsterId, zm.ZoneId);
					}
				}
				else
				{
					Log.Warning("Unable to find zone {0} for monster {1}.", zm.ZoneId, zm.MonsterId);
				}
			}
		}
	}
}
