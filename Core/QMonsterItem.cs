using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class QMonsterItem
	{
		public enum ItemLocations
		{
			Inventory,
			Equipment
		}

		// Database attributes
		public QCompositeKey Id { get; set; }
		public ItemLocations Location { get; set; }
		public string ChoicesStr { get; set; }
		public int Maximum { get; set; }

		// Runtime attributes
		public List<QItemTemplate> Choices { get; private set; }

		public QMonsterItem ()
		{
			Id = new QCompositeKey();
			Choices = new List<QItemTemplate>();
		}

		public int MonsterId
		{
			get { return Id.Value1; }
			set { Id.Value1 = value; }
		}
	}
}
