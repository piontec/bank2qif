using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sprache;

namespace Bank2Qif.Parsers
{
    public class FirstLineResult
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }

        public override string ToString()
        {
            return string.Format("[ Date = {0}, Description = {1}, Amount = {2}, Balance = {3} ]",
                Date, Description, Amount, Balance);
        }
    }

    public static class AliorSyncParsers
    {
        public static readonly Parser<decimal> Amount =
                from minus in Parse.String("-").Text().Or(Parse.Return(string.Empty))
                from triples in GenericParsers.OptionalSpaceThenNumber.Token().Many()
                from separator in Parse.Char(',').Once()
                from pointPart in Parse.Number.Once()
                let strDecimal = string.Format("{0}{1}.{2}", minus, triples.Aggregate((s1, s2) => s1 + s2),
                    pointPart.Single())
                select decimal.Parse(strDecimal, CultureInfo.InvariantCulture);

        public static readonly Parser<string> UpperString =
            Parse.Upper.Or(Parse.Char(' ')).Or(Parse.Char('-')).Many().Text().Token();

        public static readonly Parser<FirstLineResult> FirstLineParser =
            from date in GenericParsers.DateYyyyMmDd
            from desc in UpperString
            from amount in Amount
            from balance in Amount
            select new FirstLineResult { Date = date, Amount = amount, Balance = balance, Description = desc };

        public static readonly Parser<QifEntry> QifEntryParser =
            from firstLine in FirstLineParser
            from nl1 in GenericParsers.NewLine
            from secondDate in GenericParsers.DateYyyyMmDd
            from desc2 in UpperString.Or(Parse.Return(string.Empty))
            //from nl2 in NewLine
            from accNum in GenericParsers.AccountNumberParser.Or(Parse.Return(new AccountNumber(string.Empty)))
            from desc3 in Parse.AnyChar.Many().Text().Token()
            select new QifEntry
            {
                AccountName = accNum.Number,
                Amount = firstLine.Amount,
                Date = new BankDates { OperationDate = firstLine.Date, BookingDate = secondDate },
                Payee = accNum.Number,
                Description = string.Format("{1} {2}: {3}", firstLine.Description, desc2, desc3)
            };

        public static readonly Parser<IEnumerable<QifEntry>> QifEntriesParser =
            from entries in QifEntryParser.Many().End()
            select entries;
    }
}
