using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;

namespace Bank2Qif.Parsers
{
    public static class PayuParsers
    {
        private static readonly string s_stupidInfoMsg =
            "informujemy o pozytywnym zakończeniu Państwa płatności w sklepie/serwisie ";
        private static readonly string s_beginItemDesc = "allegro.pl/show_item.php?item=";
        private static readonly string s_endTagA = "</a>";
        //"<a href=\"http://www.allegro.pl/show_item.php?item=2694621696\">FILTRY ŻELOWE HITECH DO LAMPY 15 ARKUSZY STROBIST</a>"
        // <a href=\"http://allegro.pl/show_item.php?item=2810342496\">ŁAŃCUCH STOJAK NA WINO ALKOHOL NOWOCZESNY DESIGN!!</a>
        public static readonly Parser<string> SyncIdDelimiter =
            Parse.String("XX").Text();

        public static readonly Parser<string> SyncIdParser =
            from leading in Parse.AnyChar.Until(SyncIdDelimiter)
            from syncId in Parse.Number
            from endDelim in SyncIdDelimiter
            select syncId;

        // "PayU w Allegro XX255709781XX piontec aukcja nr (2693753978)"
        public static readonly Parser<Tuple<string, string>> SyncAndAllegroIdParser =
            from syncId in SyncIdParser
            from garbage in Parse.AnyChar.Until(Parse.String("aukcja nr "))
            from left in Parse.Char('(').Once()
            from allegroId in Parse.Number
            from right in Parse.Char(')').Once()
            select Tuple.Create(syncId, allegroId);

        public static readonly Parser<string> SyncPaymentReceiver =
            from garbage in Parse.AnyChar.Until(Parse.String("- odbiorca: "))
            from receiver in Parse.AnyChar.Until(GenericParsers.NewLine).Text()
            select receiver;

        public static readonly Parser<string> SyncPaymentStupidInfo =
            from garbage in Parse.AnyChar.Until(Parse.String(s_stupidInfoMsg))
            from receiver in Parse.AnyChar.Until(GenericParsers.NewLine).Text()
            select receiver;

        public static readonly Parser<string> QuickAndDirtyHtmlInfoParser =
            from garbage in Parse.AnyChar.Until(Parse.String(s_beginItemDesc))
            from itemId in Parse.Number
            from closingTagA in Parse.String("\">").Text()
            from description in Parse.AnyChar.Until(Parse.String(s_endTagA)).Text()
            select description;
    }
}
