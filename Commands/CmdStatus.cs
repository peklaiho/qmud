using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdStatus : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			player.OutLn("You are " + Utils.LowerCase(EnumNames.GenderNames[(int) player.GetSex()]) +
			                       " character [" + player.GetName() + "].");

			if (player.Level >= Player.LvlImmort)
			{
				player.OutLn("You are an administrator at the level of [" +
				             CmdWho.ImmLevelNames[player.Level - Player.LvlImmort] + "] (" + player.Level + ").");
			}
			else
			{
				player.OutLn("You are level [" + player.Level + "].");
			}
		}

		public override string GetHelpText ()
		{
			return "Display information about your character.";
		}
	}
}
