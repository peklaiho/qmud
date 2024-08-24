using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Commands;

namespace QMud.Core
{
	public class Updater
	{
		private static List<Monster> TempMonsterList = new List<Monster>();
		private static List<Item> TempItemList = new List<Item>();

		public static void Update()
		{
			UpdateMonsters();
			UpdateItems();
		}

		private static void UpdateItems()
		{
			// Make copy because main list can be modified inside loop
			TempItemList.Clear();
			TempItemList.AddRange(World.Items);

			foreach (Item i in TempItemList)
			{

			}
		}

		private static void UpdateMonsters()
		{
			if (Mud.MainLoopIterations % 20 == 0)
			{
				// Make copy because main list can be modified inside loop
				TempMonsterList.Clear();
				TempMonsterList.AddRange(World.Monsters);

				foreach (Monster m in TempMonsterList)
				{
					// Fighters are handled elsewhere
					if (m.InFight != null) continue;

					// Try to aggro players
					if (m.Template.Flags.Get(MonsterTemplate.Aggro))
					{
						List<Player> visibleTargets = m.InRoom.Players.FindAll(p => m.CanSee(p) && p.Level < Player.LvlImmort);

						if (visibleTargets.Count > 0)
						{
							Fight.StartFight(m, visibleTargets[Random.Range(0, visibleTargets.Count - 1)]);
							continue;
						}
					}

					// Try to pick up items etc.
					if (m.Template.Flags.Get(MonsterTemplate.Scavenger))
					{

					}

					// No other action taken, try to walk around
					if (m.Template.Flags.Get(MonsterTemplate.Sentinel) == false)
					{
						int dir = Random.Range(0, 100);
						if (dir < EnumNames.DirNames.Length)
						{
							CmdMove.PerformMove(m, (Directions)dir);
						}
					}
				}
			}
		}
	}
}
