using System;
using System.Collections.Generic;
using System.Linq;

using QMud.Core;

namespace QMud.Commands
{
	public class QCmdSkill : QBaseCommand
	{
		public enum Skills
		{
			Backstab,
			Cleave,
			DualStrike,
		}

		private Skills Skill;

		public QCmdSkill (Skills newSkill)
		{
			Skill = newSkill;
		}

		public override void ExecuteCommand (QPlayer player, string[] args, string wholeArg)
		{
			List<QLiving> targets = new List<QLiving>();

			if (player.Actions < 10)
			{
				player.OutLn("You need at least 10 action points to use this skill.");
				return;
			}

			// Find target for single target skills
			if (Skill != Skills.Cleave)
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
					return;
				}
				else if (target == player)
				{
					player.OutLn("Attack yourself? Not a very smart thing to do.");
					return;
				}
				else if (!player.CanDamage(target))
				{
					player.OutLn("You cannot attack " + target.HimHer() + ".");
					return;
				}

				targets.Add(target);
			}
			// Find targets for AoE skills
			else
			{
				// Add both monsters and players
				targets.AddRange(player.InRoom.Players.FindAll(x => x != player && player.CanSee(x) && player.CanDamage(x)));
				targets.AddRange(player.InRoom.Monsters.FindAll(x => player.CanSee(x) && player.CanDamage(x)));

				if (targets.Count == 0)
				{
					player.OutLn("This room contains no-one who you can attack.");
					return;
				}
			}

			List<QItemWeapon> meleeWeapons = player.GetWeapons().FindAll(x => x.IsMelee);

			switch (Skill)
			{
				case Skills.Cleave:
					if (meleeWeapons.Count < 1)
					{
						player.OutLn("You need to wield a melee weapon to use the 'cleave' skill.");
						return;
					}

					// Limit targets to 3
					while (targets.Count > 3)
					{
						// Remove random
						targets.RemoveAt(QRandom.Range(0, targets.Count - 1));
					}

					// Ok, attack them
					foreach (QLiving target in targets)
					{
						QFight.PerformSkill(player, target, Skills.Cleave);
					}
					break;

				case Skills.DualStrike:
					if (meleeWeapons.Count < 2)
					{
						player.OutLn("You need to wield two melee weapons to use the 'dualstrike' skill.");
						return;
					}

					QFight.PerformSkill(player, targets[0], Skills.DualStrike);
					break;
			}

			// Decrease action points
			player.Actions -= 10;
		}

		public override string GetHelpText ()
		{
			switch (Skill)
			{
				case Skills.Cleave:
					return "Attack up to 3 random enemies in the room. You need to wield a melee weapon to use this skill. This skill requires 10 actions points to use.";
				case Skills.DualStrike:
					return "Attack an enemy with both weapons. You need to wield two melee weapons to use this skill. This skill requires 10 actions points to use.";
				default:
					return "Unknown skill.";
			}
		}

		public override string[] GetHelpUsage ()
		{
			if (Skill == Skills.Cleave)
			{
				return base.GetHelpUsage();
			}

			return new string[]
			{
				"<target>"
			};
		}
	}
}
