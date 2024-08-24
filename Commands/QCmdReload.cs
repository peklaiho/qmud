using System;
using System.Collections.Generic;

using QMud.Core;
using QMud.Database;

namespace QMud.Commands
{
	public class QCmdReload : QBaseCommand
	{
		public enum SubCommands { Reload, Reloa };

		private SubCommands SubCommand;

		public QCmdReload (SubCommands newSubCmd)
		{
			SubCommand = newSubCmd;
		}

		public override void ExecuteCommand (QPlayer player, string[] args, string wholeArg)
		{
			if (SubCommand == SubCommands.Reloa)
			{
				player.OutLn("Please type the whole command if you wish to reload the world.");
				return;
			}

			// Check that starting room exists
			if (QDatabaseWorld.CheckStartingRoom() == false)
			{
				player.OutLn("Starting room does not exist in the database.");
				return;
			}

			// Log it!
			QLog.GodCmd("Database reload by " + player.Name + ".");

			// Start by freeing instances of any existing items
			while (QWorld.Items.Count > 0)
			{
				QHandler.ExtractItem(QWorld.Items[0]);
			}
			while (QWorld.Monsters.Count > 0)
			{
				QHandler.ExtractLiving(QWorld.Monsters[0]);
			}

			// Save their room ID's and try to restore them into their original rooms
			Dictionary<int, int> oldRooms = new Dictionary<int, int>();

			// Remove all players from rooms
			foreach (QPlayer plr in QWorld.Players)
			{
				oldRooms.Add(plr.Id, plr.InRoom.Id);
				QHandler.LivingFromRoom(plr);
			}

			// Free any existing templates
			QWorld.ClearMemory();

			// Load from database (closes database connection also)
			QDatabase.LoadWorld();

			// Return all players to starting room
			foreach (QPlayer plr in QWorld.Players)
			{
				// Try to put the player in their old room first
				QRoom targetRoom = QWorld.FindRoomById(oldRooms[plr.Id]);

				// If not found, put him in the starting room
				if (targetRoom == null)
				{
					// Starting room should be found, since we checked it earlier
					targetRoom = QWorld.FindRoomById(QSettings.StartingRoomId);
				}

				QHandler.LivingToRoom(plr, targetRoom);
				QCmdLook.LookAtRoom(plr, true);
			}

			player.OutLn();
			player.OutLn("Reloading complete.");
			player.OutLn("Please check with the 'viewlog' command if any problems occured.");
		}

		public override string GetHelpText ()
		{
			if (SubCommand == SubCommands.Reload)
			{
				return "Reload the world (rooms, items, monsters, etc.) from the database. It is not " +
					"possible to recover from an error during the reloading process. If an error " +
					"occurs, the game will shut down. Additionally all currently loaded instances " +
					"of items and monsters will be removed.";
			}
			else
			{
				return "Safety command which ensures that you must type 'reload' completely.";
			}
		}
	}
}
