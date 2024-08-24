using System;

using QMud.Core;
using QMud.Database;

namespace QMud.Commands
{
	public class CmdQuit : BaseCommand
	{
		public enum SubCommands { Quit, Qui };
		
		private SubCommands SubCommand;
		
		public CmdQuit (SubCommands newSubCmd)
		{
			SubCommand = newSubCmd;
		}
		
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{	
			if (SubCommand == CmdQuit.SubCommands.Qui)
			{
				player.OutLn("Please type the whole command if you wish to quit.");
				return;
			}
			else if (player.InFight != null)
			{
				player.OutLn("You cannot quit while in a fight.");
				return;
			}
			
			player.OutLn("Goodbye...");
			player.OutLn();
			
			// Output not sent to descriptors which will be closed, so we have to do it manually
			player.Descriptor.WriteOutput();
			
			// Log the event
			Log.Info("{0} has quit the realm (connection id {1}).", player.Name, player.Descriptor.Id);
			
			// Send a message to others in room
			Act.ToRoom("§n has left the realm.", true, player, null, null);
			
			// Save to database
			DatabasePlayer.Save(player);
			
			// Set the connection to be closed
			player.Descriptor.Disconnect = true;
			
			// Detach player from descriptor and extract him
			player.Descriptor.Player = null;
			player.Descriptor = null;
			
			Handler.ExtractLiving(player);
		}
		
		public override string GetHelpText ()
		{
			if (SubCommand == CmdQuit.SubCommands.Quit)
			{
				return "Disconnects you from the game.";
			}
			else
			{
				return "Safety command which ensures that you must type 'quit' completely.";
			}
		}
	}
}
