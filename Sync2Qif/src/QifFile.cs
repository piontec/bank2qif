using System;
using System.Collections.Generic;

namespace Bank2Qif
{
	public class QifFile
	{
        public static void Save(IEnumerable<QifEntry> entries, string originalFileName)
        {
            throw new NotImplementedException();
        }

        private static string GetQifFileName(string originalFileName)
        {
            //var extIndex = pdfFileName.LastIndexOf(".pdf", StringComparison.CurrentCultureIgnoreCase);
            //if (extIndex == -1)
            //    throw new ApplicationException("Could not find .pdf in file name");
            //return pdfFileName.Remove(extIndex) + ".qif";
            throw new NotImplementedException();
        }
	}
}

