using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;

using QMud.Core;

namespace QMud.Database
{
	public class DatabaseMonster
	{
		public static void LoadMonsters(ISession session)
		{
			Log.Info("Loading monsters from database.");

			// Read monsters
			LoadMonsterTemplates(session);
		}

		private static void LoadMonsterTemplates(ISession session)
		{
			List<MonsterTemplate> monsters = (List<MonsterTemplate>) session.QueryOver<MonsterTemplate>().List<MonsterTemplate>();
			monsters.ForEach(x => World.MonsterTemplates.Add(x.Id, x));
		}
	}
}
