using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Commands;

namespace QMud.Core
{
	public class Fight
	{
		public static List<Fight> CurrentFights = new List<Fight>();
		private static List<Fight> TempList = new List<Fight>();

		public List<Living> Attackers { get; private set; }
		public List<Living> Defenders { get; private set; }
		public List<Living> UnactedFighters { get; private set; }

		private uint StartedOnIteration;

		public static void UpdateFights ()
		{
			TempList.Clear();
			TempList.AddRange(CurrentFights);

			foreach (Fight fight in TempList)
			{
				// We randomize fight timings based on the starting time of the fight :)
				if ((Mud.MainLoopIterations - fight.StartedOnIteration) % 30 == 0)
				{
					fight.CheckForNextRound();
				}
			}
		}

		public static void StartFight (Living attacker, Living target)
		{
			if (target.InFight != null)
			{
				// Join existing fight
				Handler.LivingToFight(attacker, target.InFight, target.InFight.Defenders.Contains(target));

				Act.ToChar("You join in the fight against §N.", attacker, null, target);
				Act.ToVict("§n joins in the fight against you.", false, attacker, null, target);
				Act.ToRoomNotVict("§n joins in the fight against §N.", true, attacker, null, target);
			}
			else
			{
				// Start new fight
				Fight fight = new Fight();
				Handler.LivingToFight(attacker, fight, true);
				Handler.LivingToFight(target, fight, false);

				Act.ToChar("You start a fight with §N.", attacker, null, target);
				Act.ToVict("§n starts a fight with you.", false, attacker, null, target);
				Act.ToRoomNotVict("§n starts a fight with §N.", true, attacker, null, target);
			}
		}

		private Fight ()
		{
			Attackers = new List<Living>();
			Defenders = new List<Living>();
			UnactedFighters = new List<Living>();

			StartedOnIteration = Mud.MainLoopIterations;

			CurrentFights.Add(this);
		}

		public void CheckForEnd ()
		{
			if (Attackers.Count == 0 || Defenders.Count == 0)
			{
				OutLn("The fight is over.");

				while (Attackers.Count > 0)
				{
					Handler.LivingFromFight(Attackers[0], false);
				}
				while (Defenders.Count > 0)
				{
					Handler.LivingFromFight(Defenders[0], false);
				}

				CurrentFights.Remove(this);
			}
		}

		public List<Living> OwnTeam (Living living)
		{
			if (Attackers.Contains(living))
			{
				return Attackers;
			}
			else if (Defenders.Contains(living))
			{
				return Defenders;
			}

			return null; // should not happen
		}

		public List<Living> OpposingTeam (Living living)
		{
			if (Attackers.Contains(living))
			{
				return Defenders;
			}
			else if (Defenders.Contains(living))
			{
				return Attackers;
			}

			return null; // should not happen
		}

		private void ActionHit (Living fighter)
		{
			// Select random target
			List<Living> targets = OpposingTeam(fighter);
			Living target = targets[Random.Range(0, targets.Count - 1)];

			Act.ToChar("You hit §N.", fighter, null, target);
			Act.ToVict("§n hits you.", false, fighter, null, target);
			Act.ToRoomNotVict("§n hits §N.", true, fighter, null, target);

			// Check for death
			if (Random.Chance(10))
			{
				Act.ToRoom("§n is dead!", true, target, null, null);
				target.Die();

				if (target.IsPlayer())
				{
					Log.Info("{0} killed by {1}.", target.GetName(), fighter.GetName());
				}
			}
		}

		private void ActionFlee (Living fighter)
		{
			int dir = Random.Range(0, 7);

			if (dir < EnumNames.DirNames.Length && fighter.InRoom.Exits[dir])
			{
				Act.ToChar("You flee from the fight.", fighter, null, null);
				Act.ToRoom("§n flees from the fight, heading " + EnumNames.DirNames[dir].ToLower() + ".", true, fighter, null, null);
				Handler.LivingToRoom(fighter, fighter.InRoom.AdjacentRoom((Directions) dir));
				Act.ToRoom("§n arrives from " + CmdMove.OppositeDirName[dir] + ".", true, fighter, null, null);

				if (fighter.IsPlayer())
				{
					CmdLook.LookAtRoom((Player) fighter);
				}
			}
			else
			{
				Act.ToChar("You attempt to flee but fail!", fighter, null, null);
				Act.ToRoom("§n attempts to flee but fails!", true, fighter, null, null);
			}
		}

		private void CheckForNextRound ()
		{
			// If we have any fighters who have not chosen the next action, we are not ready
			if (Attackers.FindAll(x => x.ChooseNextAction() == false).Count > 0 ||
				Defenders.FindAll(x => x.ChooseNextAction() == false).Count > 0)
			{
				return;
			}

			// We are ready to execute next round
			UnactedFighters.Clear();
			UnactedFighters.AddRange(Attackers);
			UnactedFighters.AddRange(Defenders);

			// Loop through all fighters and execute their actions
			while (UnactedFighters.Count > 0)
			{
				// Select random fighter
				Living fighter = UnactedFighters[Random.Range(0, UnactedFighters.Count - 1)];

				// Execute next action
				switch (fighter.NextAction)
				{
					case FightActions.Flee:
						ActionFlee(fighter);
						break;

					case FightActions.Hit:
						ActionHit(fighter);
						break;

					default:
						Log.Error("{0} has no fight action.", fighter.GetName());
						break;
				}

				// Clear fight action
				fighter.NextAction = FightActions.None;

				// Remove from list of unacted fighters
				UnactedFighters.Remove(fighter);
			}
		}

		private void OutLn (string txt)
		{
			foreach (Living lv in Attackers)
			{
				if (lv.IsPlayer())
				{
					((Player)lv).OutLn(txt);
				}
			}

			foreach (Living lv in Defenders)
			{
				if (lv.IsPlayer())
				{
					((Player)lv).OutLn(txt);
				}
			}
		}
	}
}
