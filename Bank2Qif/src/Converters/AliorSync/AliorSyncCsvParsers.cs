using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;

//Data,Nazwa odbiorcy/nadawcy,Rachunek,Tytuł płatności,Kwota
//01-12-2012,PayU S.A.,Konto osobiste,PayU XX269548709XX Oplata za zamowienie nr 2975,-108.80 PLN

namespace Bank2Qif.Converters.AliorSync
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
	}
}

