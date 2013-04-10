using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif;
using Bank2Qif.Transformers.SimpleMatch;
using NUnit.Framework;

namespace Bank2QifTests.TransformersTests
{
    [TestFixture]
    public class SimpleMatchRuleTests
    {
        private const string DefaultAccount = "default account";
        
        private QifEntry entry1;

        [SetUp]
        public void SetUp()
        {
            entry1 = new QifEntry
            {
                Amount = 1000,
                Date = new BankDates
                {
                    BookingDate = DateTime.Parse("2012-01-20"),
                    OperationDate = DateTime.Parse("2012-01-19")
                },
                Description = "A thing you bought in a shop",
                Payee = "myself",
                AccountName = DefaultAccount
            };
        }

        [Test]
        public void ShouldMatchOnEqualToPayee()
        {
            string expectedAccount = "12345 6789 123";
            var rule = new SimpleMatchRule ("Payee", SimpleMatchRule.Operator.Equal, "myself", expectedAccount);

            var res = rule.Transform(new QifEntry [] {entry1});

            Assert.That(res.Count (), Is.EqualTo(1));
            var r = res.Single();

            Assert.That(r.AccountName, Is.EqualTo(expectedAccount));
        }


        [Test]
        public void ShouldMatchOnEqualToAccountName()
        {
            string expectedAccount = "12345 6789 123";
            var rule = new SimpleMatchRule("AccountName", SimpleMatchRule.Operator.Equal, DefaultAccount, 
                expectedAccount);

            var res = rule.Transform(new QifEntry[] { entry1 });

            Assert.That(res.Count(), Is.EqualTo(1));
            var r = res.Single();

            Assert.That(r.AccountName, Is.EqualTo(expectedAccount));
        }

        [Test]
        public void ShouldMatchOnLikeDescription()
        {
            string expectedAccount = "12345 6789 123";
            var rule = new SimpleMatchRule("Description", SimpleMatchRule.Operator.Like, "shop",
                expectedAccount);

            var res = rule.Transform(new QifEntry[] { entry1 });

            Assert.That(res.Count(), Is.EqualTo(1));
            var r = res.Single();

            Assert.That(r.AccountName, Is.EqualTo(expectedAccount));
        }

        [Test]
        public void ShouldNotMatchOnLikeDescription()
        {
            string expectedAccount = "12345 6789 123";
            var rule = new SimpleMatchRule("Description", SimpleMatchRule.Operator.Like, "shp",
                expectedAccount);

            var res = rule.Transform(new QifEntry[] { entry1 });

            Assert.That(res.Count(), Is.EqualTo(1));
            var r = res.Single();

            Assert.That(r.AccountName, Is.EqualTo(DefaultAccount));
        }


        [Test]
        [ExpectedException (typeof (ArgumentException))]
        public void ShouldThrowExceptionWrongField()
        {
            var rule = new SimpleMatchRule("nonExisting", SimpleMatchRule.Operator.Equal, DefaultAccount, "");

            Assert.Fail("Should not reach this");
        }
    }
}
