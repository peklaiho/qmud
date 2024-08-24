using System;

namespace QMud.Core
{
	[Serializable]
	public class CompositeKey
	{
		public int Value1 { get; set; }
		public int Value2 { get; set; }

		public CompositeKey ()
		{

		}

		public CompositeKey (int val1, int val2)
		{
			Value1 = val1;
			Value2 = val2;
		}

		public override bool Equals (object obj)
		{
			if (obj == null || !(obj is CompositeKey))
			{
				return false;
			}

			CompositeKey other = (CompositeKey)obj;
			return this.Value1 == other.Value1 && this.Value2 == other.Value2;
		}

		public override int GetHashCode ()
		{
			return Value1 ^ Value2;
		}
	}
}
