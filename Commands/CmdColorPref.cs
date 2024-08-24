using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdColorPref : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			Color col = null;
			ColorLoc loc = null;
			
			// Check that our color is turned on first.
			if (player.Preferences.Get((int) Player.PrefBits.Color) == false)
			{
				player.OutLn("Please turn color on first by using the 'preferences' command.");
				return;
			}
			
			if (args.Length < 3)
			{
				object[] formatArgs = new object[5];
				
				player.OutLn("Location     Color     Description");
				player.OutLn("----------------------------------");
				
				foreach (ColorLoc colloc in ColorLoc.LocationList)
				{
					if (colloc.ImmortalOnly == false || player.Level >= Player.LvlImmort)
					{
						formatArgs[0] = colloc.Code;
						formatArgs[1] = player.GetColorPref(colloc);
						formatArgs[2] = player.ColorPreferences[colloc.Index].Name;
						formatArgs[3] = player.GetColor(Color.Reset);
						formatArgs[4] = colloc.Description;
							
						player.OutLn("{0,-11}  {1}{2,-8}{3}  {4}", formatArgs);
					}
				}
				
				return;
			}
			
			// Find the location
			foreach (ColorLoc tmp in ColorLoc.LocationList)
			{
				if (tmp.ImmortalOnly == false || player.Level >= Player.LvlImmort)
				{
					if (Utils.StartsWith(tmp.Code, args[1]))
					{
						loc = tmp;
						break;
					}
				}
			}
			
			if (loc == null)
			{
				player.OutLn("Invalid location, try again.");
				return;
			}
			
			// Find the color
			col = Color.ColorList.Find(x => Utils.StartsWith(x.Name, args[2]));
			
			if (col == null)
			{
				player.OutLn("Invalid color, try again.");
				return;
			}
			
			// Set the color
			player.ColorPreferences[loc.Index] = col;
			player.OutLn("Ok.");
		}
		
		public override string GetHelpText ()
		{
			return "Used to change the color preferences to suit your personal taste. The possible " +
				"colors are: default, black, red, green, yellow, blue, magenta, cyan, white. " +
				"Additionally you can preface the color with a 'b' for the bright version, " +
				"or 'd' for the dim version.";
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"",
				"<location> <color>"
			};
		}
	}
}
