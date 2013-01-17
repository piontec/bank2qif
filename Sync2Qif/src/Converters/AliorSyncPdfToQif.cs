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


namespace Bank2Qif.Converters
{
    [Converter ("sync", "pdf")]
    public class AliorSyncPdfToQif : IConverter
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

        private readonly string FIRST_PAGE_START = @"DATA OPERACJI OPERACJI OPERACJI KSIĘGOWE";
        private readonly string FIRST_PAGE_END = @"Infolinia Alior Sync dostępna jest przez całą dobę pod numerami 19 506";
        private readonly string NEXT_PAGE_START = @"Wyciąg z rachunku bankowego";
        private readonly string NEXT_PAGE_END = @"Niniejszy dokument jest wydrukiem komputerowym sporządzonym zgodnie z art. 7";

        private readonly string[] OpNames = new string[] 
		{
			"PRZELEW W RAMACH BANKU NA RACH OBCY", 
			"PRZELEW DO INNEGO BANKU KRAJOWEGO",
			"PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO",
			"PRZELEW NATYCHMIASTOWY",
			"ZAŁOŻENIE LOKATY",
            "TRANSAKCJA KARTĄ DEBETOWĄ",
            "PRZELEW WEWNĘTRZNY - PŁACĘ Z ALIOR BANKIEM",
            "KAPITALIZACJA ODSETEK",
		};


        public IEnumerable<QifEntry> ParseToQif(string lines)
        {
            return QifEntriesParser.Parse(lines);
        }


        public QifEntry ParseSingleToQif(string lines)
        {
            return QifEntryParser.Parse(lines);
        }


        public IEnumerable<QifEntry> ConvertFileToQif(string fileName)
        {
            var lines = ExtractLinesFromPdf(fileName);

            return ConvertExtractedTextToPdf(lines);
        }

        public IEnumerable<QifEntry> ConvertExtractedTextToPdf(IEnumerable<string> lines)
        {
            if (!OpNames.Any(s => lines.ElementAt(0).Contains(s)))
                throw new ArgumentException("Bad first line");
            var current = new List<string>();
            var entries = new List<QifEntry>();

            foreach (var line in lines)
            {
                // if it is a line starting with correct operation name and if it is a subsequent operation line
                if (OpNames.Any(s => line.Contains(s)) && current.Count > 1)
                {
                    var total = current.Aggregate((s1, s2) => string.Format("{0}{1}{2}", s1, Environment.NewLine, s2));
                    entries.Add(ParseSingleToQif(total));
                    current.Clear();
                }
                current.Add(line);
            }
            var lastOne = current.Aggregate((s1, s2) => string.Format("{0}{1}{2}", s1, Environment.NewLine, s2));
            entries.Add(ParseSingleToQif(lastOne));

            return entries;
        }


        private IEnumerable<string> ExtractLinesFromPdf(string fileName)
        {
            PdfReader reader = new PdfReader(fileName);

            IEnumerable<string> lines = new List<string>();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                string input = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                lines = lines.Concat(TrimStrings(input, i == 1));
            }

            reader.Close();
            return lines;
        }


        public IEnumerable<string> TrimStrings(string pdfInput, bool isFirstPage = false)
        {
            var strings = pdfInput.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var startDelim = isFirstPage ? FIRST_PAGE_START : NEXT_PAGE_START;
            var endDelim = isFirstPage ? FIRST_PAGE_END : NEXT_PAGE_END;

            int firstId = strings.IndexOf(strings.Where(s => s.StartsWith(startDelim)).Single());
            int lastId = strings.IndexOf(strings.Where(s => s.StartsWith(endDelim)).Single());

            return strings.Where((s, i) => i > firstId && i < lastId);
        }


        static readonly Parser<string> NewLine = Parse.String(Environment.NewLine).Text();

        static readonly Parser<string> TwoDigits =
            from dig1 in Parse.Digit
            from dig2 in Parse.Digit
            select string.Format("{0}{1}", dig1, dig2);

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
                let strDecimal = string.Format("{0}{1}.{2}", minus, triples.Aggregate((s1, s2) => s1 + s2),
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
            select new AccountNumber(string.Format("{0} {1} {2} {3} {4} {5} {6}", dig2, dig4_1,
                dig4_2, dig4_3, dig4_4, dig4_5, dig4_6));

        static readonly Parser<QifEntry> QifEntryParser =
            from firstLine in FirstLineParser
            from nl1 in NewLine
            from secondDate in Date
            from desc2 in UpperString.Or(Parse.Return(string.Empty))
            //from nl2 in NewLine
            from accNum in AccountNumberParser.Or(Parse.Return(new AccountNumber(string.Empty)))
            from desc3 in Parse.AnyChar.Many().Text().Token()
            select new QifEntry
            {
                AccountName = accNum.Number,
                Amount = firstLine.Amount,
                Date = new BankDates { OperationDate = firstLine.Date, BookingDate = secondDate },
                Payee = accNum.Number,
                Description = string.Format("{1} {2}: {3}", firstLine.Description, desc2, desc3)
            };

        static readonly Parser<IEnumerable<QifEntry>> QifEntriesParser =
            from entries in QifEntryParser.Many().End()
            select entries;
       
    }
}
