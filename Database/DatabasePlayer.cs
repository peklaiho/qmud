using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;

using QMud.Core;

namespace QMud.Database
{
	public class DatabasePlayer
	{
		/// <summary>
		/// Tries to find a player from the database by name.
		/// </summary>
		/// <param name="name">
		/// Name of the player to search for. <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Returns the found player, or null. <see cref="QPlayer"/>
		/// </returns>
		public static Player FindByName (string name)
		{
			Player returned = null;

			try
			{
				using (ISession session = Database.Session())
				{
					returned = session.QueryOver<Player>().WhereRestrictionOn(x => x.Name)
						.IsLike(name).SingleOrDefault<Player>();

					if (returned != null)
					{
						LoadPlayerItems(session, returned);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error while querying player database: " + ex.Message);
			}

			return returned;
		}

		/// <summary>
		/// Saves a player to database.
		/// </summary>
		/// <param name="player">
		/// Player to be saved. <see cref="QPlayer"/>
		/// </param>
		public static void Save (Player player)
		{
			try
			{
				using (ISession session = Database.Session())
				{
					using (ITransaction trans = session.BeginTransaction())
					{
						session.SaveOrUpdate(player);
						SavePlayerItems(session, player);
						trans.Commit();
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error while saving player {0} to database: {1}", player.Name, ex.Message);
			}
		}

		private static void LoadPlayerItems(ISession session, Player player)
		{
			List<PlayerItem> items = (List<PlayerItem>) session.QueryOver<PlayerItem>()
				.Where(x => x.Id.Value1 == player.Id)
				.OrderBy(x => x.Id.Value2).Asc
				.List<PlayerItem>();

			Dictionary<int, Item> tempList = new Dictionary<int, Item>();

			foreach (PlayerItem pi in items)
			{
				Item item = World.LoadItem(pi.ItemId);

				if (item == null)
				{
					Log.Error("Could not find item {0} which was saved for player {1}.", pi.ItemId, player.Name);
				}
				else
				{
					tempList.Add(pi.Number, item);

					if (pi.Parent > 0)
					{
						Item parent = null;

						if (tempList.TryGetValue(pi.Parent, out parent))
						{
							Handler.ItemToContainer(item, parent);
						}
						else
						{
							Log.Error("Could not find parent item number {0} for item {1} for player {2}.", pi.Parent, pi.ItemId, player.Name);

							// We have to do something with the item we loaded. Put it to inventory.
							Handler.ItemToInventory(item, player);
						}
					}
					else
					{
						if (pi.Location == 0)
						{
							Handler.ItemToInventory(item, player);
						}
						else
						{
							Handler.ItemToEquipment(item, player);
						}
					}
				}
			}
		}

		private static void SaveOneItem(ISession session, int playerId, ref int number, int itemId, int parent, int location)
		{
			number++;

			PlayerItem pi = new PlayerItem();

			pi.PlayerId = playerId;
			pi.Number = number;
			pi.ItemId = itemId;
			pi.Parent = parent;
			pi.Location = location;

			session.Save(pi);
		}

		private static void SaveItemList(ISession session, List<Item> items, Player player, ref int number, int parent, int location)
		{
			foreach (Item item in items)
			{
				// Save this item
				SaveOneItem(session, player.Id, ref number, item.Template.Id, parent, location);

				// Save contents
				if (item.Contents.Count > 0)
				{
					int parent2 = number;

					SaveItemList(session, item.Contents, player, ref number, parent2, location);
				}
			}
		}

		private static void SavePlayerItems(ISession session, Player player)
		{
			// Delete old items first
			session.CreateSQLQuery("DELETE FROM player_items WHERE PlayerId=" + player.Id).ExecuteUpdate();

			int number = 0;
			
			if (player.Inventory.Count > 0)
			{
				SaveItemList(session, player.Inventory, player, ref number, 0, 0);
			}
			if (player.Equipment.Count > 0)
			{
				SaveItemList(session, player.Equipment, player, ref number, 0, 1);
			}
		}
	}
}
