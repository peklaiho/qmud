using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdFight : BaseCommand
	{
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{
			if (player.InFight != null)
			{
				player.OutLn("You are already in a fight!");
				return;
			}
			else if (player.InRoom.Zone == World.StartingZone)
			{
				player.OutLn("The town feels too peaceful for fighting.");
				return;
			}
			else if (args.Length < 2)
			{
				player.OutLn("Fight who?");
				return;
			}

			Living target = player.InRoom.FindLiving(args[1], player);

			if (target == null)
			{
				player.OutLn("No-one here by that name.");
				return;
			}
			else if (target == player)
			{
				player.OutLn("You attempt to start a fight with yourself.");
				return;
			}
			else if (player.IsInSameParty(target))
			{
				player.OutLn("You cannot fight someone who is in your own party.");
				return;
			}

			Fight.StartFight(player, target);
		}

		public override string GetHelpText()
		{
			return "This command allows you to start a fight or to join an ongoing fight.";
		}

		public override string[] GetHelpUsage()
		{
			return new string[]
			{
				"<target>"
			};
		}
	}
}
