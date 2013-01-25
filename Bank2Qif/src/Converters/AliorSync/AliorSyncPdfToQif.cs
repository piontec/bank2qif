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
    [Converter ("syncpdf", "pdf")]
    public class AliorSyncPdfToQif : IConverter
    {
        private readonly string FIRST_PAGE_START = @"DATA OPERACJI OPERACJI OPERACJI KSIĘGOWE";
        private readonly string FIRST_PAGE_END = @"Infolinia Alior Sync dostępna jest przez całą dobę pod numerami 19 506";
        private readonly string NEXT_PAGE_START = @"Wyciąg z rachunku bankowego";
        private readonly string NEXT_PAGE_END = @"Niniejszy dokument jest wydrukiem komputerowym sporządzonym zgodnie z art. 7";

        private readonly string[] OpNames = new string[] 
		{
			"PRZELEW W RAMACH BANKU NA RACH OBCY", 
			"PRZELEW DO INNEGO BANKU KRAJOWEGO",
			"PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO",
			"PRZELEW NATYCHMIASTOWY",
			"ZAŁOŻENIE LOKATY",
            "TRANSAKCJA KARTĄ DEBETOWĄ",
            "PRZELEW WEWNĘTRZNY - PŁACĘ Z ALIOR BANKIEM",
            "KAPITALIZACJA ODSETEK",
		};


        public IEnumerable<QifEntry> ParseToQif(string lines)
        {
            return AliorSyncPdfParsers.QifEntriesParser.Parse(lines);
        }


        public QifEntry ParseSingleToQif(string lines)
        {
            return AliorSyncPdfParsers.QifEntryParser.Parse(lines);
        }


        public IEnumerable<QifEntry> ConvertFileToQif(string fileName)
        {
            var lines = ExtractLinesFromPdf(fileName);

            return ConvertExtractedTextToQif(lines);
        }

        public IEnumerable<QifEntry> ConvertExtractedTextToQif(IEnumerable<string> lines)
        {
            if (!OpNames.Any(s => lines.ElementAt(0).Contains(s)))
                throw new ArgumentException("Bad first line");
            var current = new List<string>();
            var entries = new List<QifEntry>();

            foreach (var line in lines)
            {
                // if it is a line starting with correct operation name and if it is a subsequent operation line
                if (OpNames.Any(s => line.Contains(s)) && current.Count > 1)
                {
                    var total = current.Aggregate((s1, s2) => string.Format("{0}{1}{2}", s1, Environment.NewLine, s2));
                    entries.Add(ParseSingleToQif(total));
                    current.Clear();
                }
                current.Add(line);
            }
            var lastOne = current.Aggregate((s1, s2) => string.Format("{0}{1}{2}", s1, Environment.NewLine, s2));
            entries.Add(ParseSingleToQif(lastOne));

            return entries;
        }


        private IEnumerable<string> ExtractLinesFromPdf(string fileName)
        {
            PdfReader reader = new PdfReader(fileName);

            IEnumerable<string> lines = new List<string>();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                string input = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                lines = lines.Concat(TrimStrings(input, i == 1));
            }

            reader.Close();
            return lines;
        }


        public IEnumerable<string> TrimStrings(string pdfInput, bool isFirstPage = false)
        {
            var strings = pdfInput.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var startDelim = isFirstPage ? FIRST_PAGE_START : NEXT_PAGE_START;
            var endDelim = isFirstPage ? FIRST_PAGE_END : NEXT_PAGE_END;

            int firstId = strings.IndexOf(strings.Where(s => s.StartsWith(startDelim)).Single());
            int lastId = strings.IndexOf(strings.Where(s => s.StartsWith(endDelim)).Single());

            return strings.Where((s, i) => i > firstId && i < lastId);
        }       
    }
}
