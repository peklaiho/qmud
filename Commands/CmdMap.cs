using System;
using System.Text;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdMap : BaseCommand
	{
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (player.IsBlind())
			{
				player.OutLn("You are blind and cannot see anything.");
				return;
			}

			StringBuilder sb = new StringBuilder(2048);

			int start_x = Math.Max(0, player.InRoom.X - 2);
			int start_y = Math.Max(0, player.InRoom.Y - 2);
			int end_x = Math.Min(player.InRoom.Zone.Template.Width - 1, player.InRoom.X + 2);
			int end_y = Math.Min(player.InRoom.Zone.Template.Height - 1, player.InRoom.Y + 2);

			Room room = null;

			for (int y = start_y; y <= end_y; y++)
			{
				for (int x = start_x; x <= end_x; x++)
				{
					room = player.InRoom.Zone.GetRoom(x, y);

					sb.Append("#");

					if (room.Exits[(int)Directions.North])
					{
						sb.Append("   ");
					}
					else
					{
						sb.Append("---");
					}
				}

				sb.Append("#");
				sb.Append(Settings.LineBreak);

				for (int x = start_x; x <= end_x; x++)
				{
					room = player.InRoom.Zone.GetRoom(x, y);

					if (room.Exits[(int)Directions.West])
					{
						sb.Append(" ");
					}
					else
					{
						sb.Append("|");
					}

					if (room == player.InRoom)
					{
						sb.Append(player.GetColor(Color.BrightWhite));
						sb.Append(" @ ");
						sb.Append(player.GetColor(Color.Reset));
					}
					else if (room.Entrances.Count > 0)
					{
						sb.Append(player.GetColorPref(ColorLoc.RoomEntrances));
						sb.Append(" * ");
						sb.Append(player.GetColor(Color.Reset));
					}
					else
					{
						sb.Append("   ");
					}
				}

				if (room.Exits[(int)Directions.East])
				{
					sb.Append(" ");
				}
				else
				{
					sb.Append("|");
				}

				sb.Append(Settings.LineBreak);
			}

			for (int x = start_x; x <= end_x; x++)
			{
				room = player.InRoom.Zone.GetRoom(x, end_y);

				sb.Append("#");

				if (room.Exits[(int)Directions.South])
				{
					sb.Append("   ");
				}
				else
				{
					sb.Append("---");
				}
			}

			sb.Append("#");
			sb.Append(Settings.LineBreak);

			player.Out(sb.ToString());
		}

		public override string GetHelpText ()
		{
			return "Show a map of the current area.";
		}
	}
}
