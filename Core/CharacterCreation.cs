using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using QMud.Commands;
using QMud.Database;

namespace QMud.Core
{
	public class CharacterCreation
	{
		// For password encryption
		private static SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

		/// <summary>
		/// Handles input during character creation process.
		/// </summary>
		/// <param name="desc">
		/// The descriptor which is sending the input. <see cref="QDescriptor"/>
		/// </param>
		/// <param name="input">
		/// The input the client typed. <see cref="System.String"/>
		/// </param>
		public static void HandleCharacterCreation (Descriptor desc, string input)
		{
			// Trim our input
			input = input.Trim();

			// Handle character creation in a nice little State Machine
			switch (desc.ConnectionState)
			{
				// New connections start here: Ask character name
				case Descriptor.ConnStates.AskName:
				{
					// Disconnect player if we received empty string as input
					if (input.Length == 0)
					{
						desc.Disconnect = true;
						return;
					}

					// Capitalize the name
					input = Utils.CapitalizeName(input);

					// Check that the name is valid
					if (IsValidPlayerName(input) == false)
					{
						desc.AddOutput("Invalid name. What is your name? ");
					}
					else
					{
						// Try to find the player first from currently loaded players
						desc.Player = World.FindPlayerByName(input, null);

						// If we found him, we have either a) linkless player or b) player connected to another descriptor
						if (desc.Player != null)
						{
							desc.AddOutput("Password: ");
							desc.ConnectionState = Descriptor.ConnStates.AskPassword;
						}
						else
						{
							// Try to find the player from database
							desc.Player = DatabasePlayer.FindByName(input);

							// If we found him, ask the password like above
							if (desc.Player != null)
							{
								desc.AddOutput("Password: ");
								desc.ConnectionState = Descriptor.ConnStates.AskPassword;
							}
							// If we still don't have him, it's a new player
							else
							{
								desc.Player = new Player() { Name = input };

								desc.AddOutput("Are you sure you wish to be known as " + input + "? ");
								desc.ConnectionState = Descriptor.ConnStates.VerifyName;
							}
						}
					}

					break;
				}

				// Descriptor.Player should always be set if we get past AskName state!
				// No need to check for null in every step.

				// Ask the password for an existing character
				case Descriptor.ConnStates.AskPassword:
				{
					// Disconnect player if we received empty string as input
					if (input.Length == 0)
					{
						desc.Disconnect = true;
						return;
					}

					// Passwords match
					if (desc.Player.Password == EncryptPassword(input))
					{
						// Player is already in game
						if (World.Players.Contains(desc.Player))
						{
							// Player is linkless, connect him
							if (desc.Player.Descriptor == null)
							{
								// Attach and display a message to the player
								desc.Player.Descriptor = desc;

								desc.AddOutputLine();
								desc.AddOutputLine("You have reconnected.");
								desc.AddOutputLine();
								CmdLook.LookAtRoom(desc.Player);

								// Log it
								Log.Info("{0} has reconnected (connection id {1}).", desc.Player.Name, desc.Id);

								// Display a message to the room
								Act.ToRoom("§n has reconnected.", true, desc.Player, null, null);
							}
							// Player is being played by existing connection, take over the character
							else
							{
								// Detach old descriptor
								Descriptor old = desc.Player.Descriptor;
								old.Player = null;
								old.AddOutputLine("This character has been taken over by another connection, disconnecting...");
								// Output not sent to descriptors which will be closed, so we have to do it manually
								old.WriteOutput();
								old.Disconnect = true;

								// Attach new descriptor
								desc.Player.Descriptor = desc;

								desc.AddOutputLine();
								desc.AddOutputLine("You take over a character which was being played by another connection.");
								desc.AddOutputLine();
								CmdLook.LookAtRoom(desc.Player);

								// Log
								Log.Info("{0} has been taken over by connection id {1}.", desc.Player.Name, desc.Id);

								// Message to room
								Act.ToRoom("§n has been possessed by a new entity.", true, desc.Player, null, null);
							}
						}
						// Player was loaded from database
						else
						{
							// Attach to descriptor
							desc.Player.Descriptor = desc;

							// Player to room and PlayerList
							Handler.LivingToRoom(desc.Player, World.StartingZone.StartingRoom);
							World.Players.Add(desc.Player);

							// Log it
							Log.Info("{0} has entered the realm.", desc.Player.Name);

							// Display a message to room
							Act.ToRoom("§n has entered the realm.", true, desc.Player, null, null);

							// Display to player
							desc.AddOutputLine();
							desc.AddOutputLine("Welcome back " + desc.Player.Name + ".");
							desc.AddOutputLine();
							CmdLook.LookAtRoom(desc.Player);
						}

						// Set the connection state as Playing :)
						desc.ConnectionState = Descriptor.ConnStates.Playing;
					}
					// Wrong password, handle password attempts
					else
					{
						desc.PasswordAttempts++;

						if (desc.PasswordAttempts >= Settings.AllowedPasswordAttempts)
						{
							Log.Warning("{0} invalid password attempts for character {1} from connection id {2}.",
									Settings.AllowedPasswordAttempts, desc.Player.Name, desc.Id);
							desc.Disconnect = true;
							return;
						}
						else
						{
							desc.AddOutputLine("Wrong password, try again.");
							desc.AddOutput("Password: ");
						}
					}

					break;
				}

				// Verify the name for a new character
				case Descriptor.ConnStates.VerifyName:
				{
					if (input.Length >= 1 && input[0] == 'y' || input[0] == 'Y')
					{
						desc.AddOutput("Please enter a new password: ");
						desc.ConnectionState = Descriptor.ConnStates.NewPassword1;
					}
					else
					{
						desc.AddOutput("Ok. What is your name then? ");
						desc.ConnectionState = Descriptor.ConnStates.AskName;
					}

					break;
				}

				// Select a password for a new character
				case Descriptor.ConnStates.NewPassword1:
				{
					if (IsValidPlayerPassword(input) == false)
					{
						desc.AddOutputLine("Invalid password. Try again.");
						desc.AddOutput("Please enter a new password: ");
					}
					else
					{
						desc.Player.Password = EncryptPassword(input);
						desc.AddOutput("Please verify your password: ");
						desc.ConnectionState = Descriptor.ConnStates.NewPassword2;
					}

					break;
				}

				// Verify a password for a new character
				case Descriptor.ConnStates.NewPassword2:
				{
					if (EncryptPassword(input) == desc.Player.Password)
					{
						desc.AddOutput("Are you male or female? ");
						desc.ConnectionState = Descriptor.ConnStates.SelectSex;
					}
					else
					{
						desc.AddOutputLine("Passwords don't match. Start over.");
						desc.AddOutput("Please enter a new password: ");
						desc.ConnectionState = Descriptor.ConnStates.NewPassword1;
					}

					break;
				}

				// Ask for sex
				case Descriptor.ConnStates.SelectSex:
				{
					if (input.Length >= 1 && input[0] == 'm' || input[0] == 'M')
					{
						desc.Player.Sex = Genders.Male;
					}
					else if (input.Length >= 1 && input[0] == 'f' || input[0] == 'F')
					{
						desc.Player.Sex = Genders.Female;
					}
					else
					{
						desc.Player.Sex = Genders.Neutral;
						desc.AddOutput("Invalid sex. Are you male or female? ");
					}

					if (desc.Player.Sex != Genders.Neutral)
					{
						// Attach descriptor, initialize the player
						desc.Player.Descriptor = desc;
						desc.Player.InitializeNewPlayer();

						// Insert player to room and add to PlayerList
						Handler.LivingToRoom(desc.Player, World.StartingZone.StartingRoom);
						World.Players.Add(desc.Player);

						// Save new player to database
						DatabasePlayer.Save(desc.Player);

						// The first player is the Implementor!
						// This has to be after save because save gives us our Id
						// which tells us if this is first player or not.
						if (desc.Player.IsFounder())
						{
							desc.Player.Level = Player.LvlImpl;
						}

						// Log event and output welcome message to him
						Log.Info("New player {0} entering the realm (connection id {1}).", desc.Player.Name, desc.Id);

						// Display a message to game
						Act.ToRoom("§n has entered the realm.", true, desc.Player, null, null);

						desc.AddOutputLine();
						desc.AddOutputLine("Welcome " + desc.Player.Name + ".");
						desc.AddOutputLine();
						CmdLook.LookAtRoom(desc.Player);

						// State = Playing :)
						desc.ConnectionState = Descriptor.ConnStates.Playing;
					}

					break;
				}

				// Default, shouldn't get here
				default:
				{
					Log.Error("Invalid connection state {0} in character creation for connection id {1}.", desc.ConnectionState, desc.Id);
					desc.Disconnect = true;
					return;
				}
			}
		}

		/// <summary>
		/// Encrypts the password using SHA-1 hash algorithm.
		/// </summary>
		/// <param name="password">
		/// Plaintext password. <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Encrypted password. <see cref="System.String"/>
		/// </returns>
		private static string EncryptPassword(string password)
		{
			try
			{
				// Add our own unique key to the password
				password = "QMud_" + password;

				// Compute hash
				byte[] data = sha1.ComputeHash(Encoding.ASCII.GetBytes(password));

				// Convert it to hex string
				string result = BitConverter.ToString(data);

				// Finally remove the - characters and return it
				return result.Replace("-", "");
			}
			catch (Exception ex)
			{
				Log.Fatal("Error while encrypting password: " + ex.Message);
			}

			// We never get here since QLog.Fatal exits the game
			return "";
		}

		/// <summary>
		/// Checks that name is of valid length (defined in QSettings), and contains only letters.
		/// </summary>
		/// <param name="name">
		/// Name to test. <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// True if valid, otherwise false. <see cref="System.Boolean"/>
		/// </returns>
		private static bool IsValidPlayerName(string name)
		{
			string[] invalidPlayerNames =
			{
				"me",
				"all",
				"self"
			};

			// Check length
			if (name.Length < Settings.MinPlayerNameLength || name.Length > Settings.MaxPlayerNameLength)
			{
				return false;
			}

			// Check that all characters are letters
			if (name.Where(x => !Char.IsLetter(x)).Count() > 0)
			{
				return false;
			}

			// Check that name is not in invalid array
			if (invalidPlayerNames.Where(x => Utils.StrCmp(x, name)).Count() > 0)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Checks that password is of valid length, defined in QSettings.
		/// </summary>
		/// <param name="passwd">
		/// Password to test. <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// True if valid, otherwise false. <see cref="System.Boolean"/>
		/// </returns>
		private static bool IsValidPlayerPassword(string passwd)
		{
			if (passwd.Length < Settings.MinPlayerPasswordLength || passwd.Length > Settings.MaxPlayerPasswordLength)
			{
				return false;
			}

			return true;
		}
	}
}
