using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Bank2Qif
{
    public class QifFile
    {
        public static void Save (IEnumerable<QifEntry> entries, string originalFileName)
        {
            //entries = entries.OrderBy(e => e.Date.BookingDate);
            string nl = Environment.NewLine;
            StringBuilder output = new StringBuilder ("!Type:Bank" + nl);

            foreach (var entry in entries)
            {
                //TODO: gnucash needs merged payee and description, but should make it an option
                output.Append (entry.ToQifString (true));
                output.Append (nl);
            }

            var utf8WithoutBom = new UTF8Encoding (false);
            using (StreamWriter file = new StreamWriter (originalFileName, false, utf8WithoutBom))
            {
                file.Write (output.ToString ());
            }
        }


        public static string GetQifFileName (string originalFileName)
        {
            var extIndex = originalFileName.LastIndexOf (".");
            if (extIndex == -1)
                throw new ArgumentException ("Could not find '.' in file name");
            return string.Format ("{0}.{1}", originalFileName.Substring (0, extIndex), "qif");
        }
    }
}