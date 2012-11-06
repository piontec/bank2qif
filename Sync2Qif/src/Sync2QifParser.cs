using System;

namespace Sync2Qif
{
	public class Sync2QifParser
	{
		public void Run (string pdfFileName)
		{
			var xml = PdfToXmlReader.Read (pdfFileName);
			var qifEntries = new AliorSyncXmlToQif ().ConvertFileToQif (xml);
			var qifFile = new QifFile (qifEntries);
			qifFile.Save (GetQifFileName (pdfFileName));
		}
		
		
		public string GetQifFileName (string pdfFileName)
		{
			var extIndex = pdfFileName.LastIndexOf (".pdf", StringComparison.CurrentCultureIgnoreCase);
			if (extIndex == -1)
				throw new ApplicationException ("Could not find .pdf in file name");
			return pdfFileName.Remove (extIndex) + ".qif";
		}
	}
}

