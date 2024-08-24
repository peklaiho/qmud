using System;
using System.Collections.Generic;

namespace QMud.Core
{
	public class Color
	{	
		// Special
		public static Color Reset { get; private set; }
			
		// Colors
		public static Color Black { get; private set; }
		public static Color Red { get; private set; }
		public static Color Green { get; private set; }
		public static Color Yellow { get; private set; }
		public static Color Blue { get; private set; }
		public static Color Magenta { get; private set; }
		public static Color Cyan { get; private set; }
		public static Color White { get; private set; }
			
		public static Color BrightBlack { get; private set; }
		public static Color BrightRed { get; private set; }
		public static Color BrightGreen { get; private set; }
		public static Color BrightYellow { get; private set; }
		public static Color BrightBlue { get; private set; }
		public static Color BrightMagenta { get; private set; }
		public static Color BrightCyan { get; private set; }
		public static Color BrightWhite { get; private set; }
			
		public static Color DimBlack { get; private set; }
		public static Color DimRed { get; private set; }
		public static Color DimGreen { get; private set; }
		public static Color DimYellow { get; private set; }
		public static Color DimBlue { get; private set; }
		public static Color DimMagenta { get; private set; }
		public static Color DimCyan { get; private set; }
		public static Color DimWhite { get; private set; }

		public static List<Color> ColorList;
		
		public int Index { get; private set; }
		public string Name { get; private set; }
		public string Code { get; private set; }
		
		public Color (string newName, string newCode)
		{
			Name = newName;
			Code = newCode;
		}
		
		public static void InitializeColors ()
		{
			char escape = (char) 27;
			
			Log.Info("Initializing colors.");
			
			ColorList = new List<Color>();
			
			ColorList.Add((Reset = new Color("default", escape + "[0m")));
		
			ColorList.Add((Black = new Color("black", escape + "[0;30m")));
			ColorList.Add((Red = new Color("red", escape + "[0;31m")));
			ColorList.Add((Green = new Color("green", escape + "[0;32m")));
			ColorList.Add((Yellow = new Color("yellow", escape + "[0;33m")));
			ColorList.Add((Blue = new Color("blue", escape + "[0;34m")));
			ColorList.Add((Magenta = new Color("magenta", escape + "[0;35m")));
			ColorList.Add((Cyan = new Color("cyan", escape + "[0;36m")));
			ColorList.Add((White = new Color("white", escape + "[0;37m")));
			
			ColorList.Add((BrightBlack = new Color("bblack", escape + "[1;30m")));
			ColorList.Add((BrightRed = new Color("bred", escape + "[1;31m")));
			ColorList.Add((BrightGreen = new Color("bgreen", escape + "[1;32m")));
			ColorList.Add((BrightYellow = new Color("byellow", escape + "[1;33m")));
			ColorList.Add((BrightBlue = new Color("bblue", escape + "[1;34m")));
			ColorList.Add((BrightMagenta = new Color("bmagenta", escape + "[1;35m")));
			ColorList.Add((BrightCyan = new Color("bcyan", escape + "[1;36m")));
			ColorList.Add((BrightWhite = new Color("bwhite", escape + "[1;37m")));
			
			ColorList.Add((DimBlack = new Color("dblack", escape + "[2;30m")));
			ColorList.Add((DimRed = new Color("dred", escape + "[2;31m")));
			ColorList.Add((DimGreen = new Color("dgreen", escape + "[2;32m")));
			ColorList.Add((DimYellow = new Color("dyellow", escape + "[2;33m")));
			ColorList.Add((DimBlue = new Color("dblue", escape + "[2;34m")));
			ColorList.Add((DimMagenta = new Color("dmagenta", escape + "[2;35m")));
			ColorList.Add((DimCyan = new Color("dcyan", escape + "[2;36m")));
			ColorList.Add((DimWhite = new Color("dwhite", escape + "[2;37m")));
			
			// Set indexes
			for (int i = 0; i < ColorList.Count; i++)
			{
				ColorList[i].Index = i;
			}
		}
	}
}
