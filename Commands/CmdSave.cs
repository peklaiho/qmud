using System;

using QMud.Database;
using QMud.Core;

namespace QMud.Commands
{
	public class CmdSave : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{	
			if (args.Length < 2)
			{
				player.OutLn("Save who?");
				return;
			}
			
			// Save all
			if (Utils.StrCmp(args[1], "all"))
			{
				World.Players.ForEach(x => DatabasePlayer.Save(x));
				player.OutLn("All players have been saved.");
				
				// Log it
				Log.GodCmd(player.Name + " saved all players.");
			}
			// Save a specific player
			else
			{
				Player target = World.FindPlayerByName(args[1], player);
				
				if (target != null)
				{
					DatabasePlayer.Save(target);
					player.OutLn(target.Name + " has been saved.");
					
					// Log it
					Log.GodCmd(player.Name + " saved " + target.Name + ".");
				}
				else
				{
					player.OutLn("No player found by that name.");
				}
			}
		}
		
		public override string GetHelpText ()
		{
			return "Save a player, or all players, to database.";
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<player | 'all'>"
			};
		}
	}
}
