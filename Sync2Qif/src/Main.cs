using System;
using System.IO;
using System.Xml.Linq;

namespace Sync2Qif
{
	class Sync2QifParser
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

			if (!File.Exists (args [1])) {
				Console.WriteLine ("File does not exists");
				return false;
			}

			return true;
		}


		static void DisplayHelp (string[] args)
		{
			Console.WriteLine (string.Format ("Usage: {0} [{1}]", args [0], args [1]));
		}


		private void Run (string pdfFile)
		{
			var xml = PdfToXmlReader.Read (pdfFile);
			var qifEntries = new AliorSyncXmlToQif ().ConvertXmlToQif (xml);
			var qifFile = new QifFile (qifEntries);
			qifFile.Save (GetQifFileName (pdfFile));
		}


		string GetQifFileName (string pdfFile)
		{
			throw new NotImplementedException ();
		}
	}
}
