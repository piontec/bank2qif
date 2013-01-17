using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;

namespace Bank2Qif.Parsers
{
    public static class GenericParsers
    {
        public static readonly Parser<string> NewLine = Parse.String(Environment.NewLine).Text();
    }
}
