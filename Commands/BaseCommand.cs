using System;

using QMud.Core;

namespace QMud.Commands
{
	public abstract class BaseCommand
	{
		private string name;
		private int minLevel;
		
		public void Init(string newName, int newMinLevel)
		{
			this.name = newName;
			this.minLevel = newMinLevel;
		}

		public bool CanExecute (Player player)
		{
			return player.Level >= minLevel;
		}
		
		public abstract void ExecuteCommand (Player player, string[] args, string wholeArg);
		
		public abstract string GetHelpText ();
		
		public virtual string GetHelpShortcut ()
		{
			return null;
		}
		
		public virtual string[] GetHelpUsage ()
		{
			return null;
		}
		
		public virtual string GetName ()
		{
			return name;
		}

		public bool IsImmortalCommand ()
		{
			return minLevel >= Player.LvlImmort;
		}
	}
}
