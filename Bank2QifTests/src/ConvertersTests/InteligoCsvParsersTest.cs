using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Converters;
using Bank2Qif.Converters.Inteligo;
using NUnit.Framework;

namespace Bank2QifTests.ConvertersTests
{   
    [TestFixture]
    public class InteligoCsvParsersTest
    {
        private const string testLines =
        "\"Id\",\"Data księgowania\",\"Data zlecona\",\"Typ transakcji\",\"Kwota\",\"Waluta\",\"Saldo po transakcji\",\"Rachunek nadawcy/odbiorcy\",\"Nazwa nadawcy/odbiorcy\",\"Opis transakcji\""
        + "\n\r"
        + "\"1111\",\"2012-01-11\",\"2012-01-11\",\"Przelew z rachunku\",\"-100.00\",\"PLN\",\"200.00\",\"11222233334444555566667777\",\"XXXX YYYYY  ZZZZZ, WWWWWW ZIP City\",\"XXXX YYYYY  ZZZZZ, WWWWWW ZIP City\",\"XX XXXXXX: 11112222\",\"XX XXXX.: 11222233334444555566667777\",\"XXXXXX: XX XXXX XX XXXXX\",\"XXXXX XXXXXX: 2012-01-11\",\"XXXXXX XXXXXXXXXXXX\"";
    
        private IConverter m_converter;


        [SetUp]
        public void SetUp()
        {
            m_converter = new InteligoCsvToQif();
        }


        [Test]
        public void ShouldParseOneEntry()
        {
            var entries = m_converter.ConvertLinesToQif(testLines);
            Assert.AreEqual (entries.Count (), 1);
        }
    }
}
