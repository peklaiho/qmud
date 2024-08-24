using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

using QMud.Commands;

namespace QMud.Core
{
	public class Descriptor
	{
		/// <summary>
		/// List of possible connection states.
		/// </summary>
		public enum ConnStates
		{
			Playing,
			AskName,
			AskPassword,
			VerifyName,
			NewPassword1,
			NewPassword2,
			SelectSex
		};

		/// <summary>
		/// Id for the next client that connects.
		/// </summary>
		private static int NextDescriptorId = 1;

		/// <summary>
		/// Size of the input buffer, in bytes.
		/// </summary>
		private static int InputBufferSize = 2048;

		/// <summary>
		/// Size of the output buffer, in bytes.
		/// </summary>
		private static int OutputBufferSize = 4096;

		/// <summary>
		/// The shared input buffer used when reading input from clients.
		/// </summary>
		private static byte[] InputBuffer = new byte[InputBufferSize];

		/// <summary>
		/// The shared output buffer used for writing output to clients.
		/// </summary>
		private static byte[] OutputBuffer = new byte[OutputBufferSize];

		/// <summary>
		/// The regular expression used to split input and separate the first command.
		/// </summary>
		private static Regex RegPattern = new Regex("[\\r\\n]+");

		// Public attributes
		public int Id { get; private set; }
		public ConnStates ConnectionState { get; set; }
		public Player Player { get; set; }
		public int PasswordAttempts { get; set; }
		public bool Disconnect { get; set; }
		public bool HadInput { get; set; }

		// Private attributes
		private Socket Connection;
		private string NextCommand = null;
		private string InputQueue = "";
		private string OutputQueue = "";

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="newConn">
		/// Connection that is associated with this descriptor. <see cref="TcpClient"/>
		/// </param>
		public Descriptor (Socket newConn)
		{
			// Get next id for this one, and increment NextDescriptorId
			Id = NextDescriptorId++;

			// Assign parameters
			Connection = newConn;

			// The new connection starts in the AskName state
			ConnectionState = Descriptor.ConnStates.AskName;
		}

		/// <summary>
		/// Adds text to the output queue to be sent during next iteration of WriteOutput.
		/// </summary>
		/// <param name="text">
		/// Text to add to output queue. <see cref="System.String"/>
		/// </param>
		public void AddOutput (string text)
		{
			OutputQueue += text;
		}

		/// <summary>
		/// Adds a linebreak to the output queue.
		/// </summary>
		public void AddOutputLine ()
		{
			OutputQueue += Settings.LineBreak;
		}

		/// <summary>
		/// Adds text to the output queue, followed by a linebreak.
		/// </summary>
		/// <param name="text">
		/// Text to add to output queue. <see cref="System.String"/>
		/// </param>
		public void AddOutputLine (string text)
		{
			OutputQueue += text + Settings.LineBreak;
		}

		/// <summary>
		/// Close the connection and remove it from DescriptorList.
		/// </summary>
		public void Close ()
		{
			try
			{
				// No need to log if we are shutting down the mud
				if (Mud.RunMud)
				{
					Log.Info("Closing connection id {0}.", Id);
				}

				// Check if the descriptor is connected to a player
				// We don't need to go linkless if the whole game is shutting down
				if (Mud.RunMud && this.Player != null)
				{
					// If character is in game, we leave him linkless
					if (this.ConnectionState == Descriptor.ConnStates.Playing)
					{
						// Display message to room
						Act.ToRoom("§n has lost §s link.", true, this.Player, null, null);

						// Log the event
						Log.Info("{0} has lost his/her link from connection id {1}.", this.Player.Name, Id);

						// Detach the player from this descriptor
						this.Player.Descriptor = null;
					}
					// Else we got disconnected during character creation
					else
					{
						// Player could point to a player character which is being
						// played by ANOTHER descriptor, in case this descriptor
						// was trying to take over that character.
						// -> We cannot call ExtractLiving on the Player

						// If this was a normal login attempt, we still don't need
						// to call ExtractLiving because that player is not in game
						// yet (not in PlayerList).
					}
				}

				// Close the socket
				Connection.Close();
			}
			catch (Exception ex)
			{
				Log.Warning("Error occured while closing connection id " + Id + ": " + ex.Message);
			}
			finally
			{
				// Remove it from DescriptorList
				Network.RemoveFromDescriptorList(this);
			}
		}

		/// <summary>
		/// Executes the next command in stored in NextCommand attribute.
		/// </summary>
		public void ExecuteNextCommand ()
		{
			HadInput = false;

			// Send the command to correct interpreter depending on connection state
			if (ConnectionState == Descriptor.ConnStates.Playing)
			{
				// We should always have a player object if we are playing, otherwise we have a bad error
				if (Player == null)
				{
					Log.Error("Connection id {0} is in Playing state but has no Player object.", Id);
					Disconnect = true;
					return;
				}

				// Try to execute a normal non-queued command if we have one.
				if (NextCommand != null)
				{
					CommandInterpreter.InterpretCommand(Player, NextCommand);
					HadInput = true;
					NextCommand = null;
				}
			}
			else
			{
				// Handle the command in CharacterCreation
				if (NextCommand != null)
				{
					CharacterCreation.HandleCharacterCreation(this, NextCommand);
					HadInput = true;
					NextCommand = null;
				}
			}
		}

		/// <summary>
		/// Prepares the next command from OutputQueue into NextCommand.
		/// </summary>
		public void PrepareNextCommand ()
		{
			// If we don't have input we can leave right away
			// Also exit if we still have a unprocessed command waiting
			if (InputQueue.Length == 0 || NextCommand != null)
			{
				return;
			}

			// Split input by using our regular expression
			string[] result = RegPattern.Split(InputQueue, 2);

			// If length is 2 we have a new command
			if (result.Length == 2)
			{
				// Set the new command to be executed
				NextCommand = result[0];

				// Set the remainder to the queue
				InputQueue = result[1];
			}
		}

		/// <summary>
		/// Read input from client.
		///
		/// If client is still connected, but has no data to read, Receive will return SocketException WSAEWOULDBLOCK.
		/// If client has disconnected, Receive will return 0.
		/// </summary>
		public void ReadInput ()
		{
			int bytesRead = 0;

			try
			{
				// We can only read up to InputBufferSize by one call to this function
				do
				{
					bytesRead += Connection.Receive(InputBuffer, bytesRead, InputBufferSize - bytesRead, SocketFlags.None);
				}
				while (Connection.Available > 0 && bytesRead < InputBufferSize);

				// If we read something, lets add it to InputQueue
				if (bytesRead > 0)
				{
					InputQueue += Encoding.ASCII.GetString(InputBuffer, 0, bytesRead);
				}
				// Else if we received 0 the client has disconnected
				else
				{
					Log.Info("Connection id {0} has disconnected.", Id);
					Disconnect = true;
					return;
				}

				// Lets have some maximum limit for InputQueue string length.
				// This would fill up if the client sent a really long text
				// with no linebreaks.
				if (InputQueue.Length > (4 * InputBufferSize))
				{
					Log.Error("InputQueue length exceeded {0} characters for connection id {1}.", (4 * InputBufferSize), Id);

					// Remove input and send message to player
					InputQueue = "";
					AddOutputLine("ERROR: Continuous input with no linebreaks exceeded allowed length.");
				}
			}
			catch (SocketException sex)
			{
				// 10035 == WSAEWOULDBLOCK
				if (sex.ErrorCode != 10035)
				{
					Log.Warning("Error occured while reading from connection id " + Id + ": " + sex.Message);
					Disconnect = true;
					return;
				}
			}
			catch (Exception ex)
			{
				Log.Warning("Error occured while reading from connection id " + Id + ": " + ex.Message);
				Disconnect = true;
				return;
			}
		}

		/// <summary>
		/// Add prompt to OutputQueue if this player has any output to begin with.
		/// </summary>
		public void ShowPrompt ()
		{
			// Do not add prompt if
			// 1. Mud is shutting down
			// 2. Client is going to be disconnected
			// 3. Descriptor is in different state than Playing
			// 4. Descriptor has no Player attached
			if (Mud.RunMud == false || Disconnect || ConnectionState != Descriptor.ConnStates.Playing ||
			    Player == null)
			{
				return;
			}

			// Do not add prompt if there is no output and no input
			if (OutputQueue.Length == 0 && HadInput == false)
			{
				return;
			}

			// Check for compact mode (from preferences)
			if (Player.Preferences.Get((int) Player.PrefBits.Compact) == false)
			{
				// Add extra linebreak before prompt
				AddOutputLine();
			}

			if (Player.InFight != null)
			{
				AddOutput("Fight");
			}

			// Add the prompt
			AddOutput("> ");
		}

		/// <summary>
		/// Writes buffered output to client.
		/// </summary>
		public void WriteOutput ()
		{
			int bytesWritten = 0, bytesToWrite;

			// Return immediately if no data to send
			if (OutputQueue.Length == 0)
			{
				return;
			}

			try
			{
				// If the player had no input, we add one linebreak before output
				// so the output doesn't start on the same line as the previous prompt.
				if (HadInput == false)
				{
					OutputQueue = Settings.LineBreak + OutputQueue;
				}

				// Check that we don't have too long output
				if (Encoding.ASCII.GetMaxCharCount(OutputBufferSize) < OutputQueue.Length)
				{
					Log.Error("OutputQueue length exceeded OutputBufferSize for connection id {0}.", Id);

					// Truncate output
					OutputQueue = OutputQueue.Substring(0, Encoding.ASCII.GetMaxCharCount(OutputBufferSize));
				}

				// Convert from string to bytes
				bytesToWrite = Encoding.ASCII.GetBytes(OutputQueue, 0, OutputQueue.Length, OutputBuffer, 0);

				// Send output
				while (bytesWritten < bytesToWrite)
				{
					bytesWritten += Connection.Send(OutputBuffer, bytesWritten, bytesToWrite - bytesWritten, SocketFlags.None);
				}

				// Clear the queue
				OutputQueue = "";
			}
			catch (SocketException sex)
			{
				Log.Warning("Error occured while writing to connection id " + Id + ": " + sex.Message);
				Disconnect = true;
				return;
			}
			catch (Exception ex)
			{
				Log.Warning("Error occured while writing to connection id " + Id + ": " + ex.Message);
				Disconnect = true;
				return;
			}
		}
	}
}
