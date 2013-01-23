using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sprache;

//Data,Nazwa odbiorcy/nadawcy,Rachunek,Tytuł płatności,Kwota
//01-12-2012,PayU S.A.,Konto osobiste,PayU XX269548709XX Oplata za zamowienie nr 2975,-108.80 PLN

namespace Bank2Qif.Parsers
{
	public static class AliorSyncCsvParsers
	{
		public static readonly Parser<decimal> Amount =
			from minus in Parse.String("-").Text().Or(Parse.Return(string.Empty))
			from whole in Parse.Number
			from separator in Parse.Char('.').Once()
			from pointPart in Parse.Number
			let strDecimal = string.Format("{0}{1}.{2}", minus, whole, pointPart)
			select decimal.Parse(strDecimal, CultureInfo.InvariantCulture);

		public static readonly Parser<QifEntry> QifEntryParser =
			from date in GenericParsers.DateMmDdYyyy
			from sep1 in Parse.Char(',').Once ()
			from receiver in Parse.AnyChar.Until (Parse.Char(',').Once ()).Text ()
			from accountName in Parse.AnyChar.Until (Parse.Char(',').Once ()).Text ()
			from desc in Parse.AnyChar.Until (Parse.Char(',').Once ()).Text ()
			from amount in Amount
			from trailing in Parse.String (" PLN\n")
			select new QifEntry
			{
				AccountName = accountName,
				Amount = amount,
				Date = new BankDates { OperationDate = date, BookingDate = date },
				Payee = receiver,
				Description = desc
			};
		
		public static readonly Parser<IEnumerable<QifEntry>> QifEntriesParser =
			from firstLine in Parse.AnyChar.Until (Parse.Char('\n').Once ())
			from entries in QifEntryParser.Many().End()
			select entries;	
	}
}

