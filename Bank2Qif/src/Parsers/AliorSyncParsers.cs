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

        public override bool Equals(object obj)
        {
            FirstLineResult dst = obj as FirstLineResult;
            if (dst == null)
                return false;
            return Date == dst.Date && Description == dst.Description && Amount == dst.Amount
                && Balance == dst.Balance;
        }

        public override int GetHashCode()
        {
            return Date.GetHashCode() ^ Description.GetHashCode() ^ Amount.GetHashCode()
                ^ Balance.GetHashCode();
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
            Parse.Upper.XOr(Parse.Char(' ')).XOr(Parse.Char('-')).Many().Text().Token ();

        public static readonly Parser<FirstLineResult> FirstLineParser =
            from date in GenericParsers.DateYyyyMmDd
            from desc in UpperString
            from amount in Amount
            from balance in Amount
            select new FirstLineResult { Date = date, Amount = amount, Balance = balance, Description = desc.Trim () };

        public static readonly Parser<QifEntry> QifEntryParser =
            from firstLine in FirstLineParser
            from nl1 in GenericParsers.NewLine
            from secondDate in GenericParsers.DateYyyyMmDd            
            from desc2 in UpperString.XOr(Parse.Return(string.Empty)).Text ().Token ()            
            from accNum in GenericParsers.AccountNumberParser.Or(Parse.Return(new AccountNumber(string.Empty)))
            from desc3 in Parse.AnyChar.Many().Text().Token()
            select new QifEntry
            {
                AccountName = accNum.Number,
                Amount = firstLine.Amount,
                Date = new BankDates { OperationDate = secondDate, BookingDate = firstLine.Date },
                Payee = accNum.Number,
                Description = string.Format("{0} {1}: {2}", firstLine.Description, desc2, desc3)
                                .Replace (Environment.NewLine, " ")
            };

        public static readonly Parser<IEnumerable<QifEntry>> QifEntriesParser =
            from entries in QifEntryParser.Many().End()
            select entries;
    }
}
