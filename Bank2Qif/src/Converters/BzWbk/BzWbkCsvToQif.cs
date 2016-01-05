using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.BzWbk
{
//	2016-01-05,18-11-2015,'11 2222 2222 2222 2222 2222 2222,Pan JAN KOWALSKI ŻADNA 1/2 11-222 PCIM,PLN,"0,00","1111,11",11,
//	05-01-2016,04-01-2016,Opłata miesięczna za kartę 11111******1111,,,"-1,00","1111,11",1,
//	25-11-2015,24-11-2015,TO JEST OPIS,TO JEST NADAWCA I ADRES ELIXIR 24-11-2015,11 2222 2222 2222 2222 2222 2222,"1111,00","1111,00",2,

	[Converter("bzwbk", "csv")]
	public class BzWbkCsvToQif : BaseConverter
	{
		public override IList<QifEntry> ConvertLinesToQif (string lines)
		{
			var entries = from csvline in CsvParser.CsvComma.Parse (lines).Skip (1)
				let csv = csvline.ToArray ()
				let bookingDate = GenericParsers.DateDdMmYyyy.Parse (csv [0])
				let opDate = GenericParsers.DateDdMmYyyy.Parse (csv [1])
				let desc = csv [2]
				let payee = csv [3]  
				let account = csv[4]
				let amount = csv [5]
				select new QifEntry
			{
				Amount = Decimal.Parse(amount.Replace(",", ".")),
				Date = new BankDates {OperationDate = opDate, BookingDate = bookingDate},
				Payee = payee.Trim(),
				Description = desc.Trim(),
				AccountName = account
			};

			return entries.ToList ();
		}


		public override Encoding GetEncoding ()
		{
			return Encoding.UTF8;
		}
	}
}