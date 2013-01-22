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

//Data,Nazwa odbiorcy/nadawcy,Rachunek,Tytuł płatności,Kwota
//01-12-2012,PayU S.A.,Konto osobiste,PayU XX269548709XX Oplata za zamowienie nr 2975,-108.80 PLN


namespace Bank2Qif.Converters
{
	[Converter ("sync", "csv")]	
	public class AliorSyncCsvToQif : IConverter
	{
		public AliorSyncCsvToQif ()
		{
		}

		#region IConverter implementation

		public IEnumerable<QifEntry> ConvertFileToQif (string fileName)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

