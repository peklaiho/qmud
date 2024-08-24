using System;

namespace QMud.Core
{
	public class QAffections
	{
		public enum Affections
		{
			None,					// 0

			MinDamage,				// 1
			MaxDamage,				// 2
			DamagePercent,			// 3
			DamageType,				// 4
			ToHit,					// 5

			Armor,					// 6
			ArmorPercent,			// 7

			Strength,				// 8
			Agility,				// 9
			Intelligence,			// 10
			Vitality,				// 11
			AllAttributes,			// 12

			ResistFire,				// 13
			ResistCold,				// 14
			ResistLightning,		// 15
			ResistPoison,			// 16
			ResistAll,				// 17

			MaxHealth,				// 18
			MaxActions,				// 19
			MaxMovement,			// 20
			MaxHealthPercent,		// 21
			MaxActionsPercent,		// 22
			MaxMovementPercent,		// 23

			AllSkills				// 24
		};
	}
}
