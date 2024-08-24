using System;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdMove : BaseCommand
	{
		public static string[] OppositeDirName =
		{
			"south",
			"west",
			"north",
			"east",
		};

		private Directions dir;

		public static bool PerformMove(Living mover, Directions to)
		{
			if (!mover.InRoom.Exits[(int)to])
			{
				return false;
			}

			Room target = mover.InRoom.AdjacentRoom(to);

			Act.ToRoom("§n leaves " + EnumNames.DirNames[(int) to].ToLower() + ".", true, mover, null, null);
			Handler.LivingToRoom(mover, target);
			Act.ToRoom("§n arrives from " + OppositeDirName[(int) to] + ".", true, mover, null, null);

			return true;
		}

		public CmdMove (Directions newDir)
		{
			dir = newDir;
		}

		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (player.InFight != null)
			{
				player.OutLn("You are in a fight! Try to flee instead.");
				return;
			}

			if (PerformMove(player, dir))
			{
				// Look at room
				CmdLook.LookAtRoom(player);
			}
			else
			{
				player.OutLn("You cannot travel to that direction.");
			}
		}

		public override string GetHelpText ()
		{
			return "Move to the next room in this direction.";
		}
	}
}
