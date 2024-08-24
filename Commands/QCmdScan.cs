using System;

using QMud.Core;

namespace QMud.Commands
{
	public class QCmdScan : QBaseCommand
	{
		public override void ExecuteCommand (QPlayer player, string[] args, string wholeArg)
		{
			bool hasExits = false;
			
			for (int dir = 0; dir < QSettings.NumExits; dir++)
			{
				if (player.InRoom.Exits[dir] != null)
				{
					if (hasExits == false)
					{
						player.OutLn("You scan the adjacent rooms.");
						hasExits = true;
					}
					
					player.OutLn(QEnumNames.DirNames[dir] + ":");
					QCmdLook.ShowMonstersInRoom(player, player.InRoom.Exits[dir].To);
					QCmdLook.ShowPlayersInRoom(player, player.InRoom.Exits[dir].To);
				}
			}
			
			if (hasExits == false)
			{
				player.OutLn("The room has no obvious exits.");	
			}
		}
		
		public override string GetHelpText ()
		{
			return "List all monsters and players located in adjacent rooms.";
		}
	}
}
