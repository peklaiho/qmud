using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdWho : BaseCommand
	{
		public static string[] ImmLevelNames =
		{
			"Immortal",
			"Demigod",
			"God",
			"Greater God",
			"Implementor"
		};

		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			int displayed = 0;
			
			player.OutLn("Visible players:");
			
			foreach (Player target in World.Players)
			{
				// Skip over linkless players
				if (target.Descriptor == null)
				{
					continue;
				}

				if (player.CanSee(target))
				{
					// Immortals are shown in highlight color
					if (target.Level >= Player.LvlImmort)
					{
						player.OutLn("{0}{1} ({2}){3}", player.GetColorPref(ColorLoc.Highlight),
							target.Name, ImmLevelNames[target.Level - Player.LvlImmort],
							player.GetColor(Color.Reset));
					}
					else
					{
						player.OutLn(target.Name);
					}

					displayed++;
				}
			}
			
			if (displayed == 0)
			{
				player.OutLn("None!");
			}
		}
		
		public override string GetHelpText ()
		{
			return "Displays all players visible to you.";
		}
	}
}
