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

        public static readonly Parser<string> SyncIdDelimiter =
            Parse.String("XX").Text();

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

        public static readonly Parser<string> SyncPaymentReceiver =
            from garbage in Parse.AnyChar.Until(Parse.String("- odbiorca: ").Text())
            from receiver in Parse.AnyChar.Until(GenericParsers.NewLine).Text()
            select receiver;

        public static readonly Parser<string> SyncPaymentStupidInfo =
            from garbage in Parse.AnyChar.Until(Parse.String(s_stupidInfoMsg).Text())
            from receiver in Parse.AnyChar.Until(GenericParsers.NewLine).Text()
            select receiver;
    }
}
