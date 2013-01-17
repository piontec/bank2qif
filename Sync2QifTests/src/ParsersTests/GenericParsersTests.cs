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
        public void TestNewLineParserOK()
        {
            Assert.AreEqual (Environment.NewLine, GenericParsers.NewLine.Parse (Environment.NewLine));
        }

        [Test]
        public void TestNewLineParserException()
        {
            Assert.Throws<ParseException>(() => GenericParsers.NewLine.Parse("A"));
        }
    }
}
