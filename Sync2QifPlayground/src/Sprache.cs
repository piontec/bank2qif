using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sprache;
using Sync2Qif;

namespace Sync2QifPlayground.src
{
    class Sprache
    {
        private class AccountNumber
        {
            public AccountNumber(string num)
            {
                //TODO: validation
                Number = num;
            }

            public string Number { get; private set; }
        }

        private class FirstLineResult
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

        private readonly string line1 = "2012.09.03 PRZELEW W RAMACH BANKU NA RACH OBCY 0,10 0,10";
        private readonly string line2 = 
@"2012.09.28 PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO 13 000,00 18 193,09
2012.09.28 BANKU
86 1950 0001 3006 0057 6130 0075 PI?TKOWSKI ŁUKASZ ul.
os. Rusa 8810 61-245 POZNAŃ Lokata nr DP\12829949 -
zerwanie Data transakcji: 2012-09-28";
        private readonly string line3 =
@"2012.10.02 TRANSAKCJA KARTĄ DEBETOWĄ -142,99 50,10
2012.09.28
TESCO PL GHS 31006 POZNAN POL  5102 XXXX XXXX 2892 Data
transakcji: 2012-09-28 Kurs wymiany:   Kwota w walucie
rozliczeniowej:   Kod MCC: 5411 Informacje dodatkowe:
OTHER131006-1321686580 Kwota i waluta oryginalna transakcji:
142.99PLN";
        private readonly string line4 =
"2012.10.12 PRZELEW WEWNĘTRZNY - PŁACĘ Z ALIOR BANKIEM -67,00 980,10\r\n2012.10.12\r\n08 2490 0005 0000 4600 9079 6744 PayU S.A. ul. Marcelińska\r\n90/ 60-324 Poznań Pay by link PayU w Allegro XX255709781XX\r\npionte c aukcja nr (2693753978)";

        public static void Start()
        {
            var s = new Sprache();
            s.Sprache1();
        }

        public void Sprache1()
        {   
            Console.WriteLine(Date.Parse(line1).ToShortDateString ());            
            Console.WriteLine(Amount.Parse("123 451 100 ,10"));
            Console.WriteLine(AccountNumberParser.Parse("86 1950 0001 3006 0057 6130 0075").Number);
            Console.WriteLine(FirstLineParser.Parse(line1));
            Console.WriteLine("=================================");

            var res = QifEntriesParser.Parse(line2);
            Console.WriteLine(res.First());
            Console.WriteLine("=================================");

            var r = QifEntriesParser.Parse(line3);
            Console.WriteLine(r.First ());
            Console.WriteLine("=================================");

            var r2 = QifEntryParser.Parse(line4);
            Console.WriteLine(r2);
            Console.WriteLine("=================================");
        }

        public IEnumerable<QifEntry> ParseToQif(string lines)
        {
            return QifEntriesParser.Parse(lines);
        }

        public QifEntry ParseSingleToQif(string lines)
        {
            return QifEntryParser.Parse(lines);
        }

        static readonly Parser<string> NewLine = Parse.String(Environment.NewLine).Text();

        static readonly Parser<string> TwoDigits =
            from dig1 in Parse.Digit
            from dig2 in Parse.Digit
            select string.Format ("{0}{1}", dig1, dig2);

        static readonly Parser<string> ThreeDigits =
            from firstTwo in TwoDigits
            from dig3 in Parse.Digit
            select string.Format("{0}{1}", firstTwo, dig3);

        //static readonly Parser<string> UpToThreeDigits =
        //    from dig1 in Parse.Digit
        //    from dig2 in Parse.Digit. XOr (Parse.Return (""))
        //    from dig3 in Parse.Digit
        //    select string.Format("{0}{1}{2}", dig1, dig2, dig3);

        static readonly Parser<string> FourDigits =
            from firstThree in ThreeDigits
            from dig4 in Parse.Digit
            select string.Format("{0}{1}", firstThree, dig4);

        static readonly Parser<string> OptionalSeparator =
            Parse.Char(' ').Select(c => c.ToString()).Or(Parse.Return(string.Empty));

        static readonly Parser<string> OptSeparatorAndNumber =
            from separator in OptionalSeparator            
            from strNum in Parse.Number.Text()
            select strNum;

        static readonly Parser<decimal> Amount =
                from minus in Parse.String("-").Text().Or(Parse.Return(string.Empty))
                from triples in OptSeparatorAndNumber.Token().Many()
                from separator in Parse.Char(',').Once()
                from pointPart in Parse.Number.Once()
                let strDecimal = string.Format ("{0}{1}.{2}", minus, triples.Aggregate((s1, s2) => s1 + s2), 
                    pointPart.Single())
                select decimal.Parse(strDecimal, CultureInfo.InvariantCulture);

        static readonly Parser<DateTime> Date =
            from year in Parse.Number.Text()
            from dot1 in Parse.Char('.').Once()
            from month in Parse.Number.Text()
            from dot2 in Parse.Char('.').Once()
            from day in Parse.Number.Text()
            select new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));

        static readonly Parser<string> UpperString =
            Parse.Upper.Or(Parse.Char(' ')).Or(Parse.Char('-')).Many().Text().Token();

        static readonly Parser<FirstLineResult> FirstLineParser =
            from date in Date
            from desc in UpperString
            from amount in Amount
            from balance in Amount
            select new FirstLineResult { Date = date, Amount = amount, Balance = balance, Description = desc };

        static readonly Parser<AccountNumber> AccountNumberParser =
            from dig2 in TwoDigits
            from space1 in OptionalSeparator
            from dig4_1 in FourDigits
            from space2 in OptionalSeparator
            from dig4_2 in FourDigits
            from space3 in OptionalSeparator
            from dig4_3 in FourDigits
            from space4 in OptionalSeparator
            from dig4_4 in FourDigits
            from space5 in OptionalSeparator
            from dig4_5 in FourDigits
            from space6 in OptionalSeparator
            from dig4_6 in FourDigits
            select new AccountNumber(string.Format ("{0} {1} {2} {3} {4} {5} {6}", dig2, dig4_1,
                dig4_2, dig4_3, dig4_4, dig4_5, dig4_6));

        static readonly Parser<QifEntry> QifEntryParser =
            from firstLine in FirstLineParser
            from nl1 in NewLine
            from secondDate in Date
            from desc2 in UpperString.Or(Parse.Return(string.Empty))
            //from nl2 in NewLine
            from accNum in AccountNumberParser.Or(Parse.Return(new AccountNumber (string.Empty)))
            from desc3 in Parse.AnyChar.Many().Text().Token()
            select new QifEntry
            {
                AccountName = accNum.Number,
                Amount = firstLine.Amount,
                Date = new BankDates { OperationDate = firstLine.Date, BookingDate = secondDate },
                Description = string.Format("[{0}] {1} {2}:\n {3}", accNum.Number, firstLine.Description, desc2,
                desc3)
            };            

        static readonly Parser<IEnumerable<QifEntry>> QifEntriesParser =
            from entries in QifEntryParser.Many().End ()            
            select entries;
    }
}
