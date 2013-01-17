using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bank2Qif.Parsers;
using Sprache;


namespace Sync2QifTests.src.ParsersTests
{
    public class GenericParsersTests
    {
        [Test]
        public void TestNewLineParserOK ()
        {
            Assert.AreEqual (Environment.NewLine, GenericParsers.NewLine.Parse (Environment.NewLine));
        }

        [Test]
        public void TestNewLineParserException ()
        {
            Assert.Throws<ParseException>(() => GenericParsers.NewLine.Parse("A"));
        }

        [Test]
        public void TestTwoDigitParserOK()
        {
            Assert.AreEqual("12", GenericParsers.TwoDigits.Parse ("12"));
            Assert.AreEqual("12", GenericParsers.TwoDigits.Parse("122"));
        }

        [Test]
        public void TestTwoDigitParserException()
        {            
            Assert.Throws<ParseException>(() => GenericParsers.TwoDigits.Parse("1"));
            Assert.Throws<ParseException>(() => GenericParsers.TwoDigits.Parse("1a"));
        }

        [Test]
        public void TestThreeDigitParserOK()
        {
            Assert.AreEqual("122", GenericParsers.ThreeDigits.Parse("122"));
            Assert.AreEqual("122", GenericParsers.ThreeDigits.Parse("1222"));
        }

        [Test]
        public void TestThreeDigitParserException()
        {
            Assert.Throws<ParseException>(() => GenericParsers.ThreeDigits.Parse("11"));
            Assert.Throws<ParseException>(() => GenericParsers.ThreeDigits.Parse("12a"));
        }

        [Test]
        public void TestFourDigitParserOK()
        {
            Assert.AreEqual("1223", GenericParsers.FourDigits.Parse("1223"));
            Assert.AreEqual("1223", GenericParsers.FourDigits.Parse("1223a"));
        }

        [Test]
        public void TestFourDigitParserException()
        {
            Assert.Throws<ParseException>(() => GenericParsers.FourDigits.Parse("111"));
            Assert.Throws<ParseException>(() => GenericParsers.FourDigits.Parse("121a"));
        }

        [Test]
        public void TestOptionalSpaceOK()
        {
            Assert.AreEqual(" ", GenericParsers.OptionalSpace.Parse("  "));
            Assert.AreEqual(" ", GenericParsers.OptionalSpace.Parse(" "));
            Assert.AreEqual("", GenericParsers.OptionalSpace.Parse(""));
            Assert.AreEqual("", GenericParsers.OptionalSpace.Parse("a"));
            Assert.AreEqual(" ", GenericParsers.OptionalSpace.Parse(" ab"));
        }

        [Test]
        public void TestOptionalSpaceThenNumberOK()
        {
            Assert.AreEqual("123", GenericParsers.OptionalSpaceThenNumber.Parse(" 123"));
            Assert.AreEqual("123", GenericParsers.OptionalSpaceThenNumber.Parse("123"));
            Assert.AreEqual("12", GenericParsers.OptionalSpaceThenNumber.Parse("12abc"));
            Assert.AreEqual("12", GenericParsers.OptionalSpaceThenNumber.Parse(" 12abc"));
        }

        [Test]
        public void TestOptionalSpaceThenNumberException()
        {
            Assert.Throws<ParseException>(() => GenericParsers.OptionalSpaceThenNumber.Parse("abc"));
            Assert.Throws<ParseException>(() => GenericParsers.OptionalSpaceThenNumber.Parse(" a123"));
        }

        [Test]
        public void TestDateYyyyMmDdOK()
        {
            string d1 = "2012.11.23";
            Assert.AreEqual(DateTime.Parse (d1), GenericParsers.DateYyyyMmDd.Parse(d1));            
        }

        [Test]
        public void TestDateYyyyMmDdException()
        {
            Assert.Throws<ParseException>(() => GenericParsers.DateYyyyMmDd.Parse("a123"));
            Assert.Throws<ParseException>(() => GenericParsers.DateYyyyMmDd.Parse("201.12.11"));
            Assert.Throws<ParseException>(() => GenericParsers.DateYyyyMmDd.Parse("12.12.12"));
            Assert.Throws<ParseException>(() => GenericParsers.DateYyyyMmDd.Parse("1212.112.12"));
        }

        [Test]
        public void TestAccountNumberParserOK()
        {
            string n1 = "11 2222 3333 4444 5555 6666 7777";
            string n1NoSpace = "11222233334444555566667777";
            Assert.AreEqual(n1, GenericParsers.AccountNumberParser.Parse(n1).ToString());
            Assert.AreEqual(n1, GenericParsers.AccountNumberParser.Parse(n1NoSpace).ToString());
        }

        [Test]
        public void TestAccountNumberParserException()
        {
            Assert.Throws<ParseException>(() => GenericParsers.AccountNumberParser.Parse("12"));
            Assert.Throws<ParseException>(() => GenericParsers.AccountNumberParser.Parse(
                "1111 2222 3333 4444 5555 6666 7777"));
        }
    }
}
