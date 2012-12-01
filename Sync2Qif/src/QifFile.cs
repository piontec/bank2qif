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
            var extIndex = originalFileName.LastIndexOf(".");
            if (extIndex == -1)
                throw new ArgumentException("Could not find '.' in file name");
            return string.Format("{0}.{1}", originalFileName.Substring(0, extIndex), "qif");
        }
	}
}

