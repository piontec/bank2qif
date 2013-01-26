using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Sprache;

namespace Bank2Qif.Converters.MBank
{
    [Converter("mbank", "csv")]
    public class MBankCsvToQif : IConverter
    {
        public IEnumerable<QifEntry> ConvertFileToQif(string fileName)
        {
            string lines = File.ReadAllText(fileName, System.Text.Encoding.Default);
            var entries = MBankCsvParsers.QifEntriesParser.Parse(lines);

            TextInfo myTI = new CultureInfo("pl-PL", false).TextInfo;
            // normalize many whitespaces into one, do camel casing
            string pattern = @"Xx(?<num>\d+)xx";
            string replacement = "XX${num}XX";            
            foreach (var entry in entries)
            {
                entry.Description = System.Text.RegularExpressions.Regex.Replace(
                    myTI.ToTitleCase(entry.Description.ToLower()), @"\s+", " ");
                // fix PayU IDs
                entry.Description = System.Text.RegularExpressions.Regex.Replace(
                    entry.Description, pattern, replacement);
                entry.Payee = System.Text.RegularExpressions.Regex.Replace(
                    myTI.ToTitleCase(entry.Payee.ToLower ()), @"\s+", " ");
            }

            return entries;
        }
    }
}
