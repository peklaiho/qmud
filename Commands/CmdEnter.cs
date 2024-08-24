using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdEnter : BaseCommand
	{
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{
			if (player.InFight != null)
			{
				player.OutLn("You are fighting for your life! Try to flee instead.");
				return;
			}
			else if (args.Length < 2)
			{
				player.OutLn("Enter which area?");
				return;
			}

			foreach (ZoneEntrance entry in player.InRoom.Entrances)
			{
				if (Utils.StartsWith(entry.To.Zone.Template.Name, args[1]))
				{
					Act.ToRoom("§n enters " + entry.To.Zone.Template.Name + ".", true, player, null, null);

					Handler.LivingToRoom(player, entry.To);

					Act.ToRoom("§n arrives from " + entry.From.Zone.Template.Name + ".", true, player, null, null);
					player.OutLn("You enter {0}.", entry.To.Zone.Template.Name);
					CmdLook.LookAtRoom(player);
					return;
				}
			}

			player.OutLn("There is no entrance to such area here.");
		}

		public override string GetHelpText()
		{
			return "This command allows you to enter another area.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<area>"
			};
		}
	}
}
