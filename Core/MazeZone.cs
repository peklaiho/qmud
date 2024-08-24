using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class MazeZone : Zone
	{
		private readonly int[] oppositeDir =
		{
			2,
			3,
			0,
			1
		};

		private List<int> TempList = new List<int>(4);

		public MazeZone()
		{

		}

		public override void Initialize ()
		{
			Stack<Room> stack = new Stack<Room>();
			bool[,] visited = new bool[Template.Width, Template.Height];
			Room room = GetRandomRoom();

			while (true)
			{
				visited[room.X, room.Y] = true;

				int dir = RandomAdjacentUnvisitedDir(room, visited);

				if (dir == -1)
				{
					// Pop previous room or break
					if (stack.Count > 0)
					{
						room = stack.Pop();
					}
					else
					{
						break;
					}
				}
				else
				{
					Room next = room.AdjacentRoom((Directions)dir);

					room.Exits[dir] = true;
					next.Exits[oppositeDir[dir]] = true;

					stack.Push(room);

					room = next;
				}
			}

			// Select a random starting room
			StartingRoom = GetRandomRoom();
		}
			
		private int RandomAdjacentUnvisitedDir(Room room, bool[,] visited)
		{
			TempList.Clear();

			if (room.Y > 0 && !visited[room.X, room.Y - 1])
			{
				TempList.Add(0);
			}
			if (room.X < Template.Width - 1 && !visited[room.X + 1, room.Y])
			{
				TempList.Add(1);
			}
			if (room.Y < Template.Height - 1 && !visited[room.X, room.Y + 1])
			{
				TempList.Add(2);
			}
			if (room.X > 0 && !visited[room.X - 1, room.Y])
			{
				TempList.Add(3);
			}

			if (TempList.Count > 0)
			{
				return TempList[Random.Range(0, TempList.Count - 1)];
			}

			return -1;
		}
	}
}
