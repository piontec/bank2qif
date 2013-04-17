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
        public static readonly Parser<Tuple<decimal, string>> Amount =
            from minus in Parse.String("-").Text().Or(Parse.Return(string.Empty))
            from whole in Parse.Number
            from separator in Parse.Char('.').Once()
            from pointPart in Parse.Number
            from rest in Parse.Upper.Many ().Token().Text()
            let strDecimal = string.Format("{0}{1}.{2}", minus, whole, pointPart)
            select new Tuple<decimal, string> (decimal.Parse(strDecimal, CultureInfo.InvariantCulture), rest);

        //"Transakcja kartą debetową;obciążenie; Kurs wymiany: 5.079; Kwota w walucie rozliczeniowej: 1.90 GBP 5969 LUX AMAZON.CO.UK Amazon EU"
        public static readonly Parser<decimal> ExchangeRate =
            from leading in Parse.AnyChar.Until(Parse.String ("Kurs wymiany: ")).Text()
            from rate in Amount
            select rate.Item1;
	}
}

