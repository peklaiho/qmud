using System;

namespace QMud.Core
{
	public class QCoords
		: IEquatable<QCoords>
	{
		public int X;
		public int Y;
		public int Z;

		public QCoords () { }

		public QCoords (int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public QCoords (QCoords other)
		{
			this.X = other.X;
			this.Y = other.Y;
			this.Z = other.Z;
		}

		public QCoords (string strValue)
		{
			SetValue(strValue);
		}

		public void SetValue(string strValue)
		{
			string[] tmp = strValue.Split(',');

			if (tmp.Length != 3 || !Int32.TryParse(tmp[0], out X) ||
			    !Int32.TryParse(tmp[1], out Y) || !Int32.TryParse(tmp[2], out Z))
			{
				QLog.Error("Invalid string value '" + strValue + "' for QCoords.");
			}
		}

		public bool Equals (QCoords other)
		{
			return (this.X == other.X && this.Y == other.Y && this.Z == other.Z);
		}

		public QCoords Step (QDirections dir)
		{
			QCoords next = new QCoords(this);

			switch (dir)
			{
				case QDirections.North:
					next.Y++;
					break;
				case QDirections.East:
					next.X++;
					break;
				case QDirections.South:
					next.Y--;
					break;
				case QDirections.West:
					next.X--;
					break;
				case QDirections.Up:
					next.Z++;
					break;
				case QDirections.Down:
					next.Z--;
					break;
			}

			return next;
		}

		public override int GetHashCode ()
		{
			return (X ^ Y ^ Z).GetHashCode();
		}

		public override string ToString ()
		{
			return String.Format("{0},{1},{2}", X, Y, Z);
		}
	}
}
