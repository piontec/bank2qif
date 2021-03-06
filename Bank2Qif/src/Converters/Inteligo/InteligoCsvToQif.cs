﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.Inteligo
{
	//"Id","Data księgowania","Data zlecona","Typ transakcji","Kwota","Waluta","Saldo po transakcji","Rachunek nadawcy/odbiorcy","Nazwa nadawcy/odbiorcy","Opis transakcji"
	//"1111","2012-01-11","2012-01-11","Przelew z rachunku","-100.00","PLN","200.00","11222233334444555566667777","XXXX YYYYY  ZZZZZ, WWWWWW ZIP City","XXXX YYYYY  ZZZZZ, WWWWWW ZIP City","XX XXXXXX: 11112222","XX XXXX.: 11222233334444555566667777","XXXXXX: XX XXXX XX XXXXX","XXXXX XXXXXX: 2012-01-11","XXXXXX XXXXXXXXXXXX"

    [Converter ("inteligo", "csv")]
    public class InteligoCsvToQif : BaseConverter
    {
        public override IList<QifEntry> ConvertLinesToQif (string lines)
        {
            var entries = from csvline in CsvParser.CsvComma.Parse (lines).Skip (1)
                          let csv = csvline.ToArray ()
                          let bookingDate = GenericParsers.DateYyyyMmDd.Parse (csv [1])
                          let opDate = GenericParsers.DateYyyyMmDd.Parse (csv [2])
                          let opType = csv [3]
                          let amount = InteligoCsvParsers.Amount.Parse (csv [4])
                          let account = csv [7]
                          let payee = csv [8]
                          // sometimes csv [8] == csv [9] - in this case skip csv [9]
                          let desc = csv.Where ((s, i) => i == 7 || (i > 8 && s != csv [8])).
                                         Aggregate ((s1, s2) => string.Format ("{0} {1}", s1, s2))
                          select new QifEntry
                                     {
                                         Amount = amount,
                                         Date = new BankDates {OperationDate = opDate, BookingDate = bookingDate},
                                         Payee = payee,
                                         Description = String.IsNullOrEmpty (account)
                                                           ? desc
                                                           : String.Format ("[{0}] {1}", account, desc)
                                     };

            return entries.ToList ();
        }


        public override Encoding GetEncoding ()
        {
            return Encoding.GetEncoding ("windows-1250");
        }
    }
}