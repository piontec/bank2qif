using System;
using Sync2Qif;
using System.Xml.Linq;
using System.Data.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sync2QifPlayground
{
	class MainClass
	{
		private readonly string [] OpNames = new string [] 
		{
			"RZELEW W RAMACH BANKU NA RACH OBCY", 
			"PRZELEW DO INNEGO BANKU KRAJOWEGO",
			"PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO",
			"PRZELEW NATYCHMIASTOWY",
		};

		static IEnumerable<XElement> ExtractBoxes (IEnumerable<XElement> page, int firstBoxId)
		{
			Func<XElement, XElement, XElement> strAgg = (e1, e2) => new XElement ("line", (string)e1 + (string)e2);
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


		static IEnumerable<QifEntry> ConvertBoxes (IEnumerable<XElement> boxes)
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
				blockEntries = TryParseTransactions (boxesList, i, blockDates.Count);
				MergeDatesWithTransactions (blockEntries, blockDates);

				i += blockDates.Count;
				blockDates.Clear ();
				result.AddRange (blockEntries);
			}

			return result;
		}


		static void MergeDatesWithTransactions (object blockEntries, List<BankDates> blockDates)
		{
			throw new NotImplementedException ();
		}


		static IList<QifEntry> TryParseTransactions (List<XElement> boxesList, int startIndex, int count)
		{
			throw new NotImplementedException ();
		}


		public static void Main (string[] args)
		{
			Console.WriteLine ("Loading file");
			var xdoc = PdfToXmlReader.Read (@"../../../Sync2QifTests/data/wyciag1.pdf");
			//Console.WriteLine (xdoc);

			IEnumerable<XElement> pages = from page in xdoc.Descendants("page")
				select page;

			var page1 = from p in pages
				where (int) p.Attribute ("id") == 1
				select p;

			var boxes = ExtractBoxes (page1, 10);

			var trans = ConvertBoxes (boxes);

			foreach (var box in boxes)
				Console.WriteLine (box);

		}
	}
}
