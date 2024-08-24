using System;

namespace QMud.Core
{
	public class QItemWeapon : QItem
	{
		public enum WeaponTypes
		{
			Melee1H,
			Melee2H,
			Ranged1H,
			Ranged2H
		};

		public QItemWeapon (QItemTemplate template)
			: base(template)
		{

		}

		public bool IsMelee
		{
			get
			{
				return WeaponType == WeaponTypes.Melee1H || WeaponType == WeaponTypes.Melee2H;
			}
		}

		public bool IsRanged
		{
			get
			{
				return WeaponType == WeaponTypes.Ranged1H || WeaponType == WeaponTypes.Ranged2H;
			}
		}

		public int RequiredHands
		{
			get
			{
				if (WeaponType == WeaponTypes.Melee1H || WeaponType == WeaponTypes.Ranged1H)
				{
					return 1;
				}

				return 2;
			}
		}

		public string TypeAsString()
		{
			if (IsMelee)
			{
				return "melee";
			}

			return "ranged";
		}

		public string HandsAsString()
		{
			if (RequiredHands == 1)
			{
				return "one hand";
			}

			return "two hands";
		}

		#region Getters from template
		public WeaponTypes WeaponType
		{
			get
			{
				if (Template.TypeTemplate != null)
				{
					return ((QItemTemplateWeapon)Template.TypeTemplate).WeaponType;
				}
				return WeaponTypes.Melee1H;
			}
		}

		public int AttackType
		{
			get
			{
				if (Template.TypeTemplate != null)
				{
					return ((QItemTemplateWeapon)Template.TypeTemplate).AttackType;
				}
				return 0;
			}
		}

		public QDamageTypes DamageType
		{
			get
			{
				if (Template.TypeTemplate != null)
				{
					return ((QItemTemplateWeapon)Template.TypeTemplate).DamageType;
				}
				return QDamageTypes.Physical;
			}
		}

		public int MinDamage
		{
			get
			{
				if (Template.TypeTemplate != null)
				{
					return ((QItemTemplateWeapon)Template.TypeTemplate).MinDamage;
				}
				return 1;
			}
		}

		public int MaxDamage
		{
			get
			{
				if (Template.TypeTemplate != null)
				{
					return ((QItemTemplateWeapon)Template.TypeTemplate).MaxDamage;
				}
				return 1;
			}
		}

		public int Speed
		{
			get
			{
				if (Template.TypeTemplate != null)
				{
					return ((QItemTemplateWeapon)Template.TypeTemplate).Speed;
				}

				return 40;
			}
		}

		public bool CanBackstab
		{
			get
			{
				return Template.Flags.Get((int) QItemFlags.Backstab);
			}
		}
		#endregion
	}
}
