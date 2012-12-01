using System;
using Bank2Qif;
using System.Xml.Linq;
using System.Data.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;
using Bank2Qif.Converters;
using iTextSharp;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using System.IO;

namespace Sync2QifPlayground.src
{
	class MainClass
	{
		private readonly Func<XElement, XElement, XElement> strAgg = (e1, e2) => new XElement ("line", (string)e1 + (string)e2);
		private static readonly string [] OpNames = new string [] 
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

		private IEnumerable<XElement> ExtractBoxes (XElement page, int firstBoxId)
		{
			int lastBoxId = int.Parse ((
				from b in page.Elements ("textbox")
				let firstLn = b.Elements ("textline").First ()
				let firstLnTxt = (string)firstLn.Elements ("text").Aggregate (strAgg)
				where firstLnTxt.StartsWith ("Infolinia")
				select b.Attribute ("id").Value)
			                           .First ());
			var boxes = 
				from b in page.Elements ("textbox")
				let bid = (int)b.Attribute ("id")
				where bid > firstBoxId && bid < lastBoxId
				select b;
			new XElement ("root", boxes).Save ("/tmp/boxes.xml");
			int newid = 0;
			var lines = 
				from b in boxes
				select new XElement ("box", 
					                   new XAttribute ("id", newid++), 
				                       from tl in b.Elements ("textline")
				                       select tl.Elements ("text")
				                       .Aggregate (strAgg)
					                );
			new XElement ("root", lines).Save ("/tmp/lines.xml");
			return lines;
		}


		private IEnumerable<QifEntry> ConvertBoxes (IEnumerable<XElement> boxes)
		{
			var result = new List<QifEntry> ();
			var boxesList = boxes.ToList ();

			var blockDates = new List<BankDates> ();
			IList<QifEntry> blockEntries;
			for (int i = 0; i < boxesList.Count; i++) {
				var box = boxesList [i];
				BankDates date = BankDates.TryParse (box);
				if (date != null) {
					blockDates.Add (date);
					continue;
				}
				int parsedBoxes;
				blockEntries = TryParseTransactions (boxesList, i, blockDates.Count, out parsedBoxes);
				MergeDatesWithTransactions (blockEntries, blockDates);

				i = parsedBoxes;
				blockDates.Clear ();
				result.AddRange (blockEntries);
			}

			return result;
		}


		private void MergeDatesWithTransactions (IList<QifEntry> blockEntries, List<BankDates> blockDates)
		{
			if (blockEntries.Count != blockDates.Count)
				throw new ApplicationException ("Different number of entries");
			for (int i = 0; i < blockEntries.Count; i++)
				blockEntries [i].Date = blockDates [i];
		}


		private IList<QifEntry> TryParseTransactions (List<XElement> boxesList, int startIndex, int count, out int i)
		{
			var result = new List<QifEntry>();

			QifEntry current = null;
			i = startIndex;
			int found = 0;
			while (found < count) {
				var line = (string) boxesList [i].Elements ("line").Aggregate (strAgg);
				// check if current line is a standard operation name
				if (!IsOpLine (line)) 
					throw new ApplicationException (string.Format ("Wrong line: {0}", line));

				current = new QifEntry { Description = line };
				decimal amount = decimal.Parse ((string) boxesList [i + 1].Element ("line"));
				//double balance = double.Parse ((string) boxesList [i + 2].Element ("line"));
				var nextLine = (string) boxesList [i + 3].Elements ("line").Aggregate (strAgg);
				current.Amount = amount;
				result.Add (current);
				found++;

				if (IsOpLine (nextLine)) {
					i += 3;
					continue;
				}
				current.Description += " " + nextLine;
				i += 4;
			}

			if (count != result.Count)
				throw new ApplicationException ("Parsing error, wrong number of entries parsed");

			return result;
		}


		private bool IsOpLine (string line) 
		{
			return OpNames.Any (s => line.StartsWith (s));
		}


		public static void Main (string[] args)
		{
            ExtractWithITextSharpTest();
            //SpracheCase();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER>...");
            Console.ReadLine();
		}

        private static void SpracheCase () 
        {
            Sprache.Start();
        }
              

        
        public static void ExtractWithITextSharpTest ()
        {
            PdfReader reader = new PdfReader(@"../../../Sync2QifTests/data/wyciag1.pdf");

            IEnumerable<string> lines = new List<string>();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                string input = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());                
                lines = lines.Concat(TrimStrings(input, i == 1));                
            }

            
            //foreach (var line in lines)
            //{                
            //    Console.WriteLine(line);
            //}
            Sprache parser = new Sprache();

            if (!OpNames.Any(s => lines.ElementAt(0).Contains(s)))
                throw new ApplicationException("Bad first line");
            var current = new List<string>();// { lines.ElementAt(0) };
            var entries = new List<QifEntry>();

            foreach (var line in lines)
            {
                // if it is a line starting with correct operation name and if it is a subsequent operation line
                if (OpNames.Any(s => line.Contains(s)) && current.Count > 1)
                {
                    var total = current.Aggregate((s1, s2) => string.Format("{0}{1}{2}", s1, Environment.NewLine, s2));
                    entries.Add (parser.ParseSingleToQif(total));
                    current.Clear();
                }
                current.Add(line);
            }
            var lastOne = current.Aggregate((s1, s2) => string.Format("{0}{1}{2}", s1, Environment.NewLine, s2));
            entries.Add(parser.ParseSingleToQif(lastOne));

            foreach (var e in entries)
            {
                Console.WriteLine("==========================");
                Console.WriteLine(e);
                Console.WriteLine("==========================");
            }
        }


        public static IEnumerable<string> TrimStrings(string pdfInput, bool isFirstPage = false)
        {
            var strings = pdfInput.Split(new char [] {'\n'}, StringSplitOptions.RemoveEmptyEntries).ToList ();

            var startDelim = isFirstPage ? FIRST_PAGE_START : NEXT_PAGE_START;
            var endDelim = isFirstPage ? FIRST_PAGE_END : NEXT_PAGE_END;

            int firstId = strings.IndexOf(strings.Where(s => s.StartsWith(startDelim)).Single());
            int lastId = strings.IndexOf(strings.Where(s => s.StartsWith(endDelim)).Single());

            return strings.Where((s, i) => i > firstId && i <lastId);
        }

        private static readonly string FIRST_PAGE_START = @"DATA OPERACJI OPERACJI OPERACJI KSIĘGOWE";
        private static readonly string FIRST_PAGE_END = @"Infolinia Alior Sync dostępna jest przez całą dobę pod numerami 19 506";
        private static readonly string NEXT_PAGE_START = @"Wyciąg z rachunku bankowego";
        private static readonly string NEXT_PAGE_END = @"Niniejszy dokument jest wydrukiem komputerowym sporządzonym zgodnie z art. 7";

	}
}
