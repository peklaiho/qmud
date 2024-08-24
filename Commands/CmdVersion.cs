using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdVersion : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			player.OutLn(Settings.MudName + " " + Settings.MudVersion);
		}
		
		public override string GetHelpText ()
		{
			return "Displays version information about the game.";
		}
	}
}
