using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdPreferences : BaseCommand
	{
		// Make sure to add new preferences to QPlayer.PrefBits

		private static object[,] Prefs =
		{
			{
				"AutoCombat",
				"Automatically hit NPCs in combat",
				"You will now automatically hit NPCs in combat.",
				"You will no longer automatically hit NPCs in combat.",
				0
			},
			{
				"Color",
				"Color (use the 'colorpref' command to set your colors)",
				"You will now see colors.",
				"You will no longer see colors.",
				0
			},
			{
				"Compact",
				"Compact mode (do not add extra linebreak before prompt)",
				"You will now use compact mode.",
				"You will no longer use compact mode.",
				0
			},
			{
				"Hints",
				"Show additional hints (useful for beginners)",
				"You will now see hints.",
				"You will no longer see hints.",
				0
			},
		};

		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Your current preferences:");

				for (int i = 0; i < Prefs.GetLength(0); i++)
				{
					if (player.Level >= (int) Prefs[i, 4])
					{
						string onOff = player.Preferences.Get(i) ? "ON" : "OFF";

						player.OutLn("{0,-12}  {1,-3}  {2}", (string) Prefs[i, 0], onOff, (string) Prefs[i, 1]);
					}
				}

				return;
			}

			for (int i = 0; i < Prefs.GetLength(0); i++)
			{
				if (Utils.StartsWith((string) Prefs[i, 0], args[1]) && player.Level >= (int) Prefs[i, 4])
				{
					if (player.Preferences.Get(i))
					{
						player.Preferences.Clear(i);
						player.OutLn((string) Prefs[i, 3]);
					}
					else
					{
						player.Preferences.Set(i);
						player.OutLn((string) Prefs[i, 2]);
					}

					return;
				}
			}

			// Not found
			player.OutLn("Invalid preference. Try again.");
		}

		public override string GetHelpText ()
		{
			return "Used to toggle various preferences which can be turned on and off.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"",
				"<preference>"
			};
		}
	}
}
