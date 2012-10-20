using System;
using Sync2Qif;
using System.Xml.Linq;

namespace Sync2QifPlayground
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Loading file");
			var xdoc = PdfToXmlReader.Read (@"../../../Sync2QifTests/data/wyciag1.pdf");
			Console.WriteLine (xdoc);
		}
	}
}
