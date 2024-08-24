using System;
using NHibernate;
using NHibernate.Cfg;

using QMud.Core;

namespace QMud.Database
{
	public class Database
	{
		private static ISessionFactory sessionFactory;

		private static ISessionFactory SessionFactory
		{
			get
			{
				if (sessionFactory == null)
				{
					var cfg = new Configuration();
					cfg.Configure();
					cfg.AddAssembly(typeof(Database).Assembly);
					sessionFactory = cfg.BuildSessionFactory();
				}

				return sessionFactory;
			}
		}

		public static ISession Session()
		{
			return SessionFactory.OpenSession();
		}

		public static void LoadWorld()
		{
			try
			{
				// Load world from database
				using (var session = Session())
				{
					DatabaseItem.LoadItems(session);
					DatabaseMonster.LoadMonsters(session);
					DatabaseZone.LoadZones(session);
				}
			}
			catch (Exception ex)
			{
				Log.Fatal("Error while loading world from database: " + ex.Message);
			}
		}

		public static bool Save(object obj) { return PerformSave(obj, 0); }
		public static bool Update(object obj) { return PerformSave(obj, 1); }
		public static bool SaveOrUpdate(object obj) { return PerformSave(obj, 2); }

		private static bool PerformSave(object obj, int mode)
		{
			try
			{
				using (ISession session = Session())
				{
					using (ITransaction trans = session.BeginTransaction())
					{
						if (mode == 0)
						{
							session.Save(obj);
						}
						else if (mode == 1)
						{
							session.Update(obj);
						}
						else
						{
							session.SaveOrUpdate(obj);
						}

						trans.Commit();
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				Log.Error("Error while saving object to database: " + ex.Message);
				return false;
			}
		}
	}
}
