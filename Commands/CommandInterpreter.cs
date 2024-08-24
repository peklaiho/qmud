using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using QMud.Core;

namespace QMud.Commands
{
	public class CommandInterpreter
	{
		public static List<BaseCommand> CommandList = new List<BaseCommand>();

		// Regular expression used to split input
		private static Regex RegPattern = new Regex("\\s+");

		/// <summary>
		/// Initializes our command list.
		/// </summary>
		public static void InitializeCommands ()
		{
			Log.Info("Initializing commands.");

			// List is NOT alphabetically ordered, but instead ordered by importance!
			// Immortal commanda last in order.

			// Shortcuts
			DefineCommand("!", new CmdRepeat());
			DefineCommand("'", new CmdComm(CmdComm.SubCommands.Say));
			DefineCommand(":", new CmdComm(CmdComm.SubCommands.Gossip));
			DefineCommand(";", new CmdComm(CmdComm.SubCommands.Wiznet), Player.LvlImmort);

			// Movement
			DefineCommand("north", new CmdMove(Directions.North));
			DefineCommand("east", new CmdMove(Directions.East));
			DefineCommand("south", new CmdMove(Directions.South));
			DefineCommand("west", new CmdMove(Directions.West));

			// Normal commands
			DefineCommand("cancel", new CmdCancel());
			DefineCommand("commands", new CmdCommands());
			DefineCommand("colorpref", new CmdColorPref());

			DefineCommand("discar", new CmdDiscard(CmdDiscard.SubCommands.Discar));
			DefineCommand("discard", new CmdDiscard(CmdDiscard.SubCommands.Discard));
			DefineCommand("drop", new CmdDrop());

			DefineCommand("enter", new CmdEnter());
			DefineCommand("equipment", new CmdEquipment());
			DefineCommand("explore", new CmdExplore());

			DefineCommand("fight", new CmdFight());
			DefineCommand("flee", new CmdFlee());

			DefineCommand("get", new CmdGet());
			DefineCommand("gossip", new CmdComm(CmdComm.SubCommands.Gossip));
			DefineCommand("give", new CmdGive());
			DefineCommand("goto", new CmdGoto(), Player.LvlImmort);

			DefineCommand("health", new CmdHealth());
			DefineCommand("help", new CmdHelp());
			DefineCommand("hit", new CmdHit());

			DefineCommand("inventory", new CmdInventory());

			DefineCommand("look", new CmdLook());
			DefineCommand("load", new CmdLoad(), Player.LvlDemiGod);

			DefineCommand("map", new CmdMap());
			DefineCommand("memory", new CmdMemory(), Player.LvlImmort);

			DefineCommand("party", new CmdParty());
			DefineCommand("put", new CmdPut());
			DefineCommand("preferences", new CmdPreferences());

			DefineCommand("qui", new CmdQuit(CmdQuit.SubCommands.Qui));
			DefineCommand("quit", new CmdQuit(CmdQuit.SubCommands.Quit));

			DefineCommand("remove", new CmdRemove());

			DefineCommand("say", new CmdComm(CmdComm.SubCommands.Say));
			DefineCommand("shout", new CmdComm(CmdComm.SubCommands.Shout));
			DefineCommand("status", new CmdStatus());
			DefineCommand("save", new CmdSave(), Player.LvlDemiGod);
			DefineCommand("search", new CmdSearch(), Player.LvlImmort);
			DefineCommand("shutdow", new CmdShutdown(CmdShutdown.SubCommands.Shutdow), Player.LvlGrGod);
			DefineCommand("shutdown", new CmdShutdown(CmdShutdown.SubCommands.Shutdown), Player.LvlGrGod);

			DefineCommand("trade", new CmdComm(CmdComm.SubCommands.Trade));

			DefineCommand("uptime", new CmdUptime(), Player.LvlImmort);

			DefineCommand("version", new CmdVersion());
			DefineCommand("viewlog", new CmdViewLog(), Player.LvlGod);

			DefineCommand("wear", new CmdWear(CmdWear.SubCommands.Wear));
			DefineCommand("who", new CmdWho());
			DefineCommand("whoami", new CmdWhoAmI());
			DefineCommand("wield", new CmdWear(CmdWear.SubCommands.Wield));
			DefineCommand("wiznet", new CmdComm(CmdComm.SubCommands.Wiznet), Player.LvlImmort);

			Log.Info("{0} commands in game.", CommandList.Count);
		}

		/// <summary>
		/// Find the correct command for the input, and execute it.
		/// </summary>
		/// <param name="player">
		/// Player who is executing the command. <see cref="QPlayer"/>
		/// </param>
		/// <param name="input">
		/// Input that the player typed. <see cref="System.String"/>
		/// </param>
		public static void InterpretCommand (Player player, string input)
		{
			string[] args;
			string wholeArg;

			// Trim input
			input = input.Trim();

			// If we got empty string, just show prompt
			if (input.Length == 0)
			{
				return;
			}

			// Split the input into arguments
			args = RegPattern.Split(input);

			// Some commands need the whole argument line as one
			wholeArg = input.Substring(args[0].Length).TrimStart();

			// Find correct command
			foreach (BaseCommand cmd in CommandList)
			{
				if (cmd.CanExecute(player) && Utils.StartsWith(cmd.GetName(), args[0]))
				{
					if (cmd.GetType() != typeof(CmdRepeat))
					{
						player.LastCommand = input;
					}

					cmd.ExecuteCommand(player, args, wholeArg);
					return;
				}
			}

			// No command found
			player.OutLn("Unknown command, try again.");
		}

		private static void DefineCommand(string name, BaseCommand cmd)
		{
			DefineCommand(name, cmd, 1);
		}

		private static void DefineCommand(string name, BaseCommand cmd, int minLevel)
		{
			cmd.Init(name, minLevel);
			CommandList.Add(cmd);
		}
	}
}
