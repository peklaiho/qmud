using System;
using System.Linq;

using QMud.Core;

namespace QMud.Commands
{
	public class QCmdQueue : QBaseCommand
	{
		public override void ExecuteCommand (QPlayer player, string[] args, string wholeArg)
		{
			if (player.CommandQueue.Count == 0)
			{
				player.OutLn("You do not have any queued commands.");
			}
			else
			{
				if (args.Length >= 2)
				{
					player.CommandQueue.Clear();
					player.OutLn("Command queue cleared.");
				}
				else
				{
					player.OutLn("Commands in queue:");
					foreach (string cmd in player.CommandQueue)
					{
						player.OutLn("=> " + cmd);
					}
				}
			}
		}

		public override string GetHelpText ()
		{
			return "Show queued commands. They will be executed in order as soon as you regain your balance. You can also clear your queue.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"",
				"<'clear'>"
			};
		}
	}
}
