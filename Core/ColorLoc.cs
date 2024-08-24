using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class ColorLoc
	{
		public static ColorLoc Gossip { get; private set; }
		public static ColorLoc Highlight { get; private set; }
		public static ColorLoc RoomEntrances { get; private set; }
		public static ColorLoc RoomExits { get; private set; }
		public static ColorLoc RoomItems { get; private set; }
		public static ColorLoc RoomMobs { get; private set; }
		public static ColorLoc RoomName { get; private set; }
		public static ColorLoc RoomPlayers { get; private set; }
		public static ColorLoc Say { get; private set; }
		public static ColorLoc Shout { get; private set; }
		public static ColorLoc Tell { get; private set; }
		public static ColorLoc Trade { get; private set; }
		public static ColorLoc Wiznet { get; private set; }

		public static List<ColorLoc> LocationList;

		public int Index { get; private set; }
		public string Code { get; private set; }
		public Color Default { get; private set; }
		public string Description { get; private set; }
		public bool ImmortalOnly { get; private set; }

		public ColorLoc (string newCode, Color newDefault, string newDesc)
		{
			Code = newCode;
			Default = newDefault;
			Description = newDesc;
			ImmortalOnly = false;
		}

		public ColorLoc (string newCode, Color newDefault, string newDesc, bool newImmOnly)
		{
			Code = newCode;
			Default = newDefault;
			Description = newDesc;
			ImmortalOnly = newImmOnly;
		}

		public static void InitializeColorLocations ()
		{
			Log.Info("Initializing color locations.");

			LocationList = new List<ColorLoc>();

			// Default color setting loosely based on the legendary Age of Insanity MUD
			LocationList.Add((RoomName = new ColorLoc("areas", Color.Green, "Area names")));
			LocationList.Add((RoomEntrances = new ColorLoc("entrances", Color.Red, "Entrances to other areas")));
			LocationList.Add((RoomExits = new ColorLoc("exits", Color.Yellow, "Obvious exits in rooms")));
			LocationList.Add((Gossip = new ColorLoc("gossip", Color.Reset, "Messages on the gossip channel")));
			LocationList.Add((Highlight = new ColorLoc("highlight", Color.Yellow, "General highlight color")));
			LocationList.Add((RoomItems = new ColorLoc("items", Color.Cyan, "Items in rooms")));
			LocationList.Add((RoomMobs = new ColorLoc("npcs", Color.Magenta, "NPCs and monsters in rooms")));
			LocationList.Add((RoomPlayers = new ColorLoc("players", Color.Magenta, "Players in rooms")));
			LocationList.Add((Say = new ColorLoc("say", Color.Reset, "Messages spoken by players")));
			LocationList.Add((Shout = new ColorLoc("shout", Color.Reset, "Messages shouted by players")));
			LocationList.Add((Tell = new ColorLoc("tell", Color.Red, "Direct player to player messages")));
			LocationList.Add((Trade = new ColorLoc("trade", Color.Reset, "Messages on the trade channel")));
			LocationList.Add((Wiznet = new ColorLoc("wiznet", Color.Cyan, "Messages on the wiznet channel", true)));

			// Set indexes
			for (int i = 0; i < LocationList.Count; i++)
			{
				LocationList[i].Index = i;
			}
		}
	}
}
