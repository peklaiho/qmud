using System;

namespace QMud.Core
{
	// This class is only used for database operations
	public class PlayerItem
	{
		public CompositeKey Id { get; set; }
		public int ItemId { get; set; }
		public int Parent { get; set; }
		public int Location { get; set; }

		public PlayerItem ()
		{
			Id = new CompositeKey();
		}

		public int PlayerId
		{
			get { return Id.Value1; }
			set { Id.Value1 = value; }
		}
		public int Number
		{
			get { return Id.Value2; }
			set { Id.Value2 = value; }
		}
	}
}
