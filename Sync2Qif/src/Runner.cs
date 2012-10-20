using System;
using System.IO;
using System.Xml.Linq;
using System.Data.Linq;

namespace Sync2Qif
{
	public class Runner
	{
		public static void Main (string[] args)
		{
			if (!ValidateArgs (args)) {
				DisplayHelp (args);
				return;
			}

			var sync2qif = new Sync2QifParser ();
			sync2qif.Run (args [1]);
		}


		static bool ValidateArgs (string[] args)
		{
			if (args.Length != 2) {
				Console.WriteLine ("Wrong number of parameters");
				return false;
			}

			string fileName = args [1];
			if (!File.Exists (fileName)) {
				Console.WriteLine ("File does not exists");
				return false;
			}

			if (!File.Exists (fileName)) {
				Console.WriteLine ("File does not exists");
				return false;
			}

			if (!fileName.EndsWith (".pdf", StringComparison.CurrentCultureIgnoreCase)) {
				Console.WriteLine ("Wrong file extension, *.pdf expected");
				return false;
			}

			return true;
		}


		static void DisplayHelp (string[] args)
		{
			Console.WriteLine (string.Format ("Usage: {0} [pdf file name]", args [0]));
		}	
	}
}
