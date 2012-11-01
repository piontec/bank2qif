using System;
using Sync2Qif;
using System.Xml.Linq;
using System.Data.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;

namespace Sync2QifPlayground
{
	class MainClass
	{
		private readonly Func<XElement, XElement, XElement> strAgg = (e1, e2) => new XElement ("line", (string)e1 + (string)e2);
		private readonly string [] OpNames = new string [] 
		{
			"PRZELEW W RAMACH BANKU NA RACH OBCY", 
			"PRZELEW DO INNEGO BANKU KRAJOWEGO",
			"PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO",
			"PRZELEW NATYCHMIASTOWY",
			"ZAŁOŻENIE LOKATY",
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
				double amount = double.Parse ((string) boxesList [i + 1].Element ("line"));
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
			Console.WriteLine ("Loading file");
			var cult = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("pl-PL");
			var xdoc = PdfToXmlReader.Read (@"../../../Sync2QifTests/data/wyciag1.pdf");

			IEnumerable<XElement> pages = from page in xdoc.Descendants("page")
				select page;
			var m = new MainClass ();

			var page1 = (from p in pages
				where (int) p.Attribute ("id") == 1
			    select p).Single ();
			var boxes = m.ExtractBoxes (page1, 10);

			var nextPages = from p in pages
				where (int) p.Attribute ("id") > 1
				select p;

			foreach (var p in nextPages) 			
				boxes = boxes.Concat (m.ExtractBoxes (p, 1));

			var entries = m.ConvertBoxes (boxes);

			Thread.CurrentThread.CurrentCulture = cult;

			foreach (var entry in entries)
				Console.WriteLine (entry);

		}
	}
}
