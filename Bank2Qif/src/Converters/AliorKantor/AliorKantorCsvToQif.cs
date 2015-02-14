using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;
using Bank2Qif.Converters.MBank;


namespace Bank2Qif.Converters.AliorKantor
{
    //Data księgowania ;Data efektywna;Nazwa transakcji i opis;Kwota;Waluta;Saldo po operacji
    //2014-12-30 00:00:00;2014-12-30 00:00:00;"Rozliczenie transakcji Kantor Walutowy 1111";-11,11;"PLN";111,11
    //2014-12-16 00:00:00;2014-12-16 00:00:00;"pod usd   ";10;"PLN";11,1

    [Converter("aliorkantor", "csv")]
    public class AliorKantorCsvToQif : BaseConverter
    {
        public override IList<QifEntry> ConvertLinesToQif (string lines)
        {
            var entries = from csvline in CsvParser.CsvSemicolon.Parse (lines).Skip (1)
                          let csv = csvline.ToArray ()
                          let bookingDate = GenericParsers.DateYyyyMmDd.Parse (csv [0])
                          let opDate = GenericParsers.DateYyyyMmDd.Parse (csv [1])
                          let desc = csv [2]
                          let amount = MBankCsvParsers.Amount.Parse(csv[3])
                          let payee = csv [4]                          
                          select new QifEntry
                                     {
                                         Amount = amount,
                                         Date = new BankDates {OperationDate = opDate, BookingDate = bookingDate},
                                         Payee = payee,
                                         Description = desc
                                     };

            return entries.ToList ();
        }


        public override Encoding GetEncoding ()
        {
            return Encoding.GetEncoding ("windows-1250");
        }
    }
}