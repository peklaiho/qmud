using System;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdSearch : BaseCommand
	{
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{
			int displayed = 0;
			
			if (args.Length < 3)
			{
				player.OutLn("What do you wish to search?");
				return;
			}
			
			if (Utils.StartsWith("item", args[1]))
			{
				foreach (KeyValuePair<int, ItemTemplate> keyval in World.ItemTemplates)
				{
					if (Utils.StartsWith(keyval.Value.Keywords, args[2]))
					{
						player.OutLn("[{0,5}] {1}", keyval.Key, keyval.Value.Name);
						displayed++;
					}
				}
			}
			else if (Utils.StartsWith("monster", args[1]))
			{
				foreach (KeyValuePair<int, MonsterTemplate> keyval in World.MonsterTemplates)
				{
					if (Utils.StartsWith(keyval.Value.Keywords, args[2]))
					{
						player.OutLn("[{0,5}] {1}", keyval.Key, keyval.Value.Name);
						displayed++;
					}
				}
			}
			else
			{
				player.OutLn("The first argument must be 'item' or 'monster'.");
				return;
			}
			
			if (displayed == 0)
			{
				player.OutLn("Nothing found by that name.");
			}
		}
		
		public override string GetHelpText()
		{
			return "Search for items or monsters by keyword.";
		}
		
		public override string[] GetHelpUsage()
		{
			return new string[]
			{
				"<'item' | 'monster'> <keyword>"
			};
		}
	}
}
