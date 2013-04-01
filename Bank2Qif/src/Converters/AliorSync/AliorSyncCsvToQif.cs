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
    //01-12-2012,PayU S.A.,Konto osobiste,PayU XX269548709XX Oplata za zamowienie nr 2975,-108.80 PLN

    [Converter("sync", "csv")]
    public class AliorSyncCsvToQif : IConverter
    {
        #region IConverter implementation

        public IEnumerable<QifEntry> ConvertFileToQif(string fileName)
        {
            string lines = File.ReadAllText(fileName, System.Text.Encoding.Default);

            var entries = from csvline in CsvParser.Csv.Parse(lines).Skip(1)
                          let csv = csvline.ToArray()
                          let opDate = GenericParsers.DateMmDdYyyy.Parse(csv[0])
                          let rcvr = csv[1]
                          let accountName = csv [2]
                          let desc = csv [3]
                          let amount = AliorSyncCsvParsers.Amount.Parse(csv[4])
                          select new QifEntry
                          {
                              AccountName = accountName,
                              Amount = amount,
                              Date = new BankDates { OperationDate = opDate, BookingDate = opDate },
                              Payee = rcvr,
                              Description = desc
                          };

            return entries;
        }

        #endregion
    }
}

