using System;
using System.Collections.Generic;

using QMud.Commands;

namespace QMud.Core
{
	public class QFight
	{
		public static string[,] AttackTypes =
		{
			// Same as in CircleMud for nostalgia :)
			{ "hit", "hits", "hitting" },					// 0
			{ "sting", "stings", "stinging" },				// 1
			{ "whip", "whips", "whipping" },				// 2
			{ "slash", "slashes", "slashing" },				// 3
			{ "bite", "bites", "biting" },					// 4
			{ "bludgeon", "bludgeons", "bludgeoning" },		// 5
			{ "crush", "crushes", "crushing" },				// 6
			{ "pound", "pounds", "pounding" },				// 7
			{ "claw", "claws", "clawing" },					// 8
			{ "maul", "mauls", "mauling" },					// 9
			{ "thrash", "thrashes", "thrashing" },			// 10
			{ "pierce", "pierces", "piercing" },			// 11
			{ "blast", "blasts", "blasting" },				// 12
			{ "punch", "punches", "punching" },				// 13
			{ "stab", "stabs", "stabbing" },				// 14
			{ "impale", "impales", "impaling" },			// 15

			// New ones
			{ "shoot", "shoots", "shooting" },				// 16
			{ "swing", "swings", "swinging" },				// 17
			{ "thrust", "thrusts", "thrusting" },			// 18
			{ "chop", "chops", "chopping" },				// 19
		};

		// Returns true if the target died, otherwise false.
		public static bool Damage(QLiving source, QLiving target, int damage)
		{
			// Damage cannot be higher than remaining health
			damage = Math.Min(damage, target.Health);

			// Lower target health
			target.Health -= damage;

			// Gain experience for the hit
			if (source.IsPlayer() && target.IsMonster())
			{
				// Multiply experience by damage
				int xp = damage * ((QMonster)target).Template.Experience;

				((QPlayer)source).GainExperience(xp);
			}

			// Check for death
			if (target.Health <= 0)
			{
				HandleDeath(source, target);
				return true;
			}

			return false;
		}

		public static void PerformSkill(QLiving source, QLiving target, QCmdSkill.Skills skill)
		{

		}

		public static void PerformDefaultAttack(QLiving source, QLiving target)
		{
			// Default values for bare hand attack
			QDamageTypes damType = QDamageTypes.Physical;
			int attackType = 0;
			int minDamage = 4;
			int maxDamage = 8;
			int speed = 40;

			// Try to use a weapon instead
			QItemWeapon weapon = null;
			List<QItemWeapon> weapons = source.GetWeapons();
			if (weapons.Count > 0)
			{
				// Select a weapon (random for now)
				weapon = weapons[QRandom.Range(0, weapons.Count - 1)];

				damType = weapon.DamageType;
				attackType = weapon.AttackType;
				minDamage = weapon.MinDamage;
				maxDamage = weapon.MaxDamage;
				speed = weapon.Speed;
			}

			if (CalculateHit(source, target))
			{
				int baseDamage = QRandom.Range(minDamage, maxDamage);
				int resist = target.GetResistance(damType);
				int damageAfterResist = CalculateResistance(baseDamage, resist);

				// Damage has to be at least 1
				int damage = Math.Max(1, damageAfterResist);
			
				// Show attack message
				DisplayHitMessage(source, target, damage, attackType, damType, weapon);

				// Perform damage
				Damage(source, target, damage);
			}
			else
			{
				// Show miss message
				DisplayHitMessage(source, target, 0, attackType, damType, weapon);
			}

			// Add balance (or delay) for the attacker
			source.Balance = speed;
		}

		private static bool CalculateHit(QLiving source, QLiving target)
		{
			return QRandom.Range(0, 4) > 0;
		}

		private static int CalculateResistance(int dam, int res)
		{
			if (res == 0)
			{
				return dam;
			}
			// Negative resistance, increase damage, for example:
			// Resistance -50 -> damage multiplier 1.5
			// Resistance -100 -> damage multiplier 2
			else if (res < 0)
			{
				return (int) (dam * (1.0 + (res / -100.0)));
			}
			// Positive resistance, decrease damage, for example:
			// Resistance 20 -> damage multiplier 0.8
			// Resistance 70 -> damage multiplier 0.3
			else
			{
				return (int) (dam * ((100.0 - res) / 100.0));
			}
		}

		private static void DisplayHitMessage(QLiving source, QLiving target, int damage, int attType,
		                                      QDamageTypes damType, QItemWeapon weapon)
		{
			string msgToChar, msgToVict, msgToRoom;

			msgToChar = "You " + AttackTypes[attType, 0];
			msgToVict = "§n " + AttackTypes[attType, 1];
			msgToRoom = "§n " + AttackTypes[attType, 1];

			if ((attType == 17 || attType == 18) && weapon != null)
			{
				msgToChar += " your §p";
				msgToVict += " §s §p";
				msgToRoom += " §s §p";

				if (damage == 0)
				{
					msgToChar += " at";
					msgToVict += " at";
					msgToRoom += " at";
				}
				else
				{
					msgToChar += " into";
					msgToVict += " into";
					msgToRoom += " into";
				}
			}
			else
			{
				if (damage == 0)
				{
					msgToChar += " at";
					msgToVict += " at";
					msgToRoom += " at";
				}
			}

			msgToChar += " §N";
			msgToVict += " you";
			msgToRoom += " §N";

			if ((attType != 17 && attType != 18) && weapon != null)
			{
				msgToChar += " with your §p";
				msgToVict += " with §s §p";
				msgToRoom += " with §s §p";
			}

			if (damage == 0)
			{
				msgToChar += " but miss.";
				msgToVict += " but misses.";
				msgToRoom += " but misses.";
			}
			else
			{
				string dmgTypeName = QUtils.LowerCase(QEnumNames.DamageTypeNames[(int)damType]);

				msgToChar += " for [" + damage + "] " + dmgTypeName + " damage.";
				msgToVict += " for [" + damage + "] " + dmgTypeName + " damage.";
				msgToRoom += " for [" + damage + "] " + dmgTypeName  + " damage.";
			}

			QAct.ToChar(msgToChar, source, weapon, target);
			QAct.ToVict(msgToVict, false, source, weapon, target);
			QAct.ToRoomNotVict(msgToRoom, false, source, weapon, target);
		}

		private static void HandleDeath(QLiving source, QLiving target)
		{
			QAct.ToRoom("§n dies.", false, target, null, null);

			if (target.IsPlayer())
			{
				QLog.Info(target.GetName() + " was killed by " + source.GetName() + ".");
				QAct.ToVict("You have been killed by §n.", false, source, null, target);

				QHandler.LivingToRoom(target, QWorld.Rooms[QSettings.StartingRoomId]);
				QAct.ToRoom("§n has died and has been resurrected.", true, target, null, null);

				target.Health = 1;
				target.Balance = 0;

				QPlayer plr = (QPlayer) target;
				plr.OutLn();
				QCmdLook.LookAtRoom(plr, true);
			}
			else
			{
				QHandler.ExtractLiving(target);
			}
		}
	}
}
