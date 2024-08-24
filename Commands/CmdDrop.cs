using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdDrop : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Drop what?");
			}
			else
			{
				Item item = player.FindItem(args[1], Living.SearchInv);
				
				if (item == null)
				{
					player.OutLn("You are not carrying anything by that name.");
				}
				else if (!player.CanDrop(item))
				{
					Act.ToChar("You cannot let go of §t.", player, item, null);
				}
				else
				{
					PerformDrop(player, item);
				}
			}
		}
		
		public override string GetHelpText ()
		{
			return "Drop an item from your inventory on the ground. Consider discarding an item " +
				"instead if it is useless to you and to everyone else. You cannot drop all of " +
				"your items at once with this command.";
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<item>"
			};
		}
		
		private void PerformDrop(Player player, Item item)
		{
			Handler.ItemToRoom(item, player.InRoom);
			Act.ToChar("You drop §t.", player, item, null);
			Act.ToRoom("§n drops §o.", false, player, item, null);
		}
	}
}
