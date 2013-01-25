using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.IO;
using iTextSharp;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using Sprache;
using Bank2Qif.Parsers;


namespace Bank2Qif.Converters.AliorSync
{
    [Converter("sync", "csv")]
    public class AliorSyncCsvToQif : IConverter
    {
        #region IConverter implementation

        public IEnumerable<QifEntry> ConvertFileToQif(string fileName)
        {
            string lines = File.ReadAllText(fileName, System.Text.Encoding.UTF8);
            return AliorSyncCsvParsers.QifEntriesParser.Parse(lines);
        }

        #endregion
    }
}

