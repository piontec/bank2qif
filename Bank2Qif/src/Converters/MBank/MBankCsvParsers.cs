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
        private static readonly string START_LINE = "#Data operacji;#Data księgowania;#Opis operacji;#Tytuł;#Nadawca/Odbiorca;#Numer konta;#Kwota;#Saldo po operacji;\n";
        public static readonly Parser<decimal> Amount =
            from minus in Parse.String("-").Text().Or(Parse.Return(string.Empty))
            from whole in Parse.Number
            from separator in Parse.Char(',').Once()
            from pointPart in Parse.Number
            let strDecimal = string.Format("{0}{1}.{2}", minus, whole, pointPart)
            select decimal.Parse(strDecimal, CultureInfo.InvariantCulture);

        public static readonly Parser<QifEntry> QifEntryParser =
            from dateTr in GenericParsers.DateYyyyMmDd
            from sep1 in Parse.Char(';').Once()
            from dateBook in GenericParsers.DateYyyyMmDd
            from sep2 in Parse.Char(';').Once()
            from type in Parse.AnyChar.Until(Parse.Char(';').Once()).Text()
            from desc in Parse.AnyChar.Until(Parse.Char(';').Once()).Text()
            from receiver in Parse.AnyChar.Until(Parse.Char(';').Once()).Text()
            from receiverAcc in Parse.AnyChar.Until(Parse.Char(';').Once()).Text()
            from amount in Amount
            from sep3 in Parse.Char(';').Once()
            from balance in Amount
            from trailing in Parse.String(";\n")
            select new QifEntry
            {
                AccountName = receiverAcc.Trim ('\''),
                Amount = amount,
                Date = new BankDates { OperationDate = dateTr, BookingDate = dateBook },
                Payee = receiver.Trim ('\"'),
                Description = string.Format ("{0} - {1}", desc, type)
            };

        public static readonly Parser<IEnumerable<QifEntry>> QifEntriesParser =
            from firstLine in Parse.AnyChar.Until(Parse.String (START_LINE))
            from entries in QifEntryParser.Many().End()
            select entries;	
    }
}
