using System;
using System.Linq;
using System.Diagnostics;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdMemory : BaseCommand
	{	
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			player.OutLn("Using {0} bytes of memory.",
				Utils.SeparateThousands(Process.GetCurrentProcess().WorkingSet64));
			player.OutLn("{0} players connected.", World.Players.Count);
			player.OutLn("{0} zones containing {1} rooms.", World.Zones.Count, World.Zones.Sum(x => x.Template.Width * x.Template.Height));
			player.OutLn("{0} monster templates and {1} item templates.", World.MonsterTemplates.Count, World.ItemTemplates.Count);
			player.OutLn("{0} monsters and {1} items loaded.", World.Monsters.Count, World.Items.Count);
			player.OutLn("{0} parties and {1} fights.", Party.PartyList.Count, Fight.CurrentFights.Count);
		}
		
		public override string GetHelpText ()
		{
			return "Shows memory usage and statistics about the game.";
		}
	}
}
