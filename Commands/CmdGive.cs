using System;
using System.Collections.Generic;
using System.Linq;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdGive : BaseCommand
	{	
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length < 3)
			{
				player.OutLn("Which item do you wish to give and to whom?");
				return;
			}
			
			// Filter the word 'to'
			if (args.Length >= 4 && Utils.StrCmp(args[2], "to"))
			{
				args[2] = args[3];
			}
			
			// Find target
			Living target = player.InRoom.FindLiving(args[2], player);
			
			if (target == null)
			{
				player.OutLn("No-one here by that name.");
				return;
			}
			else if (target == player)
			{
				player.OutLn("You cannot give items to yourself.");
				return;
			}
			
			if (Utils.StrCmp(args[1], "all"))
			{
				// Safety check: Do not allow 'give all' if target is NPC
				if (target.IsMonster())
				{
					player.OutLn("You cannot give all your items at once to a NPC.");
					return;
				}

				List<Item> allItems = player.Inventory.FindAll(x => player.CanSee(x) && player.CanDrop(x) && target.CanGet(x));
				
				if (allItems.Count == 0)
				{
					player.OutLn("You are not carrying any items to give.");
				}
				else
				{
					allItems.ForEach(x => PerformGive(player, x, target, false));
				}
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
				else if (!target.CanGet(item))
				{
					Act.ToChar("You cannot give §t to §M.", player, item, target);
				}
				else
				{
					PerformGive(player, item, target, true);
				}
			}
		}
		
		public override string GetHelpText ()
		{
			return "Give an item to someone else. You can also give all of your items at once if the target is a player.";
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<item | 'all'> ['to'] <player | npc>"
			};
		}
		
		private void PerformGive(Player player, Item item, Living target, bool definite)
		{
			Handler.ItemToInventory(item, target);
			
			if (definite)
			{
				Act.ToChar("You give §t to §N.", player, item, target);
			}
			else
			{
				Act.ToChar("You give §o to §N.", player, item, target);
			}
			
			Act.ToVict("§n gives you §o.", false, player, item, target);
			Act.ToRoomNotVict("§n gives §o to §N.", true, player, item, target);
		}
	}
}
