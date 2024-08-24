using System;
using System.Collections.Generic;
using System.Linq;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdGet : BaseCommand
	{	
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{	
			if (args.Length < 2)
			{
				player.OutLn("Get what?");
			}
			else if (args.Length < 3)
			{
				// Get all items from room
				if (Utils.StrCmp(args[1], "all"))
				{
					List<Item> allItems = player.InRoom.Items.FindAll(x => player.CanSee(x) && player.CanGet(x) && !x.IsNoPickup());
					
					if (allItems.Count == 0)
					{
						player.OutLn("Nothing here to pick up.");
					}
					else
					{
						allItems.ForEach(x => PerformGet(player, x, false));
					}
				}
				// Get one item from room
				else
				{
					Item item = player.FindItem(args[1], Living.SearchRoom);
					
					if (item == null)
					{
						player.OutLn("Nothing here by that name.");
					}
					else if (item.IsNoPickup() || !player.CanGet(item))
					{
						Act.ToChar("You cannot pick up §t.", player, item, null);
					}
					else
					{
						PerformGet(player, item, true);
					}
				}
			}
			else
			{
				// Filter the word 'from'
				if (args.Length >= 4 && Utils.StrCmp(args[2], "from"))
				{
					args[2] = args[3];
				}
				
				// Find a container
				Item container = player.FindItem(args[2], Living.SearchRoom | Living.SearchInv | Living.SearchEq);
				
				// Container is still not found, return
				if (container == null)
				{
					player.OutLn("No container here by that name.");
					return;
				}
				else if (!container.IsContainer())
				{
					Act.ToChar("§t is not a container.", player, container, null);
					return;
				}
				
				// Get all items from container
				if (Utils.StrCmp(args[1], "all"))
				{
					List<Item> allItems = container.Contents.FindAll(x => player.CanSee(x) && !x.IsNoPickup() && player.CanGet(x));
					
					if (allItems.Count == 0)
					{
						Act.ToChar("§t contains nothing to get.", player, container, null);
					}
					else
					{
						allItems.ForEach(x => PerformGetFromContainer(player, x, container, false));
					}
				}
				// Get one item from container
				else
				{
					Item item = container.FindItem(args[1], player);
					
					if (item == null)
					{
						Act.ToChar("§t contains nothing by that name.", player, container, null);
					}
					else if (item.IsNoPickup() || !player.CanGet(item))
					{
						Act.ToChar("You cannot get §t from §T.", player, item, container);
					}
					else
					{
						PerformGetFromContainer(player, item, container, true);
					}
				}
			}
		}
		
		public override string GetHelpText()
		{
			return "Pick up an item from the ground. You can also get an item from a container.";
		}
		
		public override string[] GetHelpUsage()
		{
			return new string[]
			{
				"<item | 'all'>",
				"<item | 'all'> ['from'] <container>"
			};
		}
		
		private void PerformGet(Player player, Item item, bool definite)
		{
			Handler.ItemToInventory(item, player);
			
			if (definite)
			{
				Act.ToChar("You get §t.", player, item, null);
			}
			else
			{
				Act.ToChar("You get §o.", player, item, null);
			}
			
			Act.ToRoom("§n gets §o.", true, player, item, null);
		}
		
		private void PerformGetFromContainer(Player player, Item item, Item container, bool definite)
		{
			Handler.ItemToInventory(item, player);
			
			if (definite)
			{
				Act.ToChar("You get §t from §T.", player, item, container);
			}
			else
			{
				Act.ToChar("You get §o from §T.", player, item, container);
			}
			
			Act.ToRoom("§n gets §o from §O.", true, player, item, container);
		}
	}
}
