using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;

using QMud.Core;

namespace QMud.Database
{
	public class DatabaseItem
	{
		public static void LoadItems(ISession session)
		{
			Log.Info("Loading items from database.");

			LoadItemTemplates(session);
		}

		private static void LoadItemTemplates(ISession session)
		{
			List<ItemTemplate> items = (List<ItemTemplate>) session.QueryOver<ItemTemplate>().List<ItemTemplate>();
			items.ForEach(x => World.ItemTemplates.Add(x.Id, x));
		}
	}
}
