using System;
using System.Collections.Generic;
using System.Linq;

namespace QMud.Core
{
	/// <summary>
	/// Contains global lists and finders.
	/// </summary>
	public class World
	{
		public static Dictionary<int, ZoneTemplate> ZoneTemplates = new Dictionary<int, ZoneTemplate>();
		public static Dictionary<int, MonsterTemplate> MonsterTemplates = new Dictionary<int, MonsterTemplate>();
		public static Dictionary<int, ItemTemplate> ItemTemplates = new Dictionary<int, ItemTemplate>();

		public static List<Zone> Zones = new List<Zone>();
		public static List<Monster> Monsters = new List<Monster>();
		public static List<Player> Players = new List<Player>();
		public static List<Item> Items = new List<Item>();

		public static Zone StartingZone;

		public static void InitStartingZone()
		{
			ZoneTemplate template = new ZoneTemplate(0, "Township of Gauntlet", 3, 3);
			StartingZone = LoadZone<SimpleZone>(template);
		}

		public static Monster FindMonsterByName(string name, Living searcher)
		{
			return Monsters.Find(x => (searcher == null || searcher.CanSee(x)) &&
			                     Utils.StartsWith(x.Template.Keywords, name));
		}

		/// <summary>
		/// Tries to find a player who is loaded in game by given name.
		/// </summary>
		/// <param name="name">
		/// Name of the player. <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// The found player, or null. <see cref="QPlayer"/>
		/// </returns>
		public static Player FindPlayerByName(string name, Living searcher)
		{
			return Players.Find(x => (searcher == null || searcher.CanSee(x)) &&
			                     Utils.StartsWith(x.Name, name));
		}

		public static Item LoadItem(int id)
		{
			ItemTemplate template;
			ItemTemplates.TryGetValue(id, out template);
			return template != null ? LoadItem(template) : null;
		}

		public static Item LoadItem(ItemTemplate template)
		{
			Item item = new Item(template);
			Items.Add(item);
			template.Count++;

			return item;
		}

		public static Monster LoadMonster(int id, Room room)
		{
			MonsterTemplate template;
			MonsterTemplates.TryGetValue(id, out template);
			return template != null ? LoadMonster(template, room) : null;
		}

		public static Monster LoadMonster(MonsterTemplate template, Room room)
		{
			Monster monster = new Monster(template);
			Monsters.Add(monster);
			template.Count++;
			Handler.LivingToRoom(monster, room);

			return monster;
		}

		public static Zone LoadZone<T>(int id)
			where T : Zone, new()
		{
			ZoneTemplate template;
			ZoneTemplates.TryGetValue(id, out template);
			return template != null ? LoadZone<T>(template) : null;
		}

		public static Zone LoadZone<T>(ZoneTemplate template)
			where T : Zone, new()
		{
			Zone zone = new T();
			zone.FromTemplate(template);
			Zones.Add(zone);

			// Load monsters
			foreach (KeyValuePair<int, int> zm in zone.Template.Monsters)
			{
				for (int i = 0; i < zm.Value; i++)
				{
					LoadMonster(zm.Key, zone.GetRandomRoom());
				}
			}

			return zone;
		}
	}
}
