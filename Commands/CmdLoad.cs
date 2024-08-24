using System;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdLoad : BaseCommand
	{	
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{
			int targetId;
			
			if (args.Length < 3)
			{
				player.OutLn("Which item or monster do you wish to create?");
				return;
			}

			if (!Int32.TryParse(args[2], out targetId))
			{
				player.OutLn("Invalid id.");
				return;
			}
			
			if (Utils.StartsWith("item", args[1]))
			{
				Item item = World.LoadItem(targetId);
				
				if (item == null)
				{
					player.OutLn("No item by that id.");	
				}
				else
				{
					// Log this stuff!
					Log.GodCmd(player.Name + " loaded item " + item.Template.Name + " (" + item.Template.Id + ").");
					
					Handler.ItemToInventory(item, player);
					Act.ToChar("You create §o.", player, item, null);
					Act.ToRoom("§n creates §o.", true, player, item, null);
				}
			}
			else if (Utils.StartsWith("monster", args[1]))
			{
				Monster monster = World.LoadMonster(targetId, player.InRoom);
				
				if (monster == null)
				{
					player.OutLn("No monster by that id.");
				}
				else
				{
					// Log this stuff!
					Log.GodCmd(player.Name + " loaded monster " + monster.GetName() + " (" + monster.Template.Id + ").");
					
					Act.ToChar("You create §N.", player, null, monster);
					Act.ToRoom("§n creates §N.", true, player, null, monster);
				}
			}
			else
			{
				player.OutLn("The first argument must be 'item' or 'monster'.");
			}
		}
		
		public override string GetHelpText()
		{
			return "Create an instance of the specified item or monster.";
		}
		
		public override string[] GetHelpUsage()
		{
			return new string[]
			{
				"<'item' | 'monster'> <id>"
			};
		}
	}
}
