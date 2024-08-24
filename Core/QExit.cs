using System;

namespace QMud.Core
{
	public class QExit
	{
		// Database attributes
		public int Id { get; set; }
		public int FromId { get; set; }
		public int ToId { get; set; }
		public QDirections Direction { get; set; }

		// Runtime attributes
		public QRoom To { get; set; }
		
		public QExit ()
		{

		}
	}
}
