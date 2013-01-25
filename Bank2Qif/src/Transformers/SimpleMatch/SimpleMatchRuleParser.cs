using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Parsers;
using Sprache;

namespace Bank2Qif.Transformers.SimpleMatch
{   
    public static class SimpleMatchRuleParsers
    {
        // #wildcard
        // Payee % "text do znalezienia" -> nazwa_konta
        // #exact
        // Description = "text do znalezienia" -> nazwa_konta
        // Any % "text
        private const char LIKE_OP = '%';
        private const char EQUAL_OP = '=';

        public static readonly Parser<string> Separator = Parse.String("->").Text ();

        public static readonly Parser<SimpleMatchRule.Operator> Operator =
            from op in Parse.Char(LIKE_OP).Or(Parse.Char(EQUAL_OP))
            select op == LIKE_OP ? SimpleMatchRule.Operator.Like
                : op == EQUAL_OP ? SimpleMatchRule.Operator.Equal : SimpleMatchRule.Operator.None;

        public static readonly Parser<SimpleMatchRule> SimpleRule =
            from field in Parse.Letter.Many().Text ().Token ()
            from sep1 in Parse.WhiteSpace.Many()
            from op in Operator
            from pattern in Parse.AnyChar.Until(Separator).Text().Token()
            from result in Parse.AnyChar.Until(GenericParsers.NewLine).Text().Token()
            select new SimpleMatchRule (field, op, pattern, result);

        public static readonly Parser<IEnumerable<SimpleMatchRule>> SimpleRules =
            from rules in SimpleRule.Many().End()
            select rules;
    }
}
