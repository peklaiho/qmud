using System;

namespace QMud.Core
{
	public class QItemArmor : QItem
	{
		public QItemArmor(QItemTemplate template)
			: base(template)
		{

		}

		public int ArmorClass { get { return 0; } }
		public int DamageReduction { get { return 0; } }
	}
}
