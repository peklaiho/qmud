using System;
using System.Linq;
using System.Collections.Generic;

namespace QMud.Core
{
	/// <summary>
	/// The almighty Act function, similar to one found on CircleMud.
	///
	/// Only difference is that we use § instead of $.
	///
	/// For now only supports the following tags:
	/// §n, §N, §e, §E, §s, §S, §m, §M, §o, §O, §t, §T, §p, §P
	/// </summary>
	public class Act
	{
		// Controls to whom the output will be sent
		private enum ActModes
		{
			ToChar,
			ToVict,
			ToRoom,
			ToRoomNotVict,
			ToZone,
			ToZoneNotVict,
			ToParty,
			ToPartyNotVict
		};

		public static void ToChar(string txt, Living ch, Item obj, object vict_obj)
		{
			DoAct(txt, false, ch, obj, vict_obj, ActModes.ToChar);
		}

		public static void ToVict(string txt, bool hideInvis, Living ch, Item obj, object vict_obj)
		{
			DoAct(txt, hideInvis, ch, obj, vict_obj, ActModes.ToVict);
		}

		public static void ToRoom(string txt, bool hideInvis, Living ch, Item obj, object vict_obj)
		{
			DoAct(txt, hideInvis, ch, obj, vict_obj, ActModes.ToRoom);
		}

		public static void ToRoomNotVict(string txt, bool hideInvis, Living ch, Item obj, object vict_obj)
		{
			DoAct(txt, hideInvis, ch, obj, vict_obj, ActModes.ToRoomNotVict);
		}

		public static void ToZone(string txt, bool hideInvis, Living ch, Item obj, object vict_obj)
		{
			DoAct(txt, hideInvis, ch, obj, vict_obj, ActModes.ToZone);
		}

		public static void ToZoneNotVict(string txt, bool hideInvis, Living ch, Item obj, object vict_obj)
		{
			DoAct(txt, hideInvis, ch, obj, vict_obj, ActModes.ToZoneNotVict);
		}

		public static void ToParty(string txt, bool hideInvis, Living ch, Item obj, object vict_obj)
		{
			DoAct(txt, hideInvis, ch, obj, vict_obj, ActModes.ToParty);
		}

		public static void ToPartyNotVict(string txt, bool hideInvis, Living ch, Item obj, object vict_obj)
		{
			DoAct(txt, hideInvis, ch, obj, vict_obj, ActModes.ToPartyNotVict);
		}

		private static void DoAct (string txt, bool hideInvis, Living ch, Item obj, object vict_obj, ActModes actMode)
		{
			try
			{
				// Ch can never be null
				if (ch == null)
				{
					throw new Exception("Ch is null.");
				}

				// Output to char
				if (actMode == Act.ActModes.ToChar)
				{
					// ch can always see themselves, no need to check for CanSee
					PerformAct(txt, ch, obj, vict_obj, ch);
					return;
				}
				// Output to victim
				else if (actMode == Act.ActModes.ToVict)
				{
					// Check that vict_obj is given, and is a living
					if (vict_obj == null || (vict_obj.GetType() != typeof(Player) && vict_obj.GetType() != typeof(Monster)))
					{
						throw new Exception("Vict_obj is null or is not a Living in ActMode ToVict.");
					}

					Living to = (Living) vict_obj;

					if (!hideInvis || to.CanSee(ch))
					{
						PerformAct(txt, ch, obj, vict_obj, to);
					}

					return;
				}

				// If we got this far we know we are going to output to multiple folks!

				// Send to everyone in room
				if (actMode == Act.ActModes.ToRoom || actMode == Act.ActModes.ToRoomNotVict)
				{
					OutputToRoom(txt, hideInvis, ch, obj, vict_obj, actMode, ch.InRoom);
				}
				else if (actMode == ActModes.ToParty || actMode == ActModes.ToPartyNotVict)
				{
					OutputToParty(txt, hideInvis, ch, obj, vict_obj, actMode, ch.InParty);
				}
				// Send to everyone in zone
				else
				{
					// Send to all rooms in the zone
					foreach (Room room in ch.InRoom.Zone.Rooms)
					{
						OutputToRoom(txt, hideInvis, ch, obj, vict_obj, actMode, room);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error in Act: " + ex.Message);
			}
		}

		private static void OutputToRoom(string txt, bool hideInvis, Living ch, Item obj, object vict_obj, ActModes actMode, Room room)
		{
			foreach (Player to in room.Players)
			{
				// Never send to self
				if (to == ch)
				{
					continue;
				}

				// Don't send to victim if that mode is set
				if (to == vict_obj && (actMode == Act.ActModes.ToRoomNotVict || actMode == Act.ActModes.ToZoneNotVict))
				{
					continue;
				}

				// Don't send if output is hidden and can't see
				if (hideInvis && to.CanSee(ch) == false)
				{
					continue;
				}

				// All ok, send it
				PerformAct(txt, ch, obj, vict_obj, to);
			}
		}

		private static void OutputToParty(string txt, bool hideInvis, Living ch, Item obj, object vict_obj, ActModes actMode, Party party)
		{
			foreach (Player to in party.Players)
			{
				// Never send to self
				if (to == ch)
				{
					continue;
				}

				// Don't send to victim if that mode is set
				if (to == vict_obj && actMode == Act.ActModes.ToPartyNotVict)
				{
					continue;
				}

				// Don't send if output is hidden and can't see
				if (hideInvis && to.CanSee(ch) == false)
				{
					continue;
				}

				// All ok, send it
				PerformAct(txt, ch, obj, vict_obj, to);
			}
		}

		private static void PerformAct (string txt, Living ch, Item obj, object vict_obj, Living to)
		{
			try
			{
				// We know that "ch" and "to" will never be null, checked earlier
				// However "obj" and "vict_obj" can be null, so we check them in this function

				// We can only send output to players for now
				if (to.IsMonster())
				{
					return;
				}

				// Replace tags with ch in them
				txt = txt.Replace("§n", to.CanSee(ch) ? ch.GetName() : "someone");
				txt = txt.Replace("§e", ch.HeShe());
				txt = txt.Replace("§s", ch.HisHer());
				txt = txt.Replace("§m", ch.HimHer());

				// Replace object tags
				if (obj == null)
				{
					txt = txt.Replace("§o", "NULL");
					txt = txt.Replace("§t", "NULL");
					txt = txt.Replace("§p", "NULL");
				}
				else
				{
					txt = txt.Replace("§o", to.CanSee(obj) ? AName(obj) : "something");
					txt = txt.Replace("§t", to.CanSee(obj) ? TheName(obj) : "something");
					txt = txt.Replace("§p", to.CanSee(obj) ? obj.Template.Name : "something");
				}

				// Replace vict_obj tags
				if (vict_obj == null)
				{
					txt = txt.Replace("§N", "NULL");
					txt = txt.Replace("§E", "NULL");
					txt = txt.Replace("§S", "NULL");
					txt = txt.Replace("§M", "NULL");
					txt = txt.Replace("§O", "NULL");
					txt = txt.Replace("§T", "NULL");
				}
				else
				{
					if (vict_obj.GetType() == typeof(Player) || vict_obj.GetType() == typeof(Monster))
					{
						Living target = (Living) vict_obj;

						txt = txt.Replace("§N", to.CanSee(target) ? target.GetName() : "someone");
						txt = txt.Replace("§E", target.HeShe());
						txt = txt.Replace("§S", target.HisHer());
						txt = txt.Replace("§M", target.HimHer());
						txt = txt.Replace("§O", "INVALID");
						txt = txt.Replace("§T", "INVALID");
						txt = txt.Replace("§P", "INVALID");
					}
					else if (vict_obj.GetType() == typeof(Item))
					{
						Item item = (Item) vict_obj;

						txt = txt.Replace("§N", "INVALID");
						txt = txt.Replace("§E", "INVALID");
						txt = txt.Replace("§S", "INVALID");
						txt = txt.Replace("§M", "INVALID");
						txt = txt.Replace("§O", to.CanSee(item) ? AName(item) : "something");
						txt = txt.Replace("§T", to.CanSee(item) ? TheName(item) : "something");
						txt = txt.Replace("§P", to.CanSee(item) ? item.Template.Name : "something");
					}
					else
					{
						throw new Exception("Vict_obj is not a Living, and not an Item.");
					}
				}

				// Check that the string doesn't have any § characters left
				if (txt.IndexOf("§") >= 0)
				{
					throw new Exception("All tags were not replaced.");
				}

				// Get player by cast
				Player player = (Player) to;

				// Capitalize the string and send it to player's output queue
				player.OutLn(Utils.CapitalizeString(txt));
			}
			catch (Exception ex)
			{
				Log.Error("Error in Act: " + ex.Message);
			}
		}

		// Returns the items default article in front of it's name
		private static string AName(Item item)
		{
			if (item.Template.Name.Length == 0 || item.Template.Article == ItemTemplate.Articles.None)
			{
				return item.Template.Name;
			}

			if (item.Template.Article == ItemTemplate.Articles.A)
			{
				return "a " + item.Template.Name;
			}
			else if (item.Template.Article == ItemTemplate.Articles.An)
			{
				return "an " + item.Template.Name;
			}
			else
			{
				return "the " + item.Template.Name;
			}
		}

		// Same as above, but converts 'a' and 'an' to 'the'
		private static string TheName(Item item)
		{
			if (item.Template.Name.Length == 0 || item.Template.Article == ItemTemplate.Articles.None)
			{
				return item.Template.Name;
			}

			return "the " + item.Template.Name;
		}
	}
}
