using System;
using System.Collections;

namespace QMud.Core
{
	// Computer attributes (stuff which is affected by equipment)
	public class QAttributes
	{
		public enum Attributes
		{
			MaxHealth,
			MaxActions,
			MaxMovement,

			Strength,
			Agility,
			Intelligence,
			Vitality,

			ResistPhysical,
			ResistFire,
			ResistCold,
			ResistLightning,
			ResistPoison
		}

		// Return base values for now
		public static int Get(QPlayer player, Attributes attrib)
		{
			int val = 0;

			switch (attrib)
			{
				case Attributes.MaxHealth:
					val = 40 + (Get(player, Attributes.Vitality) * 6);
					break;
				case Attributes.MaxActions:
					val = 40 + (Get(player, Attributes.Agility) * 3) + (Get(player, Attributes.Intelligence) * 3);
					break;
				case Attributes.MaxMovement:
					val = 80 + Get(player, Attributes.Agility) + Get(player, Attributes.Vitality);
					break;

				case Attributes.Strength:
					val = player.Strength;
					break;
				case Attributes.Agility:
					val = player.Agility;
					break;
				case Attributes.Intelligence:
					val = player.Intelligence;
					break;
				case Attributes.Vitality:
					val = player.Vitality;
					break;

				case Attributes.ResistPhysical:
					val = 0;
					break;
				case Attributes.ResistFire:
					val = -100;
					break;
				case Attributes.ResistCold:
					val = -100;
					break;
				case Attributes.ResistLightning:
					val = -100;
					break;
				case Attributes.ResistPoison:
					val = -100;
					break;
			}

			return val;
		}
	}
}
