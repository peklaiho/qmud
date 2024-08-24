using System;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdGoto : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			Room targetRoom = null;
			int x, y;

			if (args.Length < 2)
			{
				player.OutLn("Where do you want to go?");
				return;
			}

			if (args.Length >= 3 && Int32.TryParse(args[1], out x) && Int32.TryParse(args[2], out y))
			{
				targetRoom = player.InRoom.Zone.GetRoom(x, y);

				if (targetRoom == null)
				{
					player.OutLn("No room exists in those coordinates.");
					return;
				}
			}
			else
			{
				// Not an integer, try to find a player or a monster
				Living targetLiving = World.FindPlayerByName(args[1], player);

				if (targetLiving == null)
				{
					targetLiving = World.FindMonsterByName(args[1], player);
				}

				if (targetLiving == null)
				{
					player.OutLn("No player or monster found by that name.");
					return;
				}
				else
				{
					targetRoom = targetLiving.InRoom;
				}
			}

			// All ok
			Act.ToRoom("§n vanishes.", true, player, null, null);
			Handler.LivingToRoom(player, targetRoom);
			Act.ToRoom("§n appears.", true, player, null, null);
			CmdLook.LookAtRoom(player);
		}

		public override string GetHelpText ()
		{
			return "Go to the room by the specified coordinates. You can also go directly to a player or a NPC by using their name.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<x> <y>",
				"<player | monster>"
			};
		}
	}
}
