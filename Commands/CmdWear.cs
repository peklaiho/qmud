using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdWear : BaseCommand
	{
		public enum SubCommands
		{
			Wear,
			Wield
		}
		
		private SubCommands SubCommand;
		
		public CmdWear (SubCommands newSubCmd)
		{
			SubCommand = newSubCmd;
		}
		
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				if (SubCommand == SubCommands.Wear)
				{
					player.OutLn("Which item do you wish to wear?");
				}
				else
				{
					player.OutLn("Which weapon do you wish to wield?");
				}
			}
			else if (Utils.StrCmp(args[1], "all") && SubCommand == SubCommands.Wear)
			{
				int wornItems = 0;

				for (int i = 0; i < player.Inventory.Count; )
				{
					Item item = player.Inventory[i];

					if (player.CanSee(item) && player.CanWear(item))
					{
						PerformWear(player, item, false);
						wornItems++;
					}
					else
					{
						i++;
					}
				}

				if (wornItems == 0)
				{
					player.OutLn("You are not carrying anything to wear.");
				}
			}
			else
			{
				Item item = player.FindItem(args[1], Living.SearchInv);
				
				if (item == null)
				{
					player.OutLn("You are not carrying anything by that name.");
				}
				else if (!player.CanWear(item))
				{
					Act.ToChar("You cannot use §t.", player, item, null);
				}
				else
				{
					PerformWear(player, item, true);
				}
			}
		}
		
		public override string GetHelpText ()
		{
			if (SubCommand == SubCommands.Wear)
			{
				return "Wear a piece of equipment from your inventory. You can also try to wear all " +
					"suitable items from your inventory at once.";
			}
			else
			{
				return "Wield a weapon from your inventory.";
			}
		}
		
		public override string[] GetHelpUsage ()
		{
			if (SubCommand == SubCommands.Wear)
			{
				return new string[]
				{
					"<item | 'all'>"
				};
			}
			else
			{
				return new string[]
				{
					"<weapon>"
				};
			}
		}
		
		private void PerformWear(Player player, Item item, bool definite)
		{
			Handler.ItemToEquipment(item, player);
			
			if (SubCommand == SubCommands.Wear)
			{
				if (definite)
				{
					Act.ToChar("You start using §t.", player, item, null);
				}
				else
				{
					Act.ToChar("You start using §o.", player, item, null);
				}

				Act.ToRoom("§n starts using §o.", true, player, item, null);
			}
			else
			{
				Act.ToChar("You wield §t.", player, item, null);
				Act.ToRoom("§n wields §o.", true, player, item, null);
			}
		}
	}
}
