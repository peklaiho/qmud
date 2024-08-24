using System;

namespace QMud.Core
{
	public class EnumNames
	{
		public static string[] DirNames = Enum.GetNames(typeof(Directions));
		public static string[] GenderNames = Enum.GetNames(typeof(Genders));
	}
}
