using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;

namespace Bank2Qif.Parsers
{   
    public static class SimpleMatchRuleParser
    {
        // wildcard
        // Payee % "text do znalezienia" -> nazwa_konta
        // exact
        // Description = "text do znalezienia" -> nazwa_konta
        // Any % "text
        public static readonly Parser<string> Separator = Parse.String("->");
    }
}
