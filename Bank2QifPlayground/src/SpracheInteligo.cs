using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Bank2Qif;
using Bank2Qif.Parsers;
using Sprache;

namespace Bank2QifPlayground.src
{
    public class SpracheInteligo
    {
        //"Id","Data księgowania","Data zlecona","Typ transakcji","Kwota","Waluta","Saldo po transakcji","Rachunek nadawcy/odbiorcy","Nazwa nadawcy/odbiorcy","Opis transakcji"
        public static string l1 = "\"3065\",\"2012-01-15\",\"2012-01-15\",\"Przelew z rachunku\",\"-198.00\",\"PLN\",\"493.61\",\"50102055581111174342100049\",\"Joanna Piątkowska  ul. Cisowa 6, Pasieka 77-200 Miastko\",\"Joanna Piątkowska  ul. Cisowa 6, Pasieka 77-200 Miastko\",\"Nr banku: 10205558\",\"Nr rach.: 50102055581111174342100049\",\"Tytuł: za bilet na coldplay\",\"Data przygotowania: 2012-01-15\",\"Data waluty: 2012-01-15\"";
        public void Run()
        {
            var r1 = Csvp.Parse(l1);

            var r2 = from csvline in r1
                     let csv = csvline.ToArray()
                     let bookingDate = GenericParsers.DateYyyyMmDd.Parse(csv[1])
                     let opDate = GenericParsers.DateYyyyMmDd.Parse(csv[2])
                     let opType = csv[3]
                     let amount = Amount.Parse(csv[4])
                     let payee = csv [8]
                     let desc = csv.Where((s, i) => i == 7 || (i > 8 && s != csv[8])).
                        Aggregate((s1, s2) => string.Format("{0} {1}", s1, s2))
                     select new QifEntry
                     {
                         Amount = amount,
                         Date = new BankDates { OperationDate = opDate, BookingDate = bookingDate },
                         Payee = payee,
                         Description = desc
                     };
        }


        public static readonly string FIRST_LINE = "\"Id\",\"Data księgowania\",\"Data zlecona\",\"Typ transakcji\",\"Kwota\",\"Waluta\",\"Saldo po transakcji\",\"Rachunek nadawcy/odbiorcy\",\"Nazwa nadawcy/odbiorcy\",\"Opis transakcji\"\r\n";
        public static readonly Parser<char> Quote = Parse.Char('"');

        public static readonly Parser<string> Separator = Parse.String(",").Text ();

        public static Parser<T> Quoted<T>(Parser<T> content)
        {
            return from escapeStart in Quote
                   from c in content
                   from escapeEnd in Quote
                   select c;
        }

        public static readonly Parser<string> TextField =
            from q in Quoted(Parse.AnyChar.Many().Text ().Token ())
            select q;

        public static readonly Parser<string> TextFieldThenSeparator =
            from q in TextField.Then (_ => Separator)
            select q;

        public static readonly Parser<decimal> Amount =
            from sign in Parse.String("-").Text().Or(Parse.String("+").Text().Or (Parse.Return(string.Empty)))
            from whole in Parse.Number
            from separator in Parse.Char('.')
            from pointPart in Parse.Number
            let strDecimal = string.Format("{0}{1}.{2}", sign, whole, pointPart)
            select decimal.Parse(strDecimal, CultureInfo.InvariantCulture);

        public static readonly Parser<IEnumerable<IEnumerable<string>>> Csvp =
            from csvlines in CsvParser.CsvComma      
            select csvlines;

        public static readonly Parser<QifEntry> QifEntryParser =
            from id in Quoted(Parse.Number)
            from s1 in Separator
            from bookingDate in Quoted(GenericParsers.DateYyyyMmDd)
            from s2 in Separator
            from opDate in Quoted(GenericParsers.DateYyyyMmDd)
            from s3 in Separator
            from transactionType in TextFieldThenSeparator
            from amount in Quoted (Amount)
            from s4 in Separator
            from currencyCode in Quoted (Parse.Upper.Many ())
            from s5 in Separator
            from state in Quoted(Amount)
            from accountName in TextFieldThenSeparator
            //from payee in TextField
            //from description in TextFieldThenSeparator.Many ()
            //from ending in TextField.Then (_ => GenericParsers.NewLine)
            select new QifEntry();

        
    }
}
