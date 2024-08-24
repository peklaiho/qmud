using System;

namespace QMud.Core
{
	public class BodyPart
	{
		public int Health { get; set; }

		private BodyParts part;

		public BodyPart (BodyParts part)
		{
			this.Health = 100;
			this.part = part;
		}

		public bool IsDestroyed()
		{
			return Health <= 0;
		}
	}
}
