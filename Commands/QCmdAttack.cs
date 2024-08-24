using System;

using QMud.Core;

namespace QMud.Commands
{
	public class QCmdAttack : QBaseCommand
	{
		public override void ExecuteCommand (QMud.Core.QPlayer player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Whom do you wish to attack?");
				return;
			}

			QLiving target = player.InRoom.FindLiving(args[1], player);

			if (target == null)
			{
				player.OutLn("No-one here by that name.");
			}
			else if (target == player)
			{
				player.OutLn("Attack yourself? Not a very smart thing to do.");
			}
			else if (!player.CanDamage(target))
			{
				player.OutLn("You cannot attack " + target.HimHer() + ".");
			}
			else
			{
				QFight.PerformDefaultAttack(player, target);
			}
		}

		public override string GetHelpText ()
		{
			return "Perform an attack on the target.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<player | monster>"
			};
		}
	}
}
