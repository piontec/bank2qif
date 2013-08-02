using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bank2Qif.Parsers;
using Sprache;


namespace Bank2Qif.Converters.AliorSync
{
    public class AliorSyncPdfFirstLineResult
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }


        public override string ToString ()
        {
            return string.Format ("[ Date = {0}, Description = {1}, Amount = {2}, Balance = {3} ]",
                                  Date, Description, Amount, Balance);
        }


        public override bool Equals (object obj)
        {
            AliorSyncPdfFirstLineResult dst = obj as AliorSyncPdfFirstLineResult;
            if (dst == null)
                return false;
            return Date == dst.Date && Description == dst.Description && Amount == dst.Amount
                   && Balance == dst.Balance;
        }


        public override int GetHashCode ()
        {
            return Date.GetHashCode () ^ Description.GetHashCode () ^ Amount.GetHashCode ()
                   ^ Balance.GetHashCode ();
        }
    }

    public static class AliorSyncPdfParsers
    {
        public static readonly Parser<decimal> Amount =
            from minus in Parse.String ("-").Text ().Or (Parse.Return (string.Empty))
            from triples in GenericParsers.OptionalSpaceThenNumber.Token ().Many ()
            from separator in Parse.Char (',').Once ()
            from pointPart in Parse.Number.Once ()
            let strDecimal = string.Format ("{0}{1}.{2}", minus, triples.Aggregate ((s1, s2) => s1 + s2),
                                            pointPart.Single ())
            select decimal.Parse (strDecimal, CultureInfo.InvariantCulture);

        //FIXME: this is very very ugly as this list is also in AliorSyncPdfToQif, but don't have time
        // to fix it right now. AliorSyncPdfToQif has part of parser's logic - should be moved here.
        public static readonly Parser<string> OperationName =
            from trailing in Parse.WhiteSpace.Many ()
            from op in
                Parse.String ("PRZELEW W RAMACH BANKU NA RACH OBCY").Or (
                    Parse.String ("PRZELEW DO INNEGO BANKU KRAJOWEGO").Or (
                        Parse.String ("PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO").Or (
                            Parse.String ("PRZELEW NATYCHMIASTOWY").Or (
                                Parse.String ("ZAŁOŻENIE LOKATY").Or (
                                    Parse.String ("TRANSAKCJA KARTĄ DEBETOWĄ").Or (
                                        Parse.String ("PRZELEW WEWNĘTRZNY - PŁACĘ Z ALIOR BANKIEM").Or (
                                            Parse.String ("KAPITALIZACJA ODSETEK")))))))).Text ()
            from ending in Parse.WhiteSpace.Many ()
            select op;

        public static readonly Parser<AliorSyncPdfFirstLineResult> FirstLineParser =
            from date in GenericParsers.DateYyyyMmDd
            from operation in OperationName
            from amount in Amount
            from balance in Amount
            select new AliorSyncPdfFirstLineResult {Date = date, Amount = amount, Balance = balance, Description = operation};

        public static readonly Parser<QifEntry> QifEntryParser =
            from firstLine in FirstLineParser
            from nl1 in GenericParsers.NewLine
            from secondDate in GenericParsers.DateYyyyMmDd
            //FIXME: when adding Trim () below, the next line w account number stops parsing
            from desc2 in Parse.AnyChar.Until (GenericParsers.NewLine).Text ()
            from accNum in GenericParsers.AccountNumberParser.Or (Parse.Return (new AccountNumber (string.Empty)))
            from desc3 in Parse.AnyChar.Many ().Text ().Token ()
            select new QifEntry
                       {
                           AccountName = accNum.Number,
                           Amount = firstLine.Amount,
                           Date = new BankDates {OperationDate = secondDate, BookingDate = firstLine.Date},
                           Payee = accNum.Number,
                           Description = desc2 == string.Empty
                                             ? string.Format ("{0}: {1}", firstLine.Description, desc3).Replace (Environment.NewLine, " ")
                                             : string.Format ("{0} {1}: {2}", firstLine.Description, desc2.Trim (), desc3)
                                                     .Replace (Environment.NewLine, " ")
                       };

        public static readonly Parser<IEnumerable<QifEntry>> QifEntriesParser =
            from entries in QifEntryParser.Many ().End ()
            select entries;
    }
}