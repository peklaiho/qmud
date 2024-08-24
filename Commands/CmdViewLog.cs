using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;
using QMud.Database;

namespace QMud.Commands
{
	public class CmdViewLog : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Please specify the type of log entries you wish to view.");
				return;
			}
			
			List<string> list = null;
			
			if (Utils.StartsWith("info", args[1]))
			{
				list = Log.InfoList;
			}
			else if (Utils.StartsWith("warning", args[1]))
			{
				list = Log.WarningList;
			}
			else if (Utils.StartsWith("error", args[1]))
			{
				list = Log.ErrorList;
			}
			else if (Utils.StartsWith("godcmd", args[1]))
			{
				list = Log.GodCmdList;
			}
			else if (Utils.StartsWith("debug", args[1]))
			{
				list = Log.DebugList;
			}
			else
			{
				player.OutLn("Unknown log entry type.");
				return;
			}
			
			if (list.Count == 0)
			{
				player.OutLn("No entries.");
			}
			else
			{
				if (list.Count > 20)
				{
					list.Skip(list.Count - 20).ToList().ForEach(x => player.OutLn(x));
				}
				else
				{
					list.ForEach(x => player.OutLn(x));
				}
			}
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<'info' | 'warning' | 'error' | 'godcmd' | 'debug'>"
			};
		}
		
		public override string GetHelpText ()
		{
			return "Display last 20 log entries of the specified type.";
		}
	}
}
