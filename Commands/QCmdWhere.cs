using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class QCmdWhere : QBaseCommand
	{
		public override void ExecuteCommand (QPlayer player, string[] args, string wholeArg)
		{
			for (int i = 0; i < QEnumNames.DirNames.Length; i++)
			{
				QExit byExit = player.InRoom.Exits[i];
				QRoom byCoords = player.InRoom.Next((QDirections)i);

				if (byExit != null)
				{
					player.OutLn(QEnumNames.DirNames[i] + ":");
					player.OutLn(String.Format("[{0,4}] {1} (by exit)", byExit.To.Id, byExit.To.Name), false);
				}

				if (byCoords != null && (byExit == null || byCoords != byExit.To))
				{
					if (byExit == null)
					{
						player.OutLn(QEnumNames.DirNames[i] + ":");
					}
					player.OutLn(String.Format("[{0,4}] {1} (by coordinates)", byCoords.Id, byCoords.Name), false);
				}
			}
		}

		public override string GetHelpText ()
		{
			return "Show nearby rooms by exits and by coordinates.";
		}
	}
}
