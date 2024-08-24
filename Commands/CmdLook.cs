using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdLook : BaseCommand
	{
		public static void LookAtRoom(Player plr)
		{
			if (plr.IsBlind())
			{
				plr.OutLn("You are blind and cannot see anything.");
				return;
			}

			// Zone name
			plr.Out(plr.GetColorPref(ColorLoc.RoomName));
			if (plr.Level >= Player.LvlImmort)
			{
				plr.Out("{0} {1}", plr.InRoom.GetLocationString(), plr.InRoom.Zone.Template.Name);
			}
			else
			{
				plr.Out("{0}", plr.InRoom.Zone.Template.Name);
			}
			plr.OutLn(plr.GetColor(Color.Reset));

			// Exits
			string exits = "";

			for (int i = 0; i < EnumNames.DirNames.Length; i++)
			{
				if (plr.InRoom.Exits[i])
				{
					if (exits.Length > 0)
					{
						exits += ", ";
					}

					exits += EnumNames.DirNames[i];
				}
			}

			plr.OutLn("{0}Exits: {1}{2}", plr.GetColorPref(ColorLoc.RoomExits), exits, plr.GetColor(Color.Reset));

			// Entrances
			foreach (ZoneEntrance entry in plr.InRoom.Entrances)
			{
				plr.OutLn("{0}Entrance to '{1}' is here.{2}", plr.GetColorPref(ColorLoc.RoomEntrances), entry.To.Zone.Template.Name, plr.GetColor(Color.Reset));
			}

			// Items
			foreach (Item item in plr.InRoom.Items)
			{
				if (plr.CanSee(item))
				{
					plr.Out(plr.GetColorPref(ColorLoc.RoomItems));

					if (plr.Level >= Player.LvlImmort)
					{
						plr.Out("[{0,4}] ", item.Template.Id);
					}

					plr.OutLn(item.Template.ShortDescription + plr.GetColor(Color.Reset));
				}
			}

			// Monsters
			ShowMonstersInRoom(plr, plr.InRoom);

			// Players
			ShowPlayersInRoom(plr, plr.InRoom);
		}

		private static int ShowMonstersInRoom(Player plr, Room room)
		{
			int displayed = 0;

			foreach (Monster monster in room.Monsters)
			{
				if (plr.CanSee(monster))
				{
					string str = plr.GetColorPref(ColorLoc.RoomMobs);

					if (plr.Level >= Player.LvlImmort)
					{
						str += String.Format("[{0,4}] ", monster.Template.Id);
					}

					if (monster.InFight != null)
					{
						str += Utils.CapitalizeString(monster.GetName()) + " is here, fighting " + GetOpponentName(plr, monster) + ".";
					}
					else
					{
						str += monster.Template.ShortDescription;
					}

					plr.OutLn(str + plr.GetColor(Color.Reset));

					displayed++;
				}
			}

			return displayed;
		}

		private static int ShowPlayersInRoom(Player plr, Room room)
		{
			int displayed = 0;

			foreach (Player other in room.Players)
			{
				if (plr != other && plr.CanSee(other))
				{
					string str = plr.GetColorPref(ColorLoc.RoomPlayers) + other.Name + " is here";

					if (other.InFight != null)
					{
						str += ", fighting " + GetOpponentName(plr, other);
					}
					if (other.Descriptor == null)
					{
						str += " (linkless)";
					}

					plr.OutLn(str + "." + plr.GetColor(Color.Reset));

					displayed++;
				}
			}

			return displayed;
		}

		private static string GetOpponentName(Player player, Living fighter)
		{
			List<Living> opponents = fighter.InFight.OpposingTeam(fighter).FindAll(x => player.CanSee(x));

			if (opponents.Count == 0)
			{
				return "someone";
			}
			else if (opponents.Count == 1)
			{
				if (opponents[0] == player)
				{
					return "you";
				}
				else
				{
					return opponents[0].GetName();
				}
			}
			else
			{
				return "multiple opponents";
			}
		}

		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (player.IsBlind())
			{
				player.OutLn("You are blind and cannot see anything.");
				return;
			}

			if (args.Length < 2)
			{
				LookAtRoom(player);
			}
			else if (Utils.StrCmp("in", args[1]))
			{
				if (args.Length < 3)
				{
					player.OutLn("Look inside what?");
					return;
				}

				// Search the item
				Item item = player.FindItem(args[2], Living.SearchRoom | Living.SearchInv | Living.SearchEq);

				if (item != null)
				{
					LookInItem(player, item);
					return;
				}

				player.OutLn("Nothing to look into by that name.");
			}
			else
			{
				// Filter the word 'at'
				if (args.Length >= 3 && Utils.StrCmp("at", args[1]))
				{
					args[1] = args[2];
				}

				// See if player is looking at a living entity
				Living living = player.InRoom.FindLiving(args[1], player);
				if (living != null)
				{
					// Look at self
					if (player == living)
					{
						player.OutLn("It is hard to see yourself without a mirror.");
						return;
					}

					// Show description for monsters if they have one
					if (living.IsMonster())
					{
						if (((Monster) living).Template.LongDescription.Length > 0)
						{
							string formattedDescription = Utils.FormatText(((Monster) living).Template.LongDescription, 0, 1, player.LineLength);
							player.OutLn(formattedDescription);
						}
					}

					// Show worn equipment
					CmdEquipment.ShowEquipment(living, player);

					return;
				}

				// See if player is looking at an item
				Item item = player.FindItem(args[1], Living.SearchRoom | Living.SearchInv | Living.SearchEq);
				if (item != null)
				{
					if (item.Template.LongDescription.Length > 0)
					{
						string formattedDescription = Utils.FormatText(item.Template.LongDescription, 0, 1, player.LineLength);
						player.OutLn(formattedDescription);
					}
					else
					{
						player.OutLn("There is nothing special about it.");
					}

					return;
				}

				// Nothing found.
				player.OutLn("Nothing to look at by that name.");
			}
		}

		public override string GetHelpText ()
		{
			return "Display information about the current area you are in. You can also look at " +
				"items, players and NPCs. Additionally, you can use this command to list all visible items inside a " +
				"container.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"",
				"['at'] <item | player | monster>",
				"<'in'> <container>"
			};
		}

		private void LookInItem(Player player, Item container)
		{
			int displayed = 0;

			Act.ToChar("§t (" + container.Location() + ") contains:", player, container, null);

			foreach (Item item in container.Contents)
			{
				if (player.CanSee(item))
				{
					Act.ToChar("§o", player, item, null);
					displayed++;
				}
			}

			if (displayed == 0)
			{
				player.OutLn("Nothing!");
			}
		}
	}
}
