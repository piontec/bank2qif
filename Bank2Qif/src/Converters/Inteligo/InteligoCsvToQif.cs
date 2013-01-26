using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;

namespace Bank2Qif.Converters.Inteligo
{
    [Converter("mbank", "csv")]
    public class InteligoCsvToQif : IConverter
    {
        public IEnumerable<QifEntry> ConvertFileToQif(string fileName)
        {
            string lines = File.ReadAllText(fileName, System.Text.Encoding.Default);
            var entries = InteligoCsvParsers.QifEntriesParser.Parse(lines);
        }
    }
}
