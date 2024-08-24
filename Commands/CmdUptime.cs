using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdUptime : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			TimeSpan uptime = DateTime.Now - Mud.StartupTime;
			
			player.OutLn("{0} has been running for {1} days, {2} hours and {3} minutes.",
				Settings.MudName, uptime.Days, uptime.Hours, uptime.Minutes);
		}
		
		public override string GetHelpText ()
		{
			return "Shows how long the game has been running.";
		}
	}
}
