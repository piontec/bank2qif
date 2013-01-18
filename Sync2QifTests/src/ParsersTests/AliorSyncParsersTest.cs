using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bank2Qif.Parsers;
using Sprache;
using System.Globalization;
using Bank2Qif;

namespace Sync2QifTests.ParsersTests
{
    public class AliorSyncParsersTest
    {
        [Test]
        public void TestAmountOK()
        {
            decimal res = decimal.Parse("12.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncParsers.Amount.Parse("12,99"));

            res = decimal.Parse("-12.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncParsers.Amount.Parse("-12,99"));

            res = decimal.Parse("10000.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncParsers.Amount.Parse("10 000,99"));

            res = decimal.Parse("-10000.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncParsers.Amount.Parse("-10 000,99"));

            res = decimal.Parse("-10000.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncParsers.Amount.Parse("-10000,99"));

            res = decimal.Parse("11.23", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncParsers.Amount.Parse("11,23,23"));
        }

        [Test]
        public void TestAmountException()
        {
            Assert.Throws<ParseException>(() => AliorSyncParsers.Amount.Parse(""));
            Assert.Throws<ParseException>(() => AliorSyncParsers.Amount.Parse("1000"));
            Assert.Throws<ParseException>(() => AliorSyncParsers.Amount.Parse("-10-20,99"));            
        }

        [Test]
        public void TestUpperStringOK()
        {
            Assert.AreEqual("AAA", AliorSyncParsers.UpperString.Parse("AAA"));            
            //TODO: thats strange but despite of using Token(), the " " at the end still exists
            Assert.AreEqual("AAA ", AliorSyncParsers.UpperString.Parse(" AAA "));
            Assert.AreEqual("-AAA", AliorSyncParsers.UpperString.Parse("-AAA"));
            Assert.AreEqual("AAA-BBB", AliorSyncParsers.UpperString.Parse("AAA-BBB"));
            Assert.AreEqual("AAA BBB", AliorSyncParsers.UpperString.Parse("AAA BBB"));
            Assert.AreEqual("AAA BBB", AliorSyncParsers.UpperString.Parse("AAA BBBvsdsdf"));            
        }

        [Test]
        public void TestUpperStringNotParsing()
        {
            Assert.AreEqual("", AliorSyncParsers.UpperString.Parse(""));
            Assert.AreEqual("", AliorSyncParsers.UpperString.Parse("aaaa"));
            Assert.AreEqual("", AliorSyncParsers.UpperString.Parse("aaaaAAAA"));
            Assert.AreEqual("", AliorSyncParsers.UpperString.Parse("_AAAAA"));
        }

        [Test]
        public void TestFirstLineParserOK()
        {
            string l1 = @"2012.01.30 PRZELEW W RAMACH BANKU NA RACH OBCY 0,20 22,30";
            FirstLineResult result = new FirstLineResult
            {
                Amount = decimal.Parse("0.20", CultureInfo.InvariantCulture),
                Balance = decimal.Parse("22.30", CultureInfo.InvariantCulture),
                Date = DateTime.Parse("2012.01.30"),
                Description = "PRZELEW W RAMACH BANKU NA RACH OBCY"
            };
            Assert.AreEqual(result, AliorSyncParsers.FirstLineParser.Parse(l1));
        }

        [Test]
        public void TestFirstLineParserException()
        {
            string l1 = @"2012.01.320 PRZELEW W RAMACH BANKU NA RACH OBCY 0,20 22,30";
            string l2 = @"2012.01.30 PRZELEW W RAMACH BANKU NA RACH OBCY 0,2,0 22,30";
            string l3 = @"2012.01.30 PRZELEW W RAMACH BANKU 2,20 NA RACH OBCY 0,20 22,30";
            FirstLineResult result = new FirstLineResult
            {
                Amount = decimal.Parse("0.20", CultureInfo.InvariantCulture),
                Balance = decimal.Parse("22.30", CultureInfo.InvariantCulture),
                Date = DateTime.Parse("2012.01.30"),
                Description = "PRZELEW W RAMACH BANKU NA RACH OBCY"
            };
            Assert.Throws<ArgumentOutOfRangeException>(() => AliorSyncParsers.FirstLineParser.Parse(l1));
            Assert.Throws<InvalidOperationException>(() => AliorSyncParsers.FirstLineParser.Parse(l2));
            Assert.Throws<ParseException>(() => AliorSyncParsers.FirstLineParser.Parse(l3));
        }

        [Test]
        public void TestQifEntryParserOK()
        {
            string entry1 = 
@"2012.01.10 PRZELEW W RAMACH BANKU NA RACH OBCY 0,20 0,50
2012.01.02
11 2222 3333 4444 5555 6666 7777 John Smith
very important payment
from Mr. John Smith";
            QifEntry entry = AliorSyncParsers.QifEntryParser.Parse(entry1);
            Assert.AreEqual(DateTime.Parse("2012.01.10"), entry.Date.BookingDate);
            Assert.AreEqual(DateTime.Parse("2012.01.02"), entry.Date.OperationDate);
            Assert.AreEqual(decimal.Parse("0.20", CultureInfo.InvariantCulture), entry.Amount);
            Assert.AreEqual("11 2222 3333 4444 5555 6666 7777", entry.Payee);
            Assert.AreEqual("PRZELEW W RAMACH BANKU NA RACH OBCY : John Smith very important payment" 
                + " from Mr. John Smith", entry.Description);
        }
    }
}
