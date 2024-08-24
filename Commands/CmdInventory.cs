using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdInventory : BaseCommand
	{	
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			int displayed = 0;
			
			player.OutLn("You are carrying:");
			
			foreach (Item item in player.Inventory)
			{
				if (player.CanSee(item))
				{
					if (args.Length < 2 || Utils.StartsWith(item.Template.Keywords, args[1]))
					{
						Act.ToChar("§o", player, item, null);
						displayed++;
					}
				}
			}
			
			if (displayed == 0)
			{
				player.OutLn("Nothing!");
			}
		}
		
		public override string GetHelpText ()
		{
			return "List all items you are currently carrying. Use the optional parameter to display only items starting with the specified string.";
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"[beginning_of_item_name]"
			};
		}
	}
}
