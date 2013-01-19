using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;
using Bank2Qif;


namespace Bank2QifPlayground.src
{
    public class SpracheALior
    {
        public void Run()
        {
            var r1 = TestParse.UpperString.Parse("AAA");
            //TODO: thats strange but despite of using Token(), the " " at the end still exists
            var r2 = TestParse.OtherUpper.Parse("AAA ");
        }
    }

    public static class TestParse
    {
        public static readonly Parser<string> UpperString =
            Parse.Upper.XOr(Parse.Char(' ')).XOr(Parse.Char('-')).Many().Token().Text().Token();

        public static readonly Parser<string> OtherUpper =
            from first in Parse.Upper.Once()
            from rest in Parse.Upper.XOr(Parse.Char('-')).XOr(Parse.Char(' ')).Many()
            select new string(first.Concat(rest).ToArray());

    }
}
