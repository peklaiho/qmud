using System;

namespace QMud.Core
{
	public class QMonsterAction
	{
		public enum TargetTypes
		{
			NoTarget,
			TargetAll,
			TargetOppSex
		};

		public QCompositeKey Id { get; set; }
		public TargetTypes TargetType { get; set; }
		public string ActRoom { get; set; }
		public string ActTarget { get; set; }
		public bool HideInvis { get; set; }
		
		public QMonsterAction ()
		{
			Id = new QCompositeKey();
		}

		public int MonsterId
		{
			get { return Id.Value1; }
			set { Id.Value1 = value; }
		}
	}
}
