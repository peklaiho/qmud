using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdHit : BaseCommand
	{
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{
			if (player.InFight == null)
			{
				player.OutLn("You are not in a fight.");
				return;
			}

			player.OutLn("You attempt to hit your opponent during next round.");
			player.NextAction = FightActions.Hit;
		}

		public override string GetHelpText()
		{
			return "This command allows you to hit your opponent during a fight.";
		}
	}
}
