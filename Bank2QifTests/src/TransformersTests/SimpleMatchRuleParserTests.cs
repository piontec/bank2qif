using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Transformers.SimpleMatch;
using NUnit.Framework;
using Sprache;


namespace Bank2QifTests.TransformersTests
{
    [TestFixture]
    public class SimpleMatchRuleParserTests
    {
        private string rule1 = "Description = my description => 12345";
        private string rule2 = "AccountName % my description => 12345";


        [Test]
        public void ShouldParseEqualRule()
        {
            var rule = SimpleMatchRuleParsers.SimpleRule.Parse(rule1);

            Assert.That(rule.FieldName, Is.EqualTo("Description"));
            Assert.That(rule.RuleOperator, Is.EqualTo(SimpleMatchRule.Operator.Equal));
            Assert.That(rule.Pattern, Is.EqualTo("my description"));
            Assert.That(rule.Result, Is.EqualTo("12345"));
        }


        [Test]
        public void ShouldParseLikeRule()
        {
            var rule = SimpleMatchRuleParsers.SimpleRule.Parse(rule2);

            Assert.That(rule.FieldName, Is.EqualTo("AccountName"));
            Assert.That(rule.RuleOperator, Is.EqualTo(SimpleMatchRule.Operator.Like));
            Assert.That(rule.Pattern, Is.EqualTo("my description"));
            Assert.That(rule.Result, Is.EqualTo("12345"));
        }
    }
}
