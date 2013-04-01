using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;

namespace Bank2Qif.Converters.MBank
{
    //#Data operacji;#Data księgowania;#Opis operacji;#Tytuł;#Nadawca/Odbiorca;#Numer konta;#Kwota;#Saldo po operacji;
    //2012-01-01;2012-01-01;PRZELEW MTRANSFER WYCHODZACY;"PLACE Z ALLEGRO XX111100261XX WPŁATA ŁĄCZNA OD XXX";"PAYU SPÓŁKA AKCYJNA  UL.MARCELIŃSKA 90                  60-324 POZNAŃ POLSKA";'81114020040000330261746759';-10,50;1,10;
    //2011-12-30;2012-01-01;ZAKUP PRZY UŻYCIU KARTY;"STACJA WIELKOPOLSKA/POZNAN";"  ";'';-10,95;1,50;
    public static class MBankCsvParsers
    {        
        public static readonly Parser<decimal> Amount =
            from minus in Parse.String("-").Text().Or(Parse.Return(string.Empty))
            from triples in GenericParsers.OptionalSpaceThenNumber.Token().Many()
            from separator in Parse.Char(',').Once()
            from pointPart in Parse.Number.Once()
            let strDecimal = string.Format("{0}{1}.{2}", minus, triples.Aggregate((s1, s2) => s1 + s2),
                pointPart.Single())
            select decimal.Parse(strDecimal, CultureInfo.InvariantCulture);
    }
}
