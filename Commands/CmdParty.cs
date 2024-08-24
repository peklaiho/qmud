using System;
using System.Linq;
using System.Collections.Generic;

using QMud.Core;

namespace QMud.Commands
{
	public class CmdParty : BaseCommand
	{
		public override void ExecuteCommand(Player player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("What do you want to do?");
				return;
			}

			Party party = null;
			int id;

			if (Utils.StartsWith("list", args[1]))
			{
				player.OutLn("Current parties:");

				foreach (Party p in Party.PartyList)
				{
					player.OutLn(p.Id + " - " + p.Name);
				}
			}
			else if (Utils.StartsWith("info", args[1]))
			{
				if (args.Length < 3)
				{
					if (player.InParty == null)
					{
						player.OutLn("You are not in a party.");
						return;
					}

					party = player.InParty;
				}
				else
				{
					if (!int.TryParse(args[2], out id))
					{
						player.OutLn("Invalid party id.");
						return;
					}

					party = Party.FindById(id);

					if (party == null)
					{
						player.OutLn("Party with id " + id + " does not exist.");
						return;
					}
				}

				player.OutLn("Id: " + party.Id);
				player.OutLn("Name: " + party.Name);

				List<Player> visiblePlayers = party.Players.FindAll(x => player.CanSee(x));
				List<Monster> visibleMonsters = party.Monsters.FindAll(x => player.CanSee(x));

				if (visiblePlayers.Count > 0)
				{
					player.OutLn("Players:");
					visiblePlayers.ForEach(x => player.OutLn(" " + x.GetName()));
				}

				if (visibleMonsters.Count > 0)
				{
					player.OutLn("NPCs:");
					visibleMonsters.ForEach(x => player.OutLn(" " + x.GetName()));
				}
			}
			else if (Utils.StartsWith("join", args[1]))
			{
				if (player.InParty != null)
				{
					player.OutLn("You are already in a party.");
					return;
				}
				else if (player.InFight != null)
				{
					player.OutLn("You cannot join a party while fighting.");
					return;
				}
				else if (args.Length < 3)
				{
					player.OutLn("Join which party?");
					return;
				}

				if (!int.TryParse(args[2], out id))
				{
					player.OutLn("Invalid party id.");
					return;
				}

				party = Party.FindById(id);

				if (party == null)
				{
					player.OutLn("Party with id " + id + " does not exist.");
					return;
				}

				Handler.LivingToParty(player, party);

				player.OutLn("You have joined party '" + party.Name + "'.");
				Act.ToRoom("§n has joined party '" + party.Name + "'.", true, player, null, null);
				Act.ToParty("§n has joined your party.", true, player, null, null);

				Log.Info("{0} has joined party #{1} '{2}'.", player.GetName(), party.Id, party.Name);
			}
			else if (Utils.StartsWith("create", args[1]))
			{
				if (player.InParty != null)
				{
					player.OutLn("You are already in a party.");
					return;
				}
				else if (player.InFight != null)
				{
					player.OutLn("You cannot create a party while fighting.");
					return;
				}
				else if (args.Length < 3)
				{
					player.OutLn("Please enter a name for your party.");
					return;
				}

				string name = Utils.CapitalizeString(String.Join(" ", args, 2, args.Length - 2));

				if (!IsValidPartyName(name))
				{
					player.OutLn("Party name is not valid. Try again.");
					return;
				}

				party = new Party(name);
				Handler.LivingToParty(player, party);

				player.OutLn("You have created a new party named '" + party.Name + "'.");
				player.OutLn("Other players can join your party by typing 'party join " + party.Id + "'.");
				Act.ToRoom("§n has created a new party named '" + party.Name + "'.", true, player, null, null);

				Log.Info("{0} has created new party #{1} '{2}'.", player.GetName(), party.Id, party.Name);
			}
			else if (Utils.StartsWith("leave", args[1]))
			{
				if (player.InParty == null)
				{
					player.OutLn("You are not in a party.");
					return;
				}
				else if (player.InFight != null)
				{
					player.OutLn("You cannot leave a party while fighting.");
					return;
				}

				player.OutLn("You have left your party.");
				Act.ToRoom("§n has left §s party.", true, player, null, null);
				Act.ToParty("§n has left your party.", true, player, null, null);

				Log.Info("{0} has left party #{1} '{2}'.", player.GetName(), player.InParty.Id, player.InParty.Name);

				Handler.LivingFromParty(player);
			}
			else
			{
				player.OutLn("Unknown party command.");
			}
		}

		public override string GetHelpText()
		{
			return "This command allows you to interact with parties. You can list current parties, look " +
				"up info for a party, join a party, create a new party or leave your current party.";
		}

		public override string[] GetHelpUsage()
		{
			return new string[]
			{
				"<'list'>",
				"<'info'> [id]",
				"<'join'> <id>",
				"<'create'> <name>",
				"<'leave'>"
			};
		}

		private bool IsValidPartyName(string name)
		{
			return true;
		}
	}
}
