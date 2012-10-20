using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;

namespace Sync2Qif
{
	public class PdfToXmlReader
	{
		public static XDocument Read (string file)
		{
			if (!File.Exists (file))
				throw new ApplicationException (string.Format("File does not exist: {0}", file));

			Process xml2txt = new Process ();
			xml2txt.StartInfo.FileName = "pdf2txt";
			xml2txt.StartInfo.Arguments = string.Format ("-t xml {0}", file);
			xml2txt.StartInfo.UseShellExecute = false;
			xml2txt.StartInfo.RedirectStandardOutput = true;
			string output = "";

			try {
				xml2txt.Start();				
				output = xml2txt.StandardOutput.ReadToEnd();
				xml2txt.WaitForExit();
			}
			catch (System.ComponentModel.Win32Exception) {
				Console.WriteLine ("Error invoking xml2txt parser. Is the program installed?");
			}
			catch (Exception e) {
				Console.WriteLine ("Unexpected error occured while invoking external program xml2txt.");
				Console.WriteLine (e.Message);
				throw;
			}

			return XDocument.Parse (output);
		}
	}
}

