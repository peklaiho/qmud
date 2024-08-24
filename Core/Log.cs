using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace QMud.Core
{
	/// <summary>
	/// Provides functions for writing messages to logfile.
	/// </summary>
	public class Log
	{
		public static List<string> ErrorList = new List<string>();
		public static List<string> GodCmdList = new List<string>();
		public static List<string> InfoList = new List<string>();
		public static List<string> WarningList = new List<string>();
		public static List<string> DebugList = new List<string>();

		#region Public static functions
		
		/// <summary>
		/// Writes the given error message to today's logfile.
		/// </summary>
		/// <param name="txt">
		/// Message to write. <see cref="System.String"/>
		/// </param>
		public static void Error (string txt, params object[] p)
		{
			txt = String.Format(txt, p);
			ErrorList.Add(txt);
			PerformLog("E", txt);
		}
		
		/// <summary>
		/// Writes a fatal error message to today's logfile and shuts down the mud.
		/// </summary>
		/// <param name="txt">
		/// Message to write. <see cref="System.String"/>
		/// </param>
		public static void Fatal (string txt, params object[] p)
		{
			txt = String.Format(txt, p);
			PerformLog("F", txt);
			Environment.Exit(0);
		}
		
		/// <summary>
		/// Log an executed immortal command.
		/// </summary>
		/// <param name="txt">
		/// Description of the event. <see cref="System.String"/>
		/// </param>
		public static void GodCmd (string txt, params object[] p)
		{
			txt = String.Format(txt, p);
			GodCmdList.Add(txt);
			PerformLog("G", txt);
		}
		
		/// <summary>
		/// Writes the given informative message to today's logfile.
		/// </summary>
		/// <param name="txt">
		/// Message to write. <see cref="System.String"/>
		/// </param>
		public static void Info (string txt, params object[] p)
		{
			txt = String.Format(txt, p);
			InfoList.Add(txt);
			PerformLog("I", txt);
		}
		
		/// <summary>
		/// Writes the given warning message to today's logfile.
		/// </summary>
		/// <param name="txt">
		/// Message to write. <see cref="System.String"/>
		/// </param>
		public static void Warning (string txt, params object[] p)
		{
			txt = String.Format(txt, p);
			WarningList.Add(txt);
			PerformLog("W", txt);
		}

		public static void Debug (string txt, params object[] p)
		{
			txt = String.Format(txt, p);
			DebugList.Add(txt);
			PerformLog("D", txt);
		}

		#endregion
		
		#region Private static functions
		
		private static string GetTimestamp ()
		{
			return DateTime.Now.ToString("HH:mm:ss.fff");
		}
		
		private static void PerformLog (string type, string txt)
		{
			string logFile = "Log/" + DateTime.Now.ToString("yyyy_MM_dd") + ".log";
			
			try
			{
				using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.ASCII))
				{
					sw.WriteLine(String.Format("{0} {1} {2}", type, GetTimestamp(), txt));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Can't write to log file: " + ex.Message);
				Environment.Exit(0);
			}

			// Show log message to immortals
			foreach (Player player in World.Players)
			{
				if (player.Level >= Player.LvlGrGod)
				{
					if (player.Descriptor != null && player.Descriptor.ConnectionState == Descriptor.ConnStates.Playing)
					{
						player.OutLn(String.Format("{0}[{1}] {2}{3}", player.GetColorPref(ColorLoc.Highlight),
						                           type, txt, player.GetColor(Color.Reset)));
					}
				}
			}
		}
		
		#endregion
	}
}
