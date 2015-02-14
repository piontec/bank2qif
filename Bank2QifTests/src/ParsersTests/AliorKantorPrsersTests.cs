using Bank2Qif.Converters.AliorKantor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;

namespace Bank2QifTests.src.ParsersTests
{
    [TestFixture]
    public class AliorKantorPrsersTests
    {   
        public void AmountTest (string str, decimal dec)
        {
            var res = AliorKantorCsvParsers.Amount.Parse(str);
            Assert.AreEqual(dec, res);
        }

        [Test]
        public void Amount1 ()
        {
            AmountTest("11,11", new decimal(11.11));
        }


        [Test]
        public void Amount2()
        {
            AmountTest("11,1", new decimal(11.1));
        }

        [Test]
        public void Amount3()
        {
            AmountTest("11", new decimal(11));
        }
    }
}
