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
			xml2txt.StartInfo.FileName = "pdf2xml";
			xml2txt.StartInfo.Arguments = string.Format ("-t xml {0}", file);
			xml2txt.StartInfo.UseShellExecute = false;
			xml2txt.StartInfo.RedirectStandardOutput = true;
			xml2txt.Start();
			
			string output = xml2txt.StandardOutput.ReadToEnd();
			xml2txt.WaitForExit();

			return XDocument.Parse (output);
		}
	}
}

