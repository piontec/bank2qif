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


namespace Bank2Qif.Converters.AliorSync
{
    //Data,Nazwa odbiorcy/nadawcy,Rachunek,Tytu³ p³atnoœci,Kwota
    //01-12-2012,PayU S.A.,Konto osobiste,PayU XX112233444XX Oplata za zamowienie nr 1111,-100.80 PLN
    //01-01-2013,WWW.COM.COM,Konto osobiste,Transakcja kart¹ debetow¹;obci¹¿enie; Kurs wymiany: 3.4152; Kwota w walucie rozliczeniowej: 1.9 USD,-1.90 USD

    [Converter("sync", "csv")]
    public class AliorSyncCsvToQif : BaseConverter
    {
        #region IConverter implementation

        private const string DEFAULT_ACCOUNT_NAME = "Konto osobiste";
        private const string NATIVE_CURRENCY = "PLN";

        public override IList<QifEntry> ConvertLinesToQif(string lines)
        {            
            lines = lines.Replace("\n", System.Environment.NewLine);
            var entries = from csvline in CsvParser.CsvComma.Parse(lines).Skip(1)
                          let csv = csvline.ToArray()
                          let opDate = GenericParsers.DateMmDdYyyy.Parse(csv[0])
                          let rcvr = csv[1]
                          let accountName = csv [2]
                          let desc = csv [3]
                          let amount = AliorSyncCsvParsers.Amount.Parse(csv[4])
                          let verifiedAccName = accountName == DEFAULT_ACCOUNT_NAME ? "" : accountName
                          select new QifEntry
                          {                              
                              Amount = amount.Item2 == NATIVE_CURRENCY ?
                                amount.Item1 : ComputeValue (amount.Item1, desc),
                              Date = new BankDates { OperationDate = opDate, BookingDate = opDate },
                              Payee = rcvr,
                              Description = String.IsNullOrEmpty (verifiedAccName) ? desc : 
                                String.Format ("[{0}] {1}", verifiedAccName, desc)
                          };

            return entries.ToList ();
        }

        private decimal ComputeValue(decimal amount, string desc)
        {            
            return Math.Round (amount * AliorSyncCsvParsers.ExchangeRate.Parse(desc), 2);
        }

        #endregion

        public override Encoding GetEncoding()
        {
            return Encoding.UTF8;
        }
    }
}

