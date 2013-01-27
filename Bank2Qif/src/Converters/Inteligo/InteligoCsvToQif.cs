using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sprache;
using Bank2Qif.Parsers;

namespace Bank2Qif.Converters.Inteligo
{
    [Converter("inteligo", "csv")]
    public class InteligoCsvToQif : IConverter
    {
        public IEnumerable<QifEntry> ConvertFileToQif(string fileName)
        {
            string lines = File.ReadAllText(fileName, System.Text.Encoding.Default);
            
            var result = from csvline in CsvParser.Csv.Parse(lines).Skip (1)
                     let csv = csvline.ToArray()
                     let bookingDate = GenericParsers.DateYyyyMmDd.Parse(csv[1])
                     let opDate = GenericParsers.DateYyyyMmDd.Parse(csv[2])
                     let opType = csv[3]
                     let amount = InteligoCsvParsers.Amount.Parse(csv[4])
                     let payee = csv[8]
                     // sometimes csv [8] == csv [9] - in this case skip csv [9]
                     let desc = csv.Where((s, i) => i == 7 || (i > 8 && s != csv[8])).
                        Aggregate((s1, s2) => string.Format("{0} {1}", s1, s2))
                     select new QifEntry
                     {
                         Amount = amount,
                         Date = new BankDates { OperationDate = opDate, BookingDate = bookingDate },
                         Payee = payee,
                         Description = desc
                     };

            return result;
        }
    }
}
