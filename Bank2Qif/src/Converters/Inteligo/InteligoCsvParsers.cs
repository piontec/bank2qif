using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;

namespace Bank2Qif.Converters.Inteligo
{
    //"Id","Data księgowania","Data zlecona","Typ transakcji","Kwota","Waluta","Saldo po transakcji","Rachunek nadawcy/odbiorcy","Nazwa nadawcy/odbiorcy","Opis transakcji"
    //"1111","2012-01-11","2012-01-11","Przelew z rachunku","-100.00","PLN","200.00","11222233334444555566667777","XXXX YYYYY  ZZZZZ, WWWWWW ZIP City","XXXX YYYYY  ZZZZZ, WWWWWW ZIP City","XX XXXXXX: 11112222","XX XXXX.: 11222233334444555566667777","XXXXXX: XX XXXX XX XXXXX","XXXXX XXXXXX: 2012-01-11","XXXXXX XXXXXXXXXXXX"
    

    public static class InteligoCsvParsers
    {
        public static readonly Parser<decimal> Amount =
            from sign in Parse.String("-").Text().Or(Parse.String("+").Text().Or (Parse.Return(string.Empty)))
            from whole in Parse.Number
            from separator in Parse.Char('.')
            from pointPart in Parse.Number
            let strDecimal = string.Format("{0}{1}.{2}", sign, whole, pointPart)
            select decimal.Parse(strDecimal, CultureInfo.InvariantCulture);
    }
}
