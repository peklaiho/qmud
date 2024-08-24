using System;

namespace QMud.Core
{
	/// <summary>
	/// Various settings which affect the mud's behavior.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// Name of the mud!
		/// </summary>
		public const string MudName = "Gauntlet MUD";

		/// <summary>
		/// Version of the mud.
		/// </summary>
		public const string MudVersion = "0.1.0";

		/// <summary>
		/// The port which the mud will run on.
		/// </summary>
		public const int Port = 4000;

		/// <summary>
		/// Time in milliseconds of how long the main loop waits between iterations.
		/// </summary>
		public const int MainLoopDelay = 100;

		/// <summary>
		/// Linebreak to send to telnet clients.
		/// </summary>
		public const string LineBreak = "\r\n";

		public const int NumExits = 6;

		/// <summary>
		/// Default line length of new players. They can adjust it later.
		/// </summary>
		public const int DefaultLineLength = 80;

		// Name/password limits
		public const int MinPlayerNameLength = 3;
		public const int MaxPlayerNameLength = 18;
		public const int MinPlayerPasswordLength = 5;
		public const int MaxPlayerPasswordLength = 32;

		/// <summary>
		/// Allowed password attempts during login.
		/// </summary>
		public const int AllowedPasswordAttempts = 3;

		/// <summary>
		/// Maximum number of zones.
		/// </summary>
		public const int MaxZones = 5;
	}
}
