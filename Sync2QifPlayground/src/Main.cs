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

			Func<XElement, XElement, XElement> strAgg = (e1, e2) => new XElement ("Line", (string) e1 + (string) e2);
			int lastBoxId = int.Parse (
				(from b in page1.Elements ("textbox")
				let firstLn = b.Elements ("textline").First ()
				let firstLnTxt = (string) firstLn.Elements ("text").Aggregate (strAgg)
				where firstLnTxt.StartsWith ("Infolinia")
				select b.Attribute ("id").Value)
				.First ()
				);
			var boxes = from b in page1.Elements ("textbox")
				let bid = (int)b.Attribute ("id")
				where  bid > 10 && bid < lastBoxId
				select b;
			var lines = from b in boxes
				select new XElement ("box",
		             from tl in b.Elements ("textline")
				     select tl.Elements ("text").Aggregate (strAgg)		             
				     );

			foreach (var line in lines)
				Console.WriteLine (line);

			/*
			IEnumerable<XElement> lines = from ln in xdoc.Descendants("textline")
				select	(from w in ln.Elements("text")
				        select w).Aggregate ((e1, e2) => 
				                    {
					string s1 = (string) e1;
					return s1 == string.Empty ?
						new XElement ("Line", " " + (string) e2)
							: new XElement ("Line", s1 + (string) e2);
									})
				        ;
			foreach (var line in lines)
				Console.WriteLine (line);
*/
			/*
			var lines = from ln in xdoc.Descendants ("textline")
				select	new {List = ln.Elements ("text")}
					;
			foreach (var line in lines)
				foreach (var a in line.List)
					Console.WriteLine (a);
					*/
		}
	}
}
