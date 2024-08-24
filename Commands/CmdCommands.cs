using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdCommands : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			int displayed = 0;

			if (args.Length >= 2)
			{
				player.OutLn("Commands currently available to you starting with '{0}':", args[1]);
			}
			else
			{
				player.OutLn("Commands currently available to you:");
			}

			foreach (BaseCommand cmd in CommandInterpreter.CommandList)
			{
				if (cmd.CanExecute(player) == false)
				{
					continue;
				}

				// If the optional argument is given, the command name must start with it
				if (args.Length >= 2 && Utils.StartsWith(cmd.GetName(), args[1]) == false)
				{
					continue;
				}

				if (player.Level >= Player.LvlImmort && cmd.IsImmortalCommand())
				{
					player.Out(player.GetColorPref(ColorLoc.Highlight));
				}

				player.Out("{0,-14}", cmd.GetName());
				player.Out(player.GetColor(Color.Reset));

				if (((++displayed) % 5) == 0)
				{
					player.OutLn();
				}
			}

			if (displayed == 0)
			{
				player.OutLn("None!");
			}
			else if ((displayed % 5) != 0)
			{
				player.OutLn();
			}
		}

		public override string GetHelpText ()
		{
			return "Displays all commands which are currently available to you. You can use the " +
				"optional parameter to limit results by displaying only commands with names " +
				"starting by the specified string.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"[name_starts_with]"
			};
		}
	}
}
