using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace QMud.Core
{
	public class Utils
	{
		private static StringBuilder CurrentLine = new StringBuilder(128);
		private static StringBuilder AllLines = new StringBuilder(2048);

		private static Regex WhiteSpaceRegex = new Regex("[\\s]+");

		private static char PrimarySeparator = ',';
		private static char SecondarySeparator = '=';

		#region String functions
		/// <summary>
		/// Makes the first character uppercase, and the rest lowercase.
		/// </summary>
		/// <param name="name">
		/// Name to capitalize. <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Capitalized string. <see cref="System.String"/>
		/// </returns>
		public static string CapitalizeName(string name)
		{
			string newName = "";

			// Check for empty name
			if (name.Length == 0)
			{
				return name;
			}

			// Uppercase first character
			newName += Char.ToUpper(name[0]);

			// Lowercase rest
			if (name.Length >= 2)
			{
				newName += LowerCase(name.Substring(1));
			}

			return newName;
		}

		/// <summary>
		/// Converts the first character of the string to uppercase.
		/// </summary>
		/// <param name="txt">
		/// String to capitalize. <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Capitalized string. <see cref="System.String"/>
		/// </returns>
		public static string CapitalizeString(string txt)
		{
			if (txt != null && txt.Length > 0)
			{
				txt = Char.ToUpper(txt[0]) + txt.Substring(1);
			}

			return txt;
		}

		public static string FormatText(string txt, int spaceBeforeStart, int spaceBetweenSentences, int lineLength)
		{
			if (txt == null || txt.Length == 0)
			{
				return txt;
			}

			// Old split
			// string[] words = txt.Split(' ');
			// New split, use regular expression
			string[] words = WhiteSpaceRegex.Split(txt);
			int spaces, wordLength;

			CurrentLine.Clear();
			AllLines.Clear();

			// Add starting indent
			for (int i = 0; i < (spaceBeforeStart - 1); i++)
			{
				CurrentLine.Append(' ');
			}

			// Go thru each word
			foreach (string word in words)
			{
				wordLength = CurrentLine.Length;

				if (wordLength > 0)
				{
					if (CurrentLine[wordLength - 1] == '.' || CurrentLine[wordLength - 1] == '!' || CurrentLine[wordLength - 1] == '?')
					{
						spaces = spaceBetweenSentences;
					}
					else
					{
						spaces = 1;
					}
				}
				else
				{
					spaces = 0;
				}

				// Remove [ ] from the word when comparing it's length.
				// Those will be translated to color codes when the text is shown to player.
				wordLength = word.Length;
				if (word[0] == '[')
				{
					wordLength -= 2;
				}

				if (lineLength == 0 || (CurrentLine.Length + spaces + wordLength) <= lineLength)
				{
					for (int i = 0; i < spaces; i++)
					{
						CurrentLine.Append(' ');
					}

					CurrentLine.Append(word);
				}
				else
				{
					AllLines.Append(CurrentLine.ToString());
					AllLines.Append(Settings.LineBreak);

					CurrentLine.Clear();
					CurrentLine.Append(word);
				}
			}

			// Add last line there too
			AllLines.Append(CurrentLine.ToString());

			return AllLines.ToString();
		}

		public static string LowerCase(string str)
		{
			string lower = "";

			for (int i = 0; i < str.Length; i++)
			{
				lower += Char.ToLower(str[i]);
			}

			return lower;
		}

		public static string SeparateThousands(long value)
		{
			string output = "", strVal = value.ToString();

			for (int i = 1; i <= strVal.Length; i++)
			{
				if ((i % 3) == 0 && strVal.Length > i)
				{
					output = " " + strVal[strVal.Length - i] + output;
				}
				else
				{
					output = strVal[strVal.Length - i] + output;
				}
			}

			return output;
		}

		public static bool StartsWith(string subject, string start)
		{
			return subject.StartsWith(start, StringComparison.OrdinalIgnoreCase);
		}

		// Will return true if one of the subjects starts with the specified string.
		public static bool StartsWith(string[] subjects, string start)
		{
			return subjects.Where(x => StartsWith(x, start)).Count() > 0;
		}

		public static bool StrCmp(string str1, string str2)
		{
			return String.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		// Will return true if one of the strings in str1 equals str2
		public static bool StrCmp(string[] str1, string str2)
		{
			return str1.Where(x => StrCmp(x, str2)).Count() > 0;
		}
		#endregion

		#region Color preferences
		public static void LoadColorPrefs(Player player, string cprefs)
		{
			Color col;
			ColorLoc loc;

			// Start with default colors
			player.SetDefaultColors();

			// If we don't have any custom pref, return
			if (cprefs.Length == 0)
			{
				return;
			}

			// Then we override any saved color prefs
			string[] settings = cprefs.Split(PrimarySeparator);

			// Loop thru the colorprefs
			foreach (string cpref in settings)
			{
				// Split for second time
				string[] key_val = cpref.Split(SecondarySeparator);

				// Check that we have 2 values
				if (key_val.Length == 2)
				{
					loc = ColorLoc.LocationList.Find(x => x.Code == key_val[0]);
					col = Color.ColorList.Find(x => x.Name == key_val[1]);

					// Set it
					if (col != null && loc != null)
					{
						player.ColorPreferences[loc.Index] = col;
					}
					else
					{
						Log.Warning("Unknown color or location in color preferences: {0}", cpref);
					}
				}
				else
				{
					Log.Warning("Unable to split color preferences: {0}", cpref);
				}
			}
		}

		public static string SaveColorPrefs(Player player)
		{
			string cprefs = "";

			foreach (ColorLoc loc in ColorLoc.LocationList)
			{
				// We only save those which aren't default
				if (loc.Default != player.ColorPreferences[loc.Index])
				{
					if (cprefs.Length > 0)
					{
						cprefs += PrimarySeparator;
					}

					cprefs += loc.Code + SecondarySeparator + player.ColorPreferences[loc.Index].Name;
				}
			}

			return cprefs;
		}
		#endregion
	}
}
