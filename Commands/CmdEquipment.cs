using System;
using System.Collections.Generic;
using System.Linq;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdEquipment : BaseCommand
	{
		public static int ShowEquipment (Living target, Player viewer)
		{
			int displayed = 0;

			foreach (Item item in target.Equipment)
			{
				if (viewer.CanSee(item))
				{
					Act.ToChar("§o", viewer, item, null);
					displayed++;
				}
			}

			return displayed;
		}
		
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (ShowEquipment(player, player) == 0)
			{
				player.OutLn("You are not wearing anything.");
			}
		}
		
		public override string GetHelpText ()
		{
			return "List all items you are currently wearing.";
		}
	}
}
