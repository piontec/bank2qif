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
            Case3();                       
        }

        public void Case3()
        {
            string s1 = "PRZELEW WEWNĘTRZNY - PŁACĘ Z ALIOR BANKIEM: PayU S.A. ul. Marcelińska 90/ 60-324 Poznań Pay by link PayU XX258795407XX Multikurs.pl  za mowienie nr 1497  Krysia  kursy";
            string s2 = "PRZELEW WEWNĘTRZNY - PŁACĘ Z ALIOR BANKIEM: PayU S.A. ul. Marcelińska 90/ 60-324 Poznań Pay by link PayU w Allegro XX256741454XX pionte c aukcja nr (2709801541)";

            var r1 = TestParse.SyncIdParser.Parse(s1);
            var r2 = TestParse.SyncAndAllegroIdParser.Parse(s2);
        }

        public void Case2()
        {
            var r1 = TestParse.UntilNewline.Parse("taka tam asieczka jak nie wiem \r\n i dlaej ");
        }

        public void Case1()
        {
            var r1 = TestParse.UpperString.Parse("PRZELEW DO INNEGO BANKU KRAJOWEGO -0,01 0,09");
            //TODO: thats strange but despite of using Token(), the " " at the end still exists
            //var r5 = TestParse.OpTitle.Parse("   PRZELEW DO INNEGO BANKU KRAJOWEGO -0,01 0,09");
            //var r2 = TestParse.OtherUpper.Parse("PRZELEW DO INNEGO BANKU KRAJOWEGO -0,01 0,09");

            //var r4 = TestParse.UpperString2.Parse("PRZELEW DO INNEGO BANKU KRAJOWEGO -0,01 0,09");

            //var r3 = TestParse.NewUpperString.Parse("PRZELEW DO INNEGO BANKU KRAJOWEGO -0,01 0,09");
        }
    }
    
    public static class TestParse
    {
        public static readonly Parser<string> NewLine = Parse.String(Environment.NewLine).Text();

        public static readonly Parser<string> UntilNewline =
            Parse.AnyChar.Until(NewLine).Text();


        public static readonly Parser<string> OpTitle =
            from trailing in Parse.WhiteSpace.Many()
            from op in Parse.String("PRZELEW W RAMACH BANKU NA RACH OBCY").Or(
            Parse.String("PRZELEW DO INNEGO BANKU KRAJOWEGO").Or(
            Parse.String("PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO").Or(
            Parse.String("ZAŁOŻENIE LOKATY").Or(
            Parse.String("TRANSAKCJA KARTĄ DEBETOWĄ"))))).Text()
            from ending in Parse.WhiteSpace.Many()
            select op;

        public static readonly Parser<string> UpperString =
            Parse.Upper.XOr(Parse.Char(' ')).XOr(Parse.Char('-')).Many().Token().Text().Token();

        public static readonly Parser<string> UpperString2 =
            Parse.Upper.Many().Text().Token();

        public static readonly Parser<string> OtherUpper =
            from first in Parse.Upper.Once()
            from rest in Parse.Upper.XOr(Parse.Char('-')).XOr(Parse.Char(' ')).Many()
            select new string(first.Concat(rest).ToArray());

        public static readonly Parser<string> NewUpperString =            
            from leading in Parse.WhiteSpace.Many()
            from first in Parse.Upper.Once()
            from rest in Parse.Upper.XOr(Parse.Char(' ')).XOr(Parse.Char('-')).Many()
            from last in Parse.Upper.Once()
            from trailing in Parse.WhiteSpace.Many()
            select new string(first.Concat(rest).Concat(last).ToArray());

        public static readonly Parser<string> SyncIdDelimiter =
            Parse.String("XX").Text ();

        public static readonly Parser<string> SyncIdParser =
            from leading in Parse.AnyChar.Until(SyncIdDelimiter)
            from syncId in Parse.Number
            from endDelim in SyncIdDelimiter
            select syncId;

        public static readonly Parser<Tuple<string, string>> SyncAndAllegroIdParser =
            from syncId in SyncIdParser
            from garbage in Parse.AnyChar.Until(Parse.String("aukcja nr ").Text())
            from left in Parse.Char('(').Once()
            from allegroId in Parse.Number
            from right in Parse.Char(')').Once()
            select Tuple.Create(syncId, allegroId);
            
    }
}
