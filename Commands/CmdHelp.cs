using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdHelp : BaseCommand
	{
		public static void ShowHelp(Player player, BaseCommand cmd)
		{
			player.OutLn("Information about the '{0}{1}{2}' command:",
				player.GetColorPref(ColorLoc.Highlight), cmd.GetName(), player.GetColor(Color.Reset));

			if (cmd.GetHelpShortcut() != null)
			{
				player.OutLn("* This is a shortcut for the '{0}{1}{2}' command.",
					player.GetColorPref(ColorLoc.Highlight), cmd.GetHelpShortcut(), player.GetColor(Color.Reset));
			}

			if (cmd.IsImmortalCommand())
			{
				player.OutLn("* This command is only available to immortals.");
			}

			string[] usages = cmd.GetHelpUsage();

			if (usages != null)
			{
				foreach (string usage in usages)
				{
					player.OutLn("* Usage: {0}{1} {2}{3}", player.GetColorPref(ColorLoc.Highlight),
						cmd.GetName(), usage, player.GetColor(Color.Reset));
				}
			}

			string formattedHelpText = Utils.FormatText(cmd.GetHelpText(), 0, 1, player.LineLength);
			player.OutLn(formattedHelpText);
		}

		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Please type '{0}commands{1}' to get a list of available commands. Then you can",
					player.GetColorPref(ColorLoc.Highlight), player.GetColor(Color.Reset));
				player.OutLn("type '{0}help{1}' followed by a command to get information about that command.",
					player.GetColorPref(ColorLoc.Highlight), player.GetColor(Color.Reset));
				player.OutLn();
				player.OutLn("Command parameters wrapped in < > means they are required, while parameters");
				player.OutLn("wrapped in [ ] means they are optional. Some parameters have multiple values");
				player.OutLn("separated by the | character, which means one of them should be used. If the");
				player.OutLn("value is wrapped in single quotes ('), it should be written literally.");
				return;
			}
			
			foreach (BaseCommand cmd in CommandInterpreter.CommandList)
			{
				if (cmd.CanExecute(player) && Utils.StartsWith(cmd.GetName(), args[1]))
				{
					ShowHelp(player, cmd);
					return;
				}
			}
			
			player.OutLn("No command found by that name, try again.");
		}
		
		public override string GetHelpText ()
		{
			return "Displays information about a specific command.";
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"",
				"<command>"
			};
		}
	}
}
