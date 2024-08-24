using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdExplore : BaseCommand
	{
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Available areas:");

				foreach (KeyValuePair<int, ZoneTemplate> temp in World.ZoneTemplates)
				{
					player.OutLn("{0}: {1}", temp.Key, temp.Value.Name);
				}

				return;
			}

			if (player.InFight != null)
			{
				player.OutLn("You are fighting for your life!");
				return;
			}
			else if (player.InRoom.Zone != World.StartingZone)
			{
				player.OutLn("You can only explore new areas while in town.");
				return;
			}
			else if (World.Zones.Count >= Settings.MaxZones)
			{
				player.OutLn("You cannot discover new areas at this time.");
				return;
			}

			int id;
			if (!Int32.TryParse(args[1], out id))
			{
				player.OutLn("Invalid area number.");
				return;
			}

			Zone zone = World.LoadZone<MazeZone>(id);

			if (zone == null)
			{
				player.OutLn("No area by such number exists.");
				return;
			}

			// Entrance from current zone to new zone
			ZoneEntrance entrance = new ZoneEntrance(player.InRoom, zone.StartingRoom);
			player.InRoom.Entrances.Add(entrance);

			// Entrance from new zone to current zone
			entrance = new ZoneEntrance(zone.StartingRoom, player.InRoom);
			zone.StartingRoom.Entrances.Add(entrance);

			player.OutLn("You discover new area '{0}'.", zone.Template.Name);
			Act.ToRoom("§n discovers new area '" + zone.Template.Name + "'.", false, player, null, null);

			Log.Info("{0} created new zone '{1}'.", player.GetName(), zone.Template.Name);
		}

		public override string GetHelpText()
		{
			return "This command allows you explore the world and discover new areas. " +
				"Use the command without parameters to list available areas. Explore an area " +
				"by giving the number of the area as the parameter.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"",
				"<area>"
			};
		}
	}
}
