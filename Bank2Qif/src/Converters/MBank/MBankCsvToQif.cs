using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Sprache;
using Bank2Qif.Parsers;

namespace Bank2Qif.Converters.MBank
{
    [Converter("mbank", "csv")]
    public class MBankCsvToQif : BaseConverter
    {
        private const int MBANK_HEADER_LENGTH = 37;
        private const int MBANK_FOOTER_LENGTH = 5;
        //#Data operacji;#Data księgowania;#Opis operacji;#Tytuł;#Nadawca/Odbiorca;#Numer konta;#Kwota;#Saldo po operacji;
        //2012-01-01;2012-01-01;PRZELEW MTRANSFER WYCHODZACY;"PLACE Z ALLEGRO XX111100261XX WPŁATA ŁĄCZNA OD XXX";"PAYU SPÓŁKA AKCYJNA  UL.MARCELIŃSKA 90                  60-324 POZNAŃ POLSKA";'81114020040000330261746759';-10,50;1,10;
        //2011-12-30;2012-01-01;ZAKUP PRZY UŻYCIU KARTY;"STACJA WIELKOPOLSKA/POZNAN";"  ";'';-10,95;1,50;
        public override IEnumerable<QifEntry> ConvertLinesToQif(string lines)
        {
            var filteredCsvEntries = CsvParser.CsvSemicolon.Parse(lines).Skip (MBANK_HEADER_LENGTH);
            filteredCsvEntries = filteredCsvEntries.Take(filteredCsvEntries.Count() - MBANK_FOOTER_LENGTH);

            var entries = from csvline in filteredCsvEntries
                     let csv = csvline.ToArray()
                     let opDate = GenericParsers.DateYyyyMmDd.Parse(csv[0])
                     let bookingDate = GenericParsers.DateYyyyMmDd.Parse(csv[1])
                     let opType = csv[2]
                     let description = csv [3]
                     let rcvr = csv [4]
                     let rcvrAcc = csv [5]
                     let amount = MBankCsvParsers.Amount.Parse(csv [6])                     
                     select new QifEntry
                         {
                             AccountName = rcvrAcc.Trim(new char [] {'\'', ' '}),
                             Amount = amount,
                             Date = new BankDates { OperationDate = opDate, BookingDate = bookingDate },
                             Payee = rcvr.Trim('"').Trim(' '),
                             Description = description.Trim() == string.Empty ?
                                 string.Format("{0}", opType) :
                                 string.Format("{0} - {1}", description, opType)
                         };

            Normalize(entries);
            return entries;
        }

        private static void Normalize(IEnumerable<QifEntry> entries)
        {
            TextInfo myTI = new CultureInfo("pl-PL", false).TextInfo;

            string pattern = @"Xx(?<num>\d+)xx";
            string replacement = "XX${num}XX";
            foreach (var entry in entries)
            {
                // normalize many whitespaces into one, do camel casing
                entry.Description = System.Text.RegularExpressions.Regex.Replace(
                    myTI.ToTitleCase(entry.Description.ToLower()), @"\s+", " ");
                // fix PayU IDs
                entry.Description = System.Text.RegularExpressions.Regex.Replace(
                    entry.Description, pattern, replacement);
                // normalize PayU descriptions
                entry.Description = System.Text.RegularExpressions.Regex.Replace(
                    entry.Description, "Payu", "PayU");
                // normalize PayU allegro descriptions
                entry.Description = System.Text.RegularExpressions.Regex.Replace(
                    entry.Description, "PayU Na Allegro", "PayU w Allegro");
                entry.Payee = System.Text.RegularExpressions.Regex.Replace(
                    myTI.ToTitleCase(entry.Payee.ToLower()), @"\s+", " ");
            }
        }
    }
}
