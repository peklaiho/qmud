using System;
using System.Collections.Generic;

namespace QMud.Core
{
	/// <summary>
	/// Class that represents a player character.
	/// </summary>
	public class Player : Living
	{
		// Constants
		public const int LvlImmort = 61;
		public const int LvlDemiGod = 62;
		public const int LvlGod = 63;
		public const int LvlGrGod = 64;
		public const int LvlImpl = 65;

		// Make sure to add new preferences to Preferences command (CmdPreferences)
		public enum PrefBits
		{
			// Mortal prefs
			AutoCombat,     // a
			Color,			// b
			Compact,		// c
			Hints,			// d
		};

		// Saved attributes in main players table
		public int Id { get; set; }
		public string Name { get; set; }
		public Genders Sex { get; set; }
		public int Level { get; set; }
		public string Password { get; set; }
		public BitArray Preferences { get; private set; }
		public Color[] ColorPreferences { get; private set; }
		public int LineLength { get; set; }
		public DateTime CreatedAt { get; set; }

		// Runtime attributes
		public Descriptor Descriptor { get; set; }
		public string LastCommand { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public Player ()
			: base()
		{
			Preferences = new BitArray();
			ColorPreferences = new Color[ColorLoc.LocationList.Count];
			CreatedAt = DateTime.Now;
		}

		#region Output functions
		public void Out (string text, params object[] p)
		{
			if (Descriptor != null)
			{
				text = String.Format(text, p);
				Descriptor.AddOutput(text);
			}
		}

		public void OutLn ()
		{
			if (Descriptor != null)
			{
				Descriptor.AddOutputLine();
			}
		}

		public void OutLn (string text, params object[] p)
		{
			if (Descriptor != null)
			{
				text = String.Format(text, p);
				Descriptor.AddOutputLine(text);
			}
		}

		public void Hint (string txt)
		{
			if (Preferences.Get((int) PrefBits.Hints))
			{
				OutLn("* " + txt);
			}
		}

		/// <summary>
		/// Returns the given color code if the player has color turned on. Otherwise empty string.
		/// </summary>
		/// <param name="color">
		/// Color to show.
		/// </param>
		/// <returns>
		/// Color code in text form. <see cref="System.String"/>
		/// </returns>
		public string GetColor(Color color)
		{
			if (Preferences.Get((int) Player.PrefBits.Color))
			{
				return color.Code;
			}

			return "";
		}

		/// <summary>
		/// Returns the current color for the given color pref location if player has color turned on.
		/// </summary>
		/// <param name="location">
		/// Color to show.
		/// </param>
		/// <returns>
		/// Color code in text form. <see cref="System.String"/>
		/// </returns>
		public string GetColorPref(ColorLoc location)
		{
			if (Preferences.Get((int) Player.PrefBits.Color))
			{
				return ColorPreferences[location.Index].Code;
			}

			return "";
		}
		#endregion

		#region Abstract function implementations
		public override bool ChooseNextAction ()
		{
			// Action already chosen
			if (NextAction != FightActions.None)
			{
				return true;
			}

			// Check for autocombat
			if (Preferences.Get((int) PrefBits.AutoCombat))
			{
				List<Living> opponents = InFight.OpposingTeam(this);

				// Hit if all opponents are NPCs
				if (opponents.Count == opponents.FindAll(x => x.IsMonster()).Count)
				{
					NextAction = FightActions.Hit;
					return true;
				}
			}

			return false;
		}

		public override void Die ()
		{
			OutLn("You are dead...");
			OutLn();

			Handler.LivingToRoom(this, World.StartingZone.StartingRoom);
			Commands.CmdLook.LookAtRoom(this);
		}

		public override string GetName()
		{
			return Name;
		}

		public override Genders GetSex()
		{
			return Sex;
		}
		#endregion

		public bool IsFounder()
		{
			return Id == 1;
		}

		/// <summary>
		/// Initializes starting values for a new player.
		/// </summary>
		public void InitializeNewPlayer ()
		{
			// Start at level 1
			Level = 1;

			// Set default preferences

			// No AutoCombat
			Preferences.Set((int) PrefBits.Color);
			// No Compact
			Preferences.Set((int) PrefBits.Hints);

			// Set the default color preferences
			SetDefaultColors();

			// Set default line length
			LineLength = Settings.DefaultLineLength;
		}

		public void SetDefaultColors ()
		{
			for (int i = 0; i < ColorLoc.LocationList.Count; i++)
			{
				ColorPreferences[i] = ColorLoc.LocationList[i].Default;
			}
		}

		public string DbPrefs
		{
			get { return Preferences.GetStringValue(); }
			set { Preferences.SetStringValue(value); }
		}

		public string DbColorPrefs
		{
			get { return Utils.SaveColorPrefs(this); }
			set { Utils.LoadColorPrefs(this, value); }
		}
	}
}
