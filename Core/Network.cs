using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace QMud.Core
{
	public class Network
	{
		/// <summary>
		/// Mother socket.
		/// </summary>
		private static TcpListener MyListener;
		
		/// <summary>
		/// List of connected descriptors.
		/// </summary>
		private static List<Descriptor> DescriptorList = new List<Descriptor>();
		
		/// <summary>
		/// Accepts any incoming connection that might be waiting.
		/// </summary>
		public static void AcceptConnection ()
		{
			Descriptor desc = null;
			
			try
			{
				// Check if we have pending connection
				if (MyListener.Pending())
				{
					// Accept the connection and create TcpClient
					Socket client = MyListener.AcceptSocket();
					
					// Set the client to non-blocking mode
					client.Blocking = false;
						
					// Create new QDescriptor for our client
					desc = new Descriptor(client);

					// Get the ip address
					IPEndPoint ipAddr = (IPEndPoint) client.RemoteEndPoint;
					
					// Log the happy event
					Log.Info("New connection (id {0}) from {1}.", desc.Id, ipAddr.Address.ToString());
					
					// Send a welcome message to our client
					desc.AddOutputLine("Welcome to " + Settings.MudName + ".");
					desc.AddOutputLine();
					desc.AddOutput("What is your name? ");
					
					// Only at last, if everything went ok, we add him to our DescriptorList
					DescriptorList.Add(desc);
				}
			}
			catch (Exception ex)
			{
				// If we did get as far as creating a QDescriptor item, try to close it
				if (desc != null)
				{
					desc.Close();
				}
				
				Log.Error("Error while accepting connection: " + ex.Message);
			}
		}
		
		/// <summary>
		/// Add text to output queue for all descriptors.
		/// </summary>
		/// <param name="text">
		/// Text to add to output. <see cref="System.String"/>
		/// </param>
		/// <param name="playingOnly">
		/// True if sent only to sockets in playing state, False to send to everyone. <see cref="System.Boolean"/>
		/// </param>
		public static void AddAllOutputLine(string text, bool playingOnly)
		{
			foreach (Descriptor desc in DescriptorList)
			{
				if (desc.Disconnect == false)
				{
					if (desc.ConnectionState == Descriptor.ConnStates.Playing || playingOnly == false)
					{
						desc.AddOutputLine(text);
					}
				}
			}
		}
		
		/// <summary>
		/// Disconnects connected clients first, and then stops the TcpListener.
		/// </summary>
		public static void Close ()
		{
			try
			{
				Log.Info("Disconnecting {0} connected clients.", DescriptorList.Count);
				
				// Loop until we have no descriptors left
				while (DescriptorList.Count > 0)
				{
					// Get the last one
					Descriptor desc = DescriptorList[DescriptorList.Count - 1];
					
					// Close the client
					desc.Close();
				}
				
				// Close the TcpListener
				Log.Info("Closing mother socket.");
				MyListener.Stop();
			}
			catch (Exception ex)
			{
				// We are shutting down anyway, not a very serious error if we get it
				Log.Error("Error while closing mother socket: " + ex.Message);
			}
		}
		
		/// <summary>
		/// Disconnect any descriptors who are marked to be closed.
		/// </summary>
		public static void DisconnectDescriptors ()
		{
			// Loop through descriptors, can't use foreach here because we modify DescriptorList.
			for (int i = 0; i < DescriptorList.Count; )
			{
				Descriptor desc = DescriptorList[i];
				
				if (desc.Disconnect)
				{
					desc.Close();
				}
				else
				{
					i++;
				}
			}
		}
		
		/// <summary>
		/// Executes the next pending command for all connected clients.
		/// </summary>
		public static void ExecuteAllNextCommands ()
		{
			foreach (Descriptor desc in DescriptorList)
			{
				if (desc.Disconnect == false)
				{
					desc.ExecuteNextCommand();
				}
			}
		}
		
		/// <summary>
		/// Initializes the TcpListener on port which is defined in QSettings.
		/// </summary>
		public static void InitListener ()
		{
			try
			{
				Log.Info("Listening for incoming connections on port {0}.", Settings.Port);
				
				// Start the TcpListener
				MyListener = new TcpListener(IPAddress.Any, Settings.Port);
				MyListener.Start();
			}
			catch (Exception ex)
			{
				Log.Fatal("Error while binding mother socket: " + ex.Message);
			}
		}
		
		/// <summary>
		/// Prepares next command for execution for all connected clients.
		/// </summary>
		public static void PrepareAllNextCommands ()
		{
			foreach (Descriptor desc in DescriptorList)
			{
				if (desc.Disconnect == false)
				{
					desc.PrepareNextCommand();
				}
			}
		}
		
		/// <summary>
		/// Read input of all connected clients.
		/// </summary>
		public static void ReadAllInput ()
		{
			foreach (Descriptor desc in DescriptorList)
			{
				if (desc.Disconnect == false)
				{
					desc.ReadInput();
				}
			}
		}
		
		/// <summary>
		/// Removes the given descriptor from the list.
		/// </summary>
		/// <param name="desc">
		/// Descriptor to remove. <see cref="QDescriptor"/>
		/// </param>
		public static void RemoveFromDescriptorList (Descriptor desc)
		{
			DescriptorList.Remove(desc);
		}
		
		/// <summary>
		/// Shows prompts for all connected clients who had output.
		/// </summary>
		public static void ShowAllPrompts ()
		{
			foreach (Descriptor desc in DescriptorList)
			{
				if (desc.Disconnect == false)
				{
					desc.ShowPrompt();
				}
			}
		}
		
		/// <summary>
		/// Write output to all connected clients.
		/// </summary>
		public static void WriteAllOutput ()
		{
			foreach (Descriptor desc in DescriptorList)
			{
				if (desc.Disconnect == false)
				{
					desc.WriteOutput();
				}
			}
		}
	}
}
