using System;

using QMud.Core;
using QMud.services.aonaware.com;

namespace QMud.Commands
{
	public class QCmdDictionary : QBaseCommand
	{
		public enum SubCommands { Dictionary, Thesaurus }

		private SubCommands SubCommand;

		public QCmdDictionary (SubCommands newSubCmd)
		{
			SubCommand = newSubCmd;
		}

		public override void ExecuteCommand (QPlayer player, string[] args, string wholeArg)
		{
			if (args.Length < 2)
			{
				player.OutLn("Which word do you wish to search for?");
			}
			else
			{
				string dictId = "wn";

				if (SubCommand == SubCommands.Thesaurus)
				{
					dictId = "moby-thes";
				}
				else if (args.Length >= 3)
				{
					dictId = args[2];
				}

				try
				{
					DictService svc = new DictService();

					WordDefinition wd = svc.DefineInDict(dictId, args[1]);

					if (wd.Definitions.Length == 0)
					{
						player.OutLn("No definition found.");
					}
					else
					{
						foreach (Definition d in wd.Definitions)
						{
							player.OutLn(d.WordDefinition);
						}
					}
				}
				catch (Exception ex)
				{
					string errorString = "Error while accessing dictionary: " + ex.Message;

					QLog.Error(errorString);
					player.OutLn(errorString);
				}
			}
		}

		public override string GetHelpText ()
		{
			if (SubCommand == SubCommands.Dictionary)
			{
				return "Search for the definition of a word from a dictionary. The default dictionary " +
					"to search from is the WordNet. If you wish to search from the Collaborative " +
					"International Dictionary of English, please give 'gcide' as the dictionary id.";
			}
			else
			{
				return "Search for similar words from a thesaurus.";
			}
		}

		public override string[] GetHelpUsage ()
		{
			if (SubCommand == SubCommands.Dictionary)
			{
				return new string[]
				{
					"<word> [dictionary_id]"
				};
			}
			else
			{
				return new string[]
				{
					"<word>"
				};
			}
		}
	}
}
