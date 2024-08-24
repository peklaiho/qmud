using System;
using System.Collections.Generic;
using System.Linq;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdRemove : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Which piece of worn equipment do you wish to remove?");
			}
			else if (Utils.StrCmp(args[1], "all"))
			{
				List<Item> allItems = player.Equipment.FindAll(x => player.CanSee(x) && player.CanRemove(x));
				
				if (allItems.Count == 0)
				{
					player.OutLn("You are not wearing any equipment to remove.");
				}
				else
				{
					allItems.ForEach(x => PerformRemove(player, x, false));
				}
			}
			else
			{
				Item item = player.FindItem(args[1], Living.SearchEq);
				
				if (item == null)
				{
					player.OutLn("You are not wearing any equipment by that name.");
				}
				else if (!player.CanRemove(item))
				{
					Act.ToChar("You cannot stop using §t.", player, item, null);
				}
				else
				{
					PerformRemove(player, item, true);
				}
			}
		}
		
		public override string GetHelpText ()
		{
			return "Stop using a piece of equipment. You can also remove all of your equipment.";
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<item | 'all'>"
			};
		}
		
		private void PerformRemove(Player player, Item item, bool definite)
		{
			Handler.ItemToInventory(item, player);
			
			if (definite)
			{
				Act.ToChar("You stop using §t.", player, item, null);
			}
			else
			{
				Act.ToChar("You stop using §o.", player, item, null);
			}
			
			Act.ToRoom("§n stops using §o.", true, player, item, null);
		}
	}
}
