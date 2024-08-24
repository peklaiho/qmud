using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;
using QMud.Database;

namespace QMud.Commands
{
	public class QCmdRoomEdit : QBaseCommand
	{
		private static QDirections[] OppositeDir =
		{
			QDirections.South,
			QDirections.West,
			QDirections.North,
			QDirections.East,
			QDirections.Down,
			QDirections.Up
		};

		private static void CreateExits(QRoom room1, QRoom room2, QDirections dir, out QExit exit1, out QExit exit2)
		{
			exit1 = new QExit() { FromId = room1.Id, ToId = room2.Id, Direction = dir, To = room2 };
			exit2 = new QExit() { FromId = room2.Id, ToId = room1.Id, Direction = OppositeDir[(int)dir], To = room1 };
		}

		private static void CreateNewExit(QPlayer player, QDirections dir)
		{
			QExit oldExit = player.InRoom.Exits[(int) dir];
			string dirName = QUtils.LowerCase(QEnumNames.DirNames[(int) dir]);

			if (oldExit != null)
			{
				player.OutLn("This room already has exit to " + dirName + ":");
				player.OutLn(oldExit.To.Id + ": " + oldExit.To.Name);
				return;
			}

			QCoords targetCoordinates = player.InRoom.Coords.Step(dir);
			QRoom room = player.InRoom.Zone.RoomAtCoords(targetCoordinates);

			if (room == null)
			{
				player.OutLn("This zone has no existing room " + dirName + " of here.");
				return;
			}

			oldExit = room.Exits[(int)OppositeDir[(int)dir]];

			if (oldExit != null)
			{
				player.OutLn("The target room already has exit to " + QUtils.LowerCase(QEnumNames.DirNames[(int)oldExit.Direction]) + ":");
				player.OutLn(oldExit.To.Id + ": " + oldExit.To.Name);
				return;
			}

			// Everything looks ok, lets create new exits.
			QExit exit1, exit2;
			CreateExits(player.InRoom, room, dir, out exit1, out exit2);

			if (QDatabaseWorld.AddNewRoom(null, exit1, exit2))
			{
				player.InRoom.Exits[(int)dir] = exit1;
				room.Exits[(int)OppositeDir[(int)dir]] = exit2;
				player.OutLn("New exit created successfully.");
				QLog.GodCmd(player.Name + " created exit from room " + player.InRoom.Id + " to room " + room.Id + ".");
			}
			else
			{
				player.OutLn("Error occured while saving exits to database.");
			}
		}

		private static void CreateNewRoom(QPlayer player, QDirections dir)
		{
			QExit oldExit = player.InRoom.Exits[(int) dir];
			string dirName = QUtils.LowerCase(QEnumNames.DirNames[(int) dir]);

			if (oldExit != null)
			{
				player.OutLn("This room already has exit to " + dirName + ":");
				player.OutLn(oldExit.To.Id + ": " + oldExit.To.Name);
				return;
			}

			QCoords targetCoordinates = player.InRoom.Coords.Step(dir);
			QRoom room = player.InRoom.Zone.RoomAtCoords(targetCoordinates);

			if (room != null)
			{
				player.OutLn("This zone already has a room " + dirName + " of here:");
				player.OutLn(room.Id + ": " + room.Name);
				return;
			}

			// Ok, we can create a new room
			room = new QRoom()
			{
				Id = QWorld.NextFreeRoomId(player.InRoom.Zone.Id * 100),
				ZoneId = player.InRoom.Zone.Id,
				Coords = targetCoordinates,
				Terrain = player.InRoom.Terrain,
				Name = "New Room",
				DescBegin = "",
				DescEnd = "",

				Zone = player.InRoom.Zone
			};

			// Create exits too
			QExit exit1, exit2;
			CreateExits(player.InRoom, room, dir, out exit1, out exit2);

			if (QDatabaseWorld.AddNewRoom(room, exit1, exit2))
			{
				// Link them up!
				QWorld.Rooms.Add(room.Id, room);
				player.InRoom.Zone.Rooms.Add(room.Coords, room);
				player.InRoom.Exits[(int)dir] = exit1;
				room.Exits[(int)OppositeDir[(int)dir]] = exit2;
				player.OutLn("New room created successfully.");
				QLog.GodCmd(player.Name + " created a new room " + room.Id + " and connected it with room " + player.InRoom.Id + ".");
			}
			else
			{
				player.OutLn("Error occured while saving room to database.");
			}
		}

		public override void ExecuteCommand (QPlayer player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				QCmdHelp.ShowHelp(player, this);
				return;
			}

			if (QUtils.StartsWith("exit", args[1]))
			{
				if (args.Length < 3)
				{
					player.OutLn("To which direction do you wish to create a new exit?");
				}
				else
				{
					for (int i = 0; i < QEnumNames.DirNames.Length; i++)
					{
						if (QUtils.StartsWith(QEnumNames.DirNames[i], args[2]))
						{
							CreateNewExit(player, (QDirections)i);
							return;
						}
					}

					player.OutLn("Unknown direction.");
				}
			}
			else if (QUtils.StartsWith("name", args[1]))
			{
				if (args.Length < 3)
				{
					player.OutLn("Current room name: " + player.InRoom.Name);
				}
				else
				{
					string newName = wholeArg.Substring(args[1].Length).TrimStart();
					player.InRoom.Name = newName;
					player.OutLn("Room name set to: " + newName);
					QLog.GodCmd(player.Name + " changed name of room " + player.InRoom.Id + " to '" + newName + "'.");

					if (player.Preferences.Get((int) QPlayer.PrefBits.OlcAutoSave))
					{
						QDatabase.Update(player.InRoom);
						QLog.GodCmd(player.Name + " saved room " + player.InRoom.Id + ".");
					}
				}
			}
			else if (QUtils.StartsWith("new", args[1]))
			{
				if (args.Length < 3)
				{
					player.OutLn("To which direction do you wish to create a new room?");
				}
				else
				{
					for (int i = 0; i < QEnumNames.DirNames.Length; i++)
					{
						if (QUtils.StartsWith(QEnumNames.DirNames[i], args[2]))
						{
							CreateNewRoom(player, (QDirections)i);
							return;
						}
					}

					player.OutLn("Unknown direction.");
				}
			}
			else if (QUtils.StartsWith("save", args[1]))
			{
				QDatabase.Update(player.InRoom);
				player.OutLn("Room saved.");
				QLog.GodCmd(player.Name + " saved room " + player.InRoom.Id + ".");
			}
			else if (QUtils.StartsWith("terrain", args[1]))
			{
				if (args.Length < 3)
				{
					player.OutLn("Current terrain: " + QEnumNames.TerrainNames[(int) player.InRoom.Terrain]);
					player.OutLn("Possible choices: " + String.Join(", ", QEnumNames.TerrainNames));
				}
				else
				{
					for (int i = 0; i < QEnumNames.TerrainNames.Length; i++)
					{
						if (QUtils.StartsWith(QEnumNames.TerrainNames[i], args[2]))
						{
							player.InRoom.Terrain = (QTerrains) i;
							player.OutLn("Terrain set to: " + QEnumNames.TerrainNames[i]);
							QLog.GodCmd(player.Name + " changed terrain of room " + player.InRoom.Id + " to " + QEnumNames.TerrainNames[i] + ".");

							if (player.Preferences.Get((int) QPlayer.PrefBits.OlcAutoSave))
							{
								QDatabase.Update(player.InRoom);
								QLog.GodCmd(player.Name + " saved room " + player.InRoom.Id + ".");
							}
							return;
						}
					}

					player.OutLn("Unknown terrain.");
				}
			}
			else
			{
				player.OutLn("Unknown command. What do you wish to do with redit?");
			}
		}

		public override string GetHelpText ()
		{
			return "Modify the attributes of the current room, or create a new room.";
		}

		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<'name'> <name>",
				"<'terrain'> <terrain>",
				"<'save'>",
				"<'new'> <direction>",
				"<'exit'> <direction>",
			};
		}
	}
}
