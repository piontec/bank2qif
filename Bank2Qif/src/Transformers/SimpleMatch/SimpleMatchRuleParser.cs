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
        // Payee % "text to match" => account_name
        // #exact
        // Description = "text to match" => account_name
        // Any % "text
        private const char LIKE_OP = '%';
        private const char EQUAL_OP = '=';

        public static readonly Parser<string> Separator = Parse.String("=>").Text ();

        public static readonly Parser<SimpleMatchRule.Operator> Operator =
            from op in Parse.Char(LIKE_OP).Or(Parse.Char(EQUAL_OP))
            select op == LIKE_OP ? SimpleMatchRule.Operator.Like
                : op == EQUAL_OP ? SimpleMatchRule.Operator.Equal : SimpleMatchRule.Operator.None;

        public static readonly Parser<SimpleMatchRule> SimpleRule =
            from field in Parse.Letter.Many().Text().Token()
            from sep1 in Parse.WhiteSpace.Many()
            from op in Operator
            from pattern in Parse.AnyChar.Until(Separator).Token ().Text()
            from result in Parse.AnyChar.Until(GenericParsers.NewLine).Or(
                Parse.AnyChar.Many().End()).Text().Token()
            select new SimpleMatchRule(field, op, pattern.Trim (), result);

        public static readonly Parser<IEnumerable<SimpleMatchRule>> SimpleRules =
            from rules in SimpleRule.Many()
            select rules;
    }
}
