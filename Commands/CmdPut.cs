using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdPut : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length < 3)
			{
				player.OutLn("Put which item inside which container?");
			}
			else
			{
				// Filter the word 'in'
				if (args.Length >= 4 && Utils.StrCmp(args[2], "in"))
				{
					args[2] = args[3];
				}

				// Find container, can be in the room
				Item container = player.FindItem(args[2], Living.SearchRoom | Living.SearchInv | Living.SearchEq);

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

				if (Utils.StrCmp(args[1], "all"))
				{
					List<Item> allItems = player.Inventory.FindAll(x => player.CanSee(x) && player.CanDrop(x) && x != container);

					if (allItems.Count == 0)
					{
						Act.ToChar("You are not carrying anything to put inside §t.", player, container, null);
					}
					else
					{
						allItems.ForEach(x => PerformPut(player, x, container, false));
					}
				}
				else
				{
					// Find item first, only from inventory?
					// Could find from room as well, but maybe this is clearer.
					Item item = player.FindItem(args[1], Living.SearchInv);

					if (item == null)
					{
						player.OutLn("You are not carrying anything by that name.");
					}
					else if (container == item)
					{
						player.OutLn("You cannot put an item inside itself.");
					}
					else if (!player.CanDrop(item))
					{
						Act.ToChar("You cannot let go of §t.", player, item, null);
					}
					else
					{
						PerformPut(player, item, container, true);
					}
				}
			}
		}

		public override string GetHelpText ()
		{
			return "Put an item inside a container.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<item | 'all'> ['in'] <container>"
			};
		}

		private void PerformPut(Player player, Item item, Item container, bool definite)
		{
			Handler.ItemToContainer(item, container);

			if (definite)
			{
				Act.ToChar("You put §t in §T.", player, item, container);
			}
			else
			{
				Act.ToChar("You put §o in §T.", player, item, container);
			}

			Act.ToRoom("§n puts §o in §O.", true, player, item, container);
		}
	}
}
