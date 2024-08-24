using System;
using System.Collections.Generic;

namespace QMud.Core
{
	/// <summary>
	/// Handles interactions between objects of different type, including rooms, items, players and so on.
	/// </summary>
	public class Handler
	{
		public static void ExtractItem(Item item)
		{
			// Start by extracting all contents first, if this is a container
			while (item.Contents.Count > 0)
			{
				ExtractItem(item.Contents[0]);
			}
			
			ClearAssociations(item);
			
			World.Items.Remove(item);
			item.Template.Count--;
		}
		
		/// <summary>
		/// Extracts a monster or a player from the game.
		/// </summary>
		/// <param name="living">
		/// Monster or player to be extracted. <see cref="QLiving"/>
		/// </param>
		public static void ExtractLiving (Living living)
		{
			// Extra inventory and equipment
			while (living.Inventory.Count > 0)
			{
				ExtractItem(living.Inventory[0]);
			}
			while (living.Equipment.Count > 0)
			{
				ExtractItem(living.Equipment[0]);
			}

			// Remove from party
			LivingFromParty(living);

			// Remove from room
			LivingFromRoom(living);
			
			// Remove from global list
			if (living.IsPlayer())
			{
				World.Players.Remove((Player) living);
			}
			else
			{
				Monster monster = (Monster) living;
				
				World.Monsters.Remove(monster);
				monster.Template.Count--;
			}
		}
		
		public static void ItemToContainer(Item item, Item container)
		{
			ClearAssociations(item);
			
			container.Contents.Add(item);
			item.InItem = container;
		}
		
		public static void ItemToEquipment(Item item, Living living)
		{
			ClearAssociations(item);
			
			living.Equipment.Add(item);
			item.WornBy = living;
		}
		
		public static void ItemToInventory(Item item, Living living)
		{
			ClearAssociations(item);
			
			living.Inventory.Add(item);
			item.CarriedBy = living;
		}
		
		public static void ItemToRoom(Item item, Room room)
		{
			ClearAssociations(item);
			
			room.Items.Add(item);
			item.InRoom = room;
		}

		public static void LivingFromFight (Living living, bool checkForEnd)
		{
			if (living.InFight != null)
			{
				living.InFight.Attackers.Remove(living);
				living.InFight.Defenders.Remove(living);
				living.InFight.UnactedFighters.Remove(living);

				if (checkForEnd)
				{
					living.InFight.CheckForEnd();
				}

				living.InFight = null;
				living.NextAction = FightActions.None;
			}
		}

		public static void LivingFromParty (Living living)
		{
			if (living.InParty != null)
			{
				if (living.IsPlayer())
				{
					living.InParty.Players.Remove((Player) living);
				}
				else
				{
					living.InParty.Monsters.Remove((Monster) living);
				}

				living.InParty = null;
			}
		}

		/// <summary>
		/// Removes a QLiving object (monster or player) from a room.
		/// </summary>
		/// <param name="living">
		/// Monster or player who will be removed from their current room. <see cref="QLiving"/>
		/// </param>
		public static void LivingFromRoom (Living living)
		{
			// Must leave fight before leaving room
			LivingFromFight(living, true);

			if (living.InRoom != null)
			{
				if (living.IsPlayer())
				{
					living.InRoom.Players.Remove((Player) living);
				}
				else
				{
					living.InRoom.Monsters.Remove((Monster) living);
				}
				
				living.InRoom = null;
			}
		}

		public static void LivingToFight (Living living, Fight fight, bool attacker)
		{
			if (living.InFight != null)
			{
				Log.Error("{0} is already in fight when entering LivingToFight.", living.GetName());
				return;
			}

			living.InFight = fight;
			living.NextAction = FightActions.None;

			if (attacker)
			{
				fight.Attackers.Add(living);
			}
			else
			{
				fight.Defenders.Add(living);
			}
		}

		public static void LivingToParty (Living living, Party party)
		{
			if (living.InParty != null)
			{
				Log.Error("{0} is already in party when entering LivingToParty.", living.GetName());
				return;
			}

			living.InParty = party;

			if (living.IsPlayer())
			{
				party.Players.Add((Player) living);
			}
			else
			{
				party.Monsters.Add((Monster) living);
			}
		}

		/// <summary>
		/// Insert a QLiving object (monster or player) into a room.
		/// </summary>
		/// <param name="living">
		/// Monster or player to enter the room. <see cref="QLiving"/>
		/// </param>
		/// <param name="room">
		/// Room which the monster or player enters. <see cref="QRoom"/>
		/// </param>
		public static void LivingToRoom (Living living, Room room)
		{	
			// Leave any old room
			LivingFromRoom(living);
			
			// Enter new room
			living.InRoom = room;
			
			if (living.IsPlayer())
			{
				room.Players.Add((Player) living);	
			}
			else
			{
				room.Monsters.Add((Monster) living);
			}
		}

		// Private functions
		
		private static void ClearAssociations(Item item)
		{
			if (item.CarriedBy != null)
			{
				item.CarriedBy.Inventory.Remove(item);
				item.CarriedBy = null;
			}
			if (item.WornBy != null)
			{
				item.WornBy.Equipment.Remove(item);
				item.WornBy = null;
			}
			if (item.InRoom != null)
			{
				item.InRoom.Items.Remove(item);
				item.InRoom = null;
			}
			if (item.InItem != null)
			{
				item.InItem.Contents.Remove(item);
				item.InItem = null;
			}
		}
	}
}
