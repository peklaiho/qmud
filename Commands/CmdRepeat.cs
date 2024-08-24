using System;

namespace QMud.Commands
{
	public class CmdRepeat : BaseCommand
	{
		public override void ExecuteCommand (QMud.Core.Player player, string[] args, string wholeArg)
		{
			if (player.LastCommand == null)
			{
				player.OutLn("You have not typed any commands yet.");
			}
			else
			{
				CommandInterpreter.InterpretCommand(player, player.LastCommand);
			}
		}

		public override string GetHelpText ()
		{
			return "Repeat the last typed command.";
		}
	}
}
