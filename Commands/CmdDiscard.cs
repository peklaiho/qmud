using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdDiscard : BaseCommand
	{
		public enum SubCommands { Discar, Discard }

		private SubCommands SubCommand;

		public CmdDiscard (SubCommands newSubCmd)
		{
			SubCommand = newSubCmd;
		}

		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (SubCommand == SubCommands.Discar)
			{
				player.OutLn("Please type the whole command if you wish to discard something.");
				return;
			}

			if (args.Length < 2)
			{
				player.OutLn("Which item do you wish to discard?");
			}
			else
			{
				Item item = player.FindItem(args[1], Living.SearchInv);

				if (item == null)
				{
					player.OutLn("You are not carrying anything by that name.");
					return;
				}
				else if (!player.CanDrop(item))
				{
					Act.ToChar("You cannot let go of §t.", player, item, null);
					return;
				}

				// Safety check: Do not allow discard if item contains other items which player can see
				// If invisible items get discarded, we don't stop that, player's own fault
				foreach (Item subItem in item.Contents)
				{
					if (player.CanSee(subItem))
					{
						Act.ToChar("Empty §t first before discarding it.", player, item, null);
						return;
					}
				}

				PerformDiscard(player, item);
			}
		}

		public override string GetHelpText ()
		{
			if (SubCommand == SubCommands.Discard)
			{
				return "Permanently destroys an item. Be careful with this command and use it only on completely useless items.";
			}
			else
			{
				return "Safety command which ensures that you must type 'discard' completely.";
			}
		}

		public override string[] GetHelpUsage ()
		{
			if (SubCommand == SubCommands.Discard)
			{
				return new string[]
				{
					"<item>"
				};
			}
			else
			{
				return base.GetHelpUsage();
			}
		}

		private void PerformDiscard(Player player, Item item)
		{
			Handler.ExtractItem(item);
			Act.ToChar("You discard §t.", player, item, null);
			Act.ToRoom("§n discards §o.", true, player, item, null);
		}
	}
}
