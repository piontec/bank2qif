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


namespace Bank2Qif.Converters
{
	[Converter ("sync", "csv")]	
	public class AliorSyncCsvToQif : IConverter
	{
		#region IConverter implementation

		public IEnumerable<QifEntry> ConvertFileToQif (string fileName)
		{
			return TemporaryHack (fileName);
			
			//string lines = File.ReadAllText(fileName, System.Text.Encoding.UTF8);

			//IEnumerable<QifEntry> result = AliorSyncCsvParsers.QifEntriesParser.Parse (lines);

			//return result;
		}

		#endregion
//Data,Nazwa odbiorcy/nadawcy,Rachunek,Tytuł płatności,Kwota
//01-12-2012,PayU S.A.,Konto osobiste,PayU XX269548709XX Oplata za zamowienie nr 2975,-108.80 PLN

		IEnumerable<QifEntry> TemporaryHack (string fileName)
		{
			var lines = File.ReadAllLines(fileName, System.Text.Encoding.UTF8);

			lines = lines.AsEnumerable ().Skip (1).ToArray ();

			var result = from line in lines
				let cols = line.Split (',')
				let date = DateTime.Parse (cols [0])
				select new QifEntry
				{
					Date = new BankDates { OperationDate = date, BookingDate = date },
					Payee = cols [1],
					AccountName = cols [2],
					Description = cols [3],
					Amount = decimal.Parse (cols [4].Substring (0, cols[4].IndexOf (" PLN")),
						                        CultureInfo.InvariantCulture)
				};

			return result;
		}
	}
}

