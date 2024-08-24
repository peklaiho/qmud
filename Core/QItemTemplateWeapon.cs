using System;

namespace QMud.Core
{
	public class QItemTemplateWeapon : QItemTypeTemplate
	{
		public int Id { get; set; }
		public QItemWeapon.WeaponTypes WeaponType { get; set; }
		public int AttackType { get; set; }
		public int Speed { get; set; }
		public QDamageTypes DamageType { get; set; }
		public int MinDamage { get; set; }
		public int MaxDamage { get; set; }
	}
}
