using System;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdComm : BaseCommand
	{
		public enum SubCommands { Gossip, Say, Shout, Trade, Wiznet };
		
		private static string[] ActionNames = new string[] { "gossip", "say", "shout", "trade", "wiznet" };
		
		private SubCommands SubCommand;
		
		public CmdComm (SubCommands newSubCmd)
		{
			SubCommand = newSubCmd;
		}
		
		public override void ExecuteCommand (Player player, string[] args, string wholeArg)
		{
			if (args.Length == 1)
			{
				player.OutLn("What do you wish to " + ActionNames[(int) SubCommand] + "?");
				return;
			}
			
			// Colors
			ColorLoc[] CommColorLocations = new ColorLoc[]
			{
				ColorLoc.Gossip,
				ColorLoc.Say,
				ColorLoc.Shout,
				ColorLoc.Trade,
				ColorLoc.Wiznet
			};
			
			// Perform communication
			string msgToSelf, msgToOther, msgToOtherHidden;
			
			if (SubCommand == CmdComm.SubCommands.Wiznet)
			{
				msgToSelf = "You: " + wholeArg;
				msgToOther = player.Name + ": " + wholeArg;
				msgToOtherHidden = "Someone: " + wholeArg;
			}
			else
			{
				string actionName = ActionNames[(int) SubCommand];

				if (SubCommand == SubCommands.Say)
				{
					if (wholeArg.EndsWith("!"))
					{
						actionName = "exclaim";
					}
					else if (wholeArg.EndsWith("?"))
					{
						actionName = "ask";
					}
				}

				msgToSelf = "You " + actionName + ", '" + wholeArg + "'.";
				msgToOther = player.Name + " " + actionName + "s, '" + wholeArg + "'.";
				msgToOtherHidden = "Someone " + actionName + "s, '" + wholeArg + "'.";
			}
			
			player.OutLn(player.GetColorPref(CommColorLocations[(int) SubCommand]) + msgToSelf + player.GetColor(Color.Reset));
			
			foreach (Player other in World.Players)
			{
				// Don't send to self
				if (other == player)
				{
					continue;
				}
				
				// Wiznet is only for immortals
				if (SubCommand == CmdComm.SubCommands.Wiznet && other.Level < Player.LvlImmort)
				{
					continue;
				}
				
				// Say only if both are in same room
				if (SubCommand == CmdComm.SubCommands.Say && player.InRoom != other.InRoom)
				{
					continue;
				}
				
				// Shout only if both are in same zone
				if (SubCommand == CmdComm.SubCommands.Shout && player.InRoom.Zone != other.InRoom.Zone)
				{
					continue;
				}
				
				if (other.CanSee(player))
				{
					other.OutLn(other.GetColorPref(CommColorLocations[(int) SubCommand]) + msgToOther + other.GetColor(Color.Reset));
				}
				else
				{
					other.OutLn(other.GetColorPref(CommColorLocations[(int) SubCommand]) + msgToOtherHidden + other.GetColor(Color.Reset));
				}
			}
		}
		
		public override string GetHelpText ()
		{
			switch (SubCommand)
			{
				case SubCommands.Gossip:
					return "Gossip channel is used for chatting with all players.";
				case SubCommands.Say:
					return "Say is used to communicate with all players who are in the same room as yourself.";
				case SubCommands.Shout:
					return "Shout is used to communicate with all players who are in the same area as yourself.";
				case SubCommands.Trade:
					return "Trade channel is used for trading items between players. Please don't use it for other purposes.";
				case SubCommands.Wiznet:
					return "Wiznet channel is used for communication between immortals.";
			}
			
			return "";
		}
		
		public override string GetHelpShortcut ()
		{
			if (SubCommand == CmdComm.SubCommands.Wiznet && GetName() != "wiznet")
			{
				return "wiznet";
			}
			else if (SubCommand == CmdComm.SubCommands.Say && GetName() != "say")
			{
				return "say";
			}
			else if (SubCommand == SubCommands.Gossip && GetName() != "gossip")
			{
				return "gossip";
			}
			
			return base.GetHelpShortcut();
		}
		
		public override string[] GetHelpUsage ()
		{
			return new string[]
			{
				"<message>"
			};
		}
	}
}
