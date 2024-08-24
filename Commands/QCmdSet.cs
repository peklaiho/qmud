using System;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class QCmdSet : QBaseCommand
	{
		private static object[,] Setters =
		{
			// Name, min, max
			{ "experience", 0, 2000000000 },	// 0
			{ "gold", 0, 2000000000 },			// 1
			{ "level", 1, 65 },					// 2

			{ "actions", 0, 5000 },				// 3
			{ "health", 1, 5000 },				// 4
			{ "movement", 0, 5000 },			// 5

			{ "agility", 0, 500 },				// 6
			{ "intelligence", 0, 500 },			// 7
			{ "strength", 0, 500 },				// 8
			{ "vitality", 0, 500 }				// 9
		};

		public override void ExecuteCommand (QPlayer player, string[] args, string wholeArg)
		{
			int val, displayed = 0;

			if (args.Length < 4)
			{
				// Show all possible attributes
				player.OutLn("Attributes which can be set:");

				for (int i = 0; i < Setters.GetLength(0); i++)
				{
					player.Out(String.Format("{0,-14}", (string) Setters[i, 0]));

					if (((++displayed) % 5) == 0)
					{
						player.OutLn();
					}
				}

				if ((displayed % 5) != 0)
				{
					player.OutLn();
				}

				return;
			}

			// Find the target player
			QPlayer target = QWorld.FindPlayerByName(args[1], player);

			if (target == null)
			{
				player.OutLn("No player found by that name.");
				return;
			}

			// Parse the value
			if (!Int32.TryParse(args[3], out val))
			{
				player.OutLn("Invalid value.");
				return;
			}

			// Never allowed to touch other gods of same level or higher
			if (player.IsFounder() == false)
			{
				if (player != target && player.Level <= target.Level)
				{
					player.OutLn("You are not allowed to set attributes of people who are your level or higher.");
					QLog.GodCmd(player.GetName() + " attempted to use set on " + target.GetName() + ".");
					return;
				}
			}

			for (int i = 0; i < Setters.GetLength(0); i++)
			{
				string setName = (string) Setters[i, 0];
				int setMin = (int) Setters[i, 1];
				int setMax = (int) Setters[i, 2];

				if (QUtils.StartsWith(setName, args[2]))
				{
					// Check for min/max
					if (val < setMin || val > setMax)
					{
						player.OutLn("Value for " + setName + " must be between " + setMin + " and " + setMax + ".");
						return;
					}

					// Some additional checks for setting level
					if (player.IsFounder() == false && QUtils.StrCmp(setName, "level"))
					{
						if (player == target)
						{
							player.OutLn("You cannot change your own level.");
							return;
						}

						if (val >= player.Level)
						{
							player.OutLn("You cannot raise the level of the target to higher or equal than your own.");
							return;
						}
					}

					// All is ok, we can perform the set
					player.OutLn(QUtils.CapitalizeString(setName) + " for " + target.Name + " has been set to " + val + ".");
					QLog.GodCmd(player.Name + " set the " + setName + " of " + target.Name + " to " + val + ".");

					// Set the correct value
					switch (i)
					{
						case 0:
							target.Experience = val;
							break;
						case 1:
							target.Gold = val;
							break;
						case 2:
							target.Level = val;
							break;
						case 3:
							target.Actions = val;
							break;
						case 4:
							target.Health = val;
							break;
						case 5:
							target.Movement = val;
							break;
						case 6:
							target.Agility = val;
							break;
						case 7:
							target.Intelligence = val;
							break;
						case 8:
							target.Strength = val;
							break;
						case 9:
							target.Vitality = val;
							break;
					}

					// Stop the loop
					return;
				}
			}

			player.OutLn("Unknown attribute.");
		}

		public override string GetHelpText ()
		{
			return "Set the player's specified attribute to the specified value. Type the command " +
				"without arguments to show all possible attributes which can be set.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"",
				"<player> <attribute> <value>"
			};
		}
	}
}
