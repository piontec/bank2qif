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
            string nl = System.Environment.NewLine;
            StringBuilder output = new StringBuilder("!Type:Bank" + nl);

            foreach (var entry in entries)
            {
                output.Append(entry.ToQifString());
                output.Append(nl);
            }

            using (StreamWriter file = new StreamWriter (originalFileName, false, Encoding.UTF8))
            {
                file.Write(output.ToString());
            }
        }

        public static string GetQifFileName(string originalFileName)
        {
            var extIndex = originalFileName.LastIndexOf(".");
            if (extIndex == -1)
                throw new ArgumentException("Could not find '.' in file name");
            return string.Format("{0}.{1}", originalFileName.Substring(0, extIndex), "qif");
        }
	}
}

