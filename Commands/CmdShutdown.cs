using System;

using QMud.Core;
using QMud.Database;

namespace QMud.Commands
{
	public class CmdShutdown : BaseCommand
	{
		public enum SubCommands { Shutdown, Shutdow };
		
		private SubCommands SubCommand;
		
		public CmdShutdown (SubCommands newSubCmd)
		{
			SubCommand = newSubCmd;
		}
		
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (SubCommand == CmdShutdown.SubCommands.Shutdow)
			{
				player.OutLn("Please type the whole command if you wish to shutdown the game.");
				return;
			}
			
			// Log it
			Log.GodCmd("Shutdown by " + player.Name + ".");

			// Send message to players
			Network.AddAllOutputLine("Shutting down...", true);
			
			// Main loop will exit after current iteration
			Mud.RunMud = false;
		}
		
		public override string GetHelpText ()
		{
			if (SubCommand == CmdShutdown.SubCommands.Shutdown)
			{
				return "Shuts the game down.";
			}
			else
			{
				return "Safety command which ensures that you must type 'shutdown' completely.";
			}
		}
	}
}
