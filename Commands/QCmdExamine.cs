using System;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class QCmdExamine : QBaseCommand
	{
		public override void ExecuteCommand (QMud.Core.QPlayer player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Examine what?");
				return;
			}

			QItem item = player.FindItem(args[1], QLiving.SearchRoom | QLiving.SearchInv | QLiving.SearchEq);

			if (item == null)
			{
				player.OutLn("Nothing to examine by that name.");
			}
			else
			{
				List<string> hints = new List<string>();

				QAct.ToChar("You examine §t carefully to learn more about it.", player, item, null);

				// player.OutLn("You learn that it belongs to the [" + QUtils.LowerCase(QItemTemplate.ItemTypeNames[(int) item.Template.ItemType]) + "] category of items.");

				if (item.CanBeWorn())
				{
					string wearLocName1 = QCmdEquipment.wearLocationNames[(int) item.Template.WearLocation, 0];
					string wearLocName2 = QCmdEquipment.wearLocationNames[(int) item.Template.WearLocation, 1];

					player.OutLn("You learn that it can be worn [" + wearLocName1 + " " + wearLocName2 + "].");

					// Hint for wearable items
					if (player.CanWear(item) && player.FreeSlotFor(item))
					{
						hints.Add("Use '[wear]' to start using it.");
					}
				}

				if (item.IsContainer())
				{
					player.OutLn("You learn that it is a container and can be used to store other items.");
					hints.Add("Use '[put]' to put objects inside it.");
					hints.Add("Use '[look in]' to see what is inside.");
				}

				switch (item.Template.ItemType)
				{
					case QItemTypes.Weapon:
						QItemWeapon weapon = (QItemWeapon) item;

						player.OutLn(String.Format("You learn that it is a {0} weapon and requires [{1}] to wield.",
					                           weapon.TypeAsString(), weapon.HandsAsString()));

						player.OutLn(String.Format("You learn that it does [{0}] to [{1}] points of [{2}] damage.",
						             weapon.MinDamage, weapon.MaxDamage,
						             QUtils.LowerCase(QEnumNames.DamageTypeNames[(int) weapon.DamageType])));

						player.OutLn(String.Format("You learn that it has an attack speed of [{0:f1}] seconds.", weapon.Speed / 10.0f));

						float avgDam = ((weapon.MaxDamage - weapon.MinDamage) / 2.0f) + weapon.MinDamage;
						float dps = (avgDam / weapon.Speed) * 10;

						player.OutLn(String.Format("Thus, you quickly calculate that it does [{0:f1}] damage per second.", dps));

						if (weapon.CanBackstab)
						{
							player.OutLn("You learn that this weapon can be used with the [backstab] skill.");
						}

						if (player.CanWear(item) && player.FreeHandFor(weapon))
						{
							hints.Add("Use '[wield]' to start using it.");
						}

						break;

					case QItemTypes.Armor:
					case QItemTypes.Shield:
						QItemArmor armor = (QItemArmor) item;
						ReportStat(player, armor.ArmorClass, "armor class");
						ReportStat(player, armor.DamageReduction, "damage reduction");
						break;
				}

				// Hint for description
				if (item.Template.LongDescription.Length > 0)
				{
					hints.Add("Use '[look]' to read a detailed description about the item.");
				}

				// Show hints
				hints.ForEach(x => player.Hint(x));
			}
		}

		public override string GetHelpText ()
		{
			return "Used to learn detailed information about an item.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<item>"
			};
		}

		private void ReportStat(QPlayer player, int value, string name)
		{
			if (value != 0)
			{
				player.OutLn("You learn that it " + QUtils.IncDecr(value) +
				                       " your " + name + " by [" + Math.Abs(value) + "].");
			}
		}
	}
}
