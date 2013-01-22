using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;
using Bank2Qif;

namespace Bank2Qif.Parsers
{
    public static class GenericParsers
    {
        public static readonly Parser<string> NewLine = Parse.String(Environment.NewLine).Text();

        public static readonly Parser<string> TwoDigits =
           from dig1 in Parse.Digit
           from dig2 in Parse.Digit
           select string.Format("{0}{1}", dig1, dig2);

        public static readonly Parser<string> ThreeDigits =
            from firstTwo in TwoDigits
            from dig3 in Parse.Digit
            select string.Format("{0}{1}", firstTwo, dig3);
        
        public static readonly Parser<string> FourDigits =
            from firstThree in ThreeDigits
            from dig4 in Parse.Digit
            select string.Format("{0}{1}", firstThree, dig4);

        public static readonly Parser<string> OptionalSpace =
          Parse.Char(' ').Select(c => c.ToString()).Or(Parse.Return(string.Empty));

        public static readonly Parser<string> OptionalSpaceThenNumber =
            from separator in OptionalSpace
            from strNum in Parse.Number.Text()
            select strNum;

        public static readonly Parser<DateTime> DateYyyyMmDd =
            from year in FourDigits
            from dot1 in Parse.Char('.').Once()
            from month in TwoDigits
            from dot2 in Parse.Char('.').Once()
            from day in TwoDigits
            select new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));

		public static readonly Parser<DateTime> DateMmDdYyyy =
			from day in TwoDigits
			from sep1 in Parse.Char('-').Once()
			from month in TwoDigits
			from dot2 in Parse.Char('-').Once()
			from year in FourDigits
			select new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));

        public static readonly Parser<AccountNumber> AccountNumberParser =
          from dig2 in GenericParsers.TwoDigits
          from space1 in OptionalSpace
          from dig4_1 in FourDigits
          from space2 in OptionalSpace
          from dig4_2 in FourDigits
          from space3 in OptionalSpace
          from dig4_3 in FourDigits
          from space4 in OptionalSpace
          from dig4_4 in FourDigits
          from space5 in OptionalSpace
          from dig4_5 in FourDigits
          from space6 in OptionalSpace
          from dig4_6 in FourDigits
          select new AccountNumber(string.Format("{0} {1} {2} {3} {4} {5} {6}", dig2, dig4_1,
              dig4_2, dig4_3, dig4_4, dig4_5, dig4_6));
    }
}
