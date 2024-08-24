using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdWhoAmI : BaseCommand
	{	
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{	
			player.OutLn(player.Name);
		}
		
		public override string GetHelpText ()
		{
			return "Displays the name of your character.";
		}
	}
}
