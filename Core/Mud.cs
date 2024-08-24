using System;
using System.Threading;

using QMud.Commands;
using QMud.Database;

namespace QMud.Core
{
	/// <summary>
	/// Main class for QMud.
	/// </summary>
	class Mud
	{
		public static DateTime StartupTime { get; private set; }
		public static bool RunMud = true;

		public static uint MainLoopIterations { get; private set; }

		public static void Initialize ()
		{
			// Record start time so we can use the uptime command
			StartupTime = DateTime.Now;

			// Register exception handler for unhandled exceptions
			RegisterExceptionHandler();

			// Initialize commands
			CommandInterpreter.InitializeCommands();

			// Initialize colors
			Color.InitializeColors();
			ColorLoc.InitializeColorLocations();

			// Load stuff from database
			Database.Database.LoadWorld();

			// Initialize starting zone
			World.InitStartingZone();

			// Initialize random names
			RandomNameGenerator.Initialize();

			// Start TcpListener
			Network.InitListener();
		}

		public static void Main (string[] args)
		{
			Log.Info("Starting program.");

			// Initialize
			Initialize();

			// Main loop
			MainLoop();

			// Free resources
			Shutdown();

			Log.Info("Exiting program.");
		}

		public static void MainLoop ()
		{
			while (RunMud)
			{
				// Record loop start time
				DateTime startTime = DateTime.Now;

				// Accept incoming connection if available
				Network.AcceptConnection();

				// Read input from clients
				Network.ReadAllInput();

				// Prepare next command
				Network.PrepareAllNextCommands();

				// Execute next command
				Network.ExecuteAllNextCommands();

				// Update fights
				Fight.UpdateFights();

				// Update everything else
				Updater.Update();

				// Add prompt
				Network.ShowAllPrompts();

				// Write output
				Network.WriteAllOutput();

				// Disconnect any descriptors that are marked to be closed
				Network.DisconnectDescriptors();

				// Check how much time passed during this iteration
				TimeSpan elapsedTime = DateTime.Now - startTime;

				// If we have time left, sleep for the remainder
				if (elapsedTime.TotalMilliseconds < Settings.MainLoopDelay)
				{
					try
					{
						Thread.Sleep(Settings.MainLoopDelay - Convert.ToInt32(elapsedTime.TotalMilliseconds));
					}
					catch (Exception) { }
				}

				// Increase amount of iterations we have performed
				MainLoopIterations++;
			}
		}

		public static void Shutdown ()
		{
			// Close down TcpListener
			Network.Close();
		}

		private static void HandleException(object sender, UnhandledExceptionEventArgs args)
		{
			Exception ex = (Exception) args.ExceptionObject;

			// Log the nasty bug and exit game
			Log.Fatal(ex.ToString());
		}

		private static void RegisterExceptionHandler()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleException);
		}
	}
}
