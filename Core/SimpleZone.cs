using System;

namespace QMud.Core
{
	public class SimpleZone : Zone
	{
		public SimpleZone ()
		{
			
		}

		public override void Initialize()
		{
			// Connect all exits
			for (int x = 0; x < Template.Width; x++)
			{
				for (int y = 0; y < Template.Height; y++)
				{
					if (x < (Template.Width - 1))
					{
						GetRoom(x, y).Exits[(int) Directions.East] = true;
					}
					if (x > 0)
					{
						GetRoom(x, y).Exits[(int) Directions.West] = true;	
					}
					if (y < (Template.Height - 1))
					{
						GetRoom(x, y).Exits[(int) Directions.South] = true;
					}
					if (y > 0)
					{
						GetRoom(x, y).Exits[(int) Directions.North] = true;
					}
				}
			}

			// Start at middle
			StartingRoom = GetRoom(Template.Width / 2, Template.Height / 2);
		}
	}
}
