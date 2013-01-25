using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bank2Qif.Parsers;
using Sprache;
using System.Globalization;
using Bank2Qif;
using Bank2Qif.Converters.AliorSync;

namespace Bank2QifTests.ParsersTests
{
    public class AliorSyncParsersTest
    {
        [Test]
        public void TestAmountOK()
        {
            decimal res = decimal.Parse("12.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncPdfParsers.Amount.Parse("12,99"));

            res = decimal.Parse("-12.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncPdfParsers.Amount.Parse("-12,99"));

            res = decimal.Parse("10000.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncPdfParsers.Amount.Parse("10 000,99"));

            res = decimal.Parse("-10000.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncPdfParsers.Amount.Parse("-10 000,99"));

            res = decimal.Parse("-10000.99", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncPdfParsers.Amount.Parse("-10000,99"));

            res = decimal.Parse("11.23", CultureInfo.InvariantCulture);
            Assert.AreEqual(res, AliorSyncPdfParsers.Amount.Parse("11,23,23"));
        }

        [Test]
        public void TestAmountException()
        {
            Assert.Throws<ParseException>(() => AliorSyncPdfParsers.Amount.Parse(""));
            Assert.Throws<ParseException>(() => AliorSyncPdfParsers.Amount.Parse("1000"));
            Assert.Throws<ParseException>(() => AliorSyncPdfParsers.Amount.Parse("-10-20,99"));            
        }

        [Test]
        public void TestOperationNameOK()
        {
            Assert.AreEqual("ZAŁOŻENIE LOKATY", AliorSyncPdfParsers.OperationName.Parse("ZAŁOŻENIE LOKATY"));
            Assert.AreEqual("ZAŁOŻENIE LOKATY", AliorSyncPdfParsers.OperationName.Parse("   ZAŁOŻENIE LOKATY   -67"));            
        }

        [Test]
        public void TestOperationNameException()
        {
            Assert.Throws<ParseException>(() => AliorSyncPdfParsers.OperationName.Parse("AAAA"));
            Assert.Throws<ParseException>(() => AliorSyncPdfParsers.OperationName.Parse("AAAA BBBBBB"));
            Assert.Throws<ParseException>(() => AliorSyncPdfParsers.OperationName.Parse("AŁOŻENIE LOKATY"));
        }

        [Test]
        public void TestFirstLineParserOK()
        {
            string l1 = @"2012.01.30 PRZELEW W RAMACH BANKU NA RACH OBCY 0,20 22,30";
            AliorSyncPdfFirstLineResult result = new AliorSyncPdfFirstLineResult
            {
                Amount = decimal.Parse("0.20", CultureInfo.InvariantCulture),
                Balance = decimal.Parse("22.30", CultureInfo.InvariantCulture),
                Date = DateTime.Parse("2012.01.30"),
                Description = "PRZELEW W RAMACH BANKU NA RACH OBCY"
            };
            Assert.AreEqual(result, AliorSyncPdfParsers.FirstLineParser.Parse(l1));
        }

        [Test]
        public void TestFirstLineParserException()
        {
            string l1 = @"2012.01.320 PRZELEW W RAMACH BANKU NA RACH OBCY 0,20 22,30";
            string l2 = @"2012.01.30 PRZELEW W RAMACH BANKU NA RACH OBCY 0,2,0 22,30";
            string l3 = @"2012.01.30 PRZELEW W RAMACH BANKU 2,20 NA RACH OBCY 0,20 22,30";
            AliorSyncPdfFirstLineResult result = new AliorSyncPdfFirstLineResult
            {
                Amount = decimal.Parse("0.20", CultureInfo.InvariantCulture),
                Balance = decimal.Parse("22.30", CultureInfo.InvariantCulture),
                Date = DateTime.Parse("2012.01.30"),
                Description = "PRZELEW W RAMACH BANKU NA RACH OBCY"
            };
            Assert.Throws<ArgumentOutOfRangeException>(() => AliorSyncPdfParsers.FirstLineParser.Parse(l1));
            Assert.Throws<InvalidOperationException>(() => AliorSyncPdfParsers.FirstLineParser.Parse(l2));
            Assert.Throws<ParseException>(() => AliorSyncPdfParsers.FirstLineParser.Parse(l3));
        }

        [Test]
        public void TestQifEntryParserOK_1()
        {
            string entry1 = 
@"2012.01.10 PRZELEW W RAMACH BANKU NA RACH OBCY 0,20 0,50
2012.01.02
11 2222 3333 4444 5555 6666 7777 John Smith
very important payment
from Mr. John Smith";
            QifEntry entry = AliorSyncPdfParsers.QifEntryParser.Parse(entry1);
            Assert.AreEqual(DateTime.Parse("2012.01.10"), entry.Date.BookingDate);
            Assert.AreEqual(DateTime.Parse("2012.01.02"), entry.Date.OperationDate);
            Assert.AreEqual(decimal.Parse("0.20", CultureInfo.InvariantCulture), entry.Amount);
            Assert.AreEqual("11 2222 3333 4444 5555 6666 7777", entry.Payee);
            Assert.AreEqual("PRZELEW W RAMACH BANKU NA RACH OBCY: John Smith very important payment" 
                + " from Mr. John Smith", entry.Description);
        }

        [Test]
        public void TestQifEntryParserOK_2()
        {
            string entry1 =
@"2012.01.15 PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO 100,00 0,0
2012.01.05 BANKU
11 2222 3333 4444 5555 6666 7777 Jan Kowalski
very important payment
from Mr. John Smith";
            QifEntry entry = AliorSyncPdfParsers.QifEntryParser.Parse(entry1);
            Assert.AreEqual(DateTime.Parse("2012.01.15"), entry.Date.BookingDate);
            Assert.AreEqual(DateTime.Parse("2012.01.05"), entry.Date.OperationDate);
            Assert.AreEqual(decimal.Parse("100", CultureInfo.InvariantCulture), entry.Amount);
            Assert.AreEqual("11 2222 3333 4444 5555 6666 7777", entry.Payee);
            Assert.AreEqual("PRZELEW KRAJOWY ELIXIR PRZYCHODZACY Z INNEGO BANKU: Jan Kowalski "
                + "very important payment from Mr. John Smith", entry.Description);            
        }        
    }
}
