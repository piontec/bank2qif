﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.MBank
{
    public static class Extensions
    {
        // if this ever failes and results in a bug, scrap it and replace with plain \" removal
        private static readonly Regex s_incorrectCitationMarks = new Regex ("(?<start>.*;\".*?)((?<c1>\")(?<c>[^;]+?)(?<c2>\")(?<r>.*?))+(?<end>.*?\";.*)");


        public static IEnumerable<string> RemoveIncorrectCitationMarks (this IEnumerable<string> source)
        {
            foreach (var item in source)
            {
                var r = s_incorrectCitationMarks.Matches (item);
                if (r.Count == 0)
                    yield return item;
                else
                    yield return s_incorrectCitationMarks.Replace (item, "${start}${c}${r}${end}");
            }
        }
    }

    [Converter ("mbank", "csv")]
    public class MBankCsvToQif : BaseConverter
    {
        public const int MBANK_HEADER_LENGTH = 38;
		public const int MBANK_FOOTER_LENGTH = 5;
        //#Data operacji;#Data księgowania;#Opis operacji;#Tytuł;#Nadawca/Odbiorca;#Numer konta;#Kwota;#Saldo po operacji;
        //2012-01-01;2012-01-01;PRZELEW MTRANSFER WYCHODZACY;"PLACE Z ALLEGRO XX111100261XX WPŁATA ŁĄCZNA OD XXX";"PAYU SPÓŁKA AKCYJNA  UL.MARCELIŃSKA 90                  60-324 POZNAŃ POLSKA";'81114020040000330261746759';-10,50;1,10;
        //2011-12-30;2012-01-01;ZAKUP PRZY UŻYCIU KARTY;"STACJA WIELKOPOLSKA/POZNAN";"  ";'';-10,95;1,50;
        public override IList<QifEntry> ConvertLinesToQif (string lines)
        {
            var transactionLines = lines.Split (new[] {"\r\n"}, StringSplitOptions.None);
            // skip header and footer of mbank's log and remove incorrect citation marks
            var filteredCsvEntries = transactionLines.AsEnumerable ()
                                                     .Skip (MBANK_HEADER_LENGTH)
                                                     .Take (transactionLines.Length - MBANK_HEADER_LENGTH - MBANK_FOOTER_LENGTH)
                                                     .RemoveIncorrectCitationMarks ()
                                                     .Select (line => CsvParser.CsvRecordSemicolon.Parse (line));

            var entries = from csvline in filteredCsvEntries
                          let csv = csvline.ToArray ()
                          let opDate = GenericParsers.DateYyyyMmDd.Parse (csv [0])
                          let bookingDate = GenericParsers.DateYyyyMmDd.Parse (csv [1])
                          let opType = csv [2]
                          let description = csv [3]
                          let rcvr = csv [4]
                          let rcvrAcc = csv [5]
                          let amount = MBankCsvParsers.Amount.Parse (csv [6])
                          let verfiedAccName = rcvrAcc.Trim (new[] {'\'', ' '})
                          let newDescription = description.Trim () == string.Empty
                                                   ? string.Format ("{0}", opType)
                                                   : string.Format ("{0} - {1}", description, opType)
                          select new QifEntry
                                     {
                                         Amount = amount,
                                         Date = new BankDates {OperationDate = opDate, BookingDate = bookingDate},
                                         Payee = rcvr.Trim ('"').Trim (' '),
                                         Description = String.IsNullOrEmpty (verfiedAccName)
                                                           ? newDescription
                                                           : String.Format ("[{0}] {1}", verfiedAccName, newDescription)
                                     };

            var result = entries.ToList ();
            Normalize (result);

            return result;
        }


        private static void Normalize (IEnumerable<QifEntry> entries)
        {
            TextInfo myTi = new CultureInfo ("pl-PL", false).TextInfo;

            string pattern = @"Xx(?<num>\d+)xx";
            string replacement = "XX${num}XX";
            foreach (var entry in entries)
            {
                // normalize many whitespaces into one, do camel casing
                entry.Description = Regex.Replace (
                    myTi.ToTitleCase (entry.Description.ToLower ()), @"\s+", " ");
                // fix PayU IDs
                entry.Description = Regex.Replace (
                    entry.Description, pattern, replacement);
                // normalize PayU descriptions
                entry.Description = Regex.Replace (
                    entry.Description, "Payu", "PayU");
                // normalize PayU allegro descriptions
                entry.Description = Regex.Replace (
                    entry.Description, "PayU Na Allegro", "PayU w Allegro");
                entry.Payee = Regex.Replace (
                    myTi.ToTitleCase (entry.Payee.ToLower ()), @"\s+", " ");
            }
        }


        public override Encoding GetEncoding ()
        {
            return Encoding.GetEncoding ("windows-1250");
        }
    }
}