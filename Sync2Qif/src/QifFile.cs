using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bank2Qif
{
	public class QifFile
	{
        public static void Save(IEnumerable<QifEntry> entries, string originalFileName)
        {
            StringBuilder output = new StringBuilder();

            foreach (var entry in entries)
            {
                output.Append(entry.ToQifString());
                output.Append(System.Environment.NewLine);
            }

            using (StreamWriter file = new StreamWriter (originalFileName, false, Encoding.UTF8))
            {
                file.Write(output.ToString());
            }
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

