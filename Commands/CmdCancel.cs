using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdCancel : BaseCommand
	{
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{
			if (player.InFight == null)
			{
				player.OutLn("You are not in a fight.");
				return;
			}

			player.OutLn("You cancel your selected action for next round.");
			player.NextAction = FightActions.None;
		}

		public override string GetHelpText()
		{
			return "This command allows you to cancel your selected action during a fight.";
		}
	}
}
