using System;
using System.Collections.Generic;
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
            return MBankCsvParsers.QifEntriesParser.Parse(lines);
        }
    }
}
