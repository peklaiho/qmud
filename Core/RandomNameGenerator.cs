using System;

namespace QMud.Core
{
	public class RandomNameGenerator
	{
		public static void Initialize ()
		{

		}

		public static string GetZoneName ()
		{
			string[] names =
			{
				"Marshes of Blackwater",
				"Green Fields of Glover",
				"Sands of the Sun",
				"Peaks of the World",
				"Evergreen Grazing Grounds"
			};

			return names[Random.Range(0, names.Length - 1)];
		}
	}
}
