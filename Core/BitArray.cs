using System;

namespace QMud.Core
{
	/// <summary>
	/// Similar to System.Collections.BitArray class, but with the included conversion from/to integer.
	/// </summary>
	public class BitArray
	{
		private const string StringChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+*";

		private ulong val = 0;
		
		public BitArray()
		{
			
		}

		public BitArray(ulong startVal)
		{
			SetLongValue(startVal);
		}

		public BitArray(string stringVal)
		{
			SetStringValue(stringVal);
		}

		public void Clear(int bit)
		{
			val = val & (~(1UL << bit));
		}
		
		public bool Get(int bit)
		{
			return (val & (1UL << bit)) != 0;
		}
		
		public ulong GetLongValue()
		{
			return val;
		}

		public string GetStringValue()
		{
			string str = "";

			for (int i = 0; i < 64; i++)
			{
				if (Get(i))
				{
					str += StringChars[i];
				}
			}

			return str;
		}

		public void Set(int bit)
		{
			val = val | (1UL << bit);
		}
		
		public void SetLongValue(ulong newVal)
		{
			val = newVal;
		}

		public void SetStringValue(string stringVal)
		{
			val = 0;

			foreach (char c in stringVal)
			{
				Set(StringChars.IndexOf(c));
			}
		}

		public bool Toggle(int bit)
		{
			val = val ^ (1UL << bit);
		
			// Returns the new state
			return Get(bit);
		}
	}
}
