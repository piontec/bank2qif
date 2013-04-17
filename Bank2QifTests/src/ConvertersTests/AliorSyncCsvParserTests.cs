using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bank2Qif.Converters.AliorSync;
using Bank2Qif.Converters;

namespace Bank2QifTests.ConvertersTests
{
    [TestFixture]
    public class AliorSyncCsvParserTests
    {
        private const string testLines = @"Data,Nazwa odbiorcy/nadawcy,Rachunek,Tytuł płatności,Kwota"
            + "\r\n"
            + @"01-09-2012,AAA BBB CCC 11001,Konto osobiste,Transakcja kartą debetową;obciążenie 1111 OTHER111111-1111666666 POL AA BB CC DDD 55555,-111.99 PLN";

        private IConverter m_converter;

        [SetUp]
        public void SetUp()
        {
            m_converter = new AliorSyncCsvToQif();
        }

        [Test]
        public void AliorSyncShouldParseOneEntry()
        {
            var entries = m_converter.ConvertLinesToQif(testLines);
            Assert.AreEqual(entries.Count(), 1);
            var entry = entries.Single();
            Assert.AreEqual(-111.99, entry.Amount);
            Assert.AreEqual("", entry.AccountName);
            Assert.AreEqual("AAA BBB CCC 11001", entry.Payee);
            Assert.AreEqual(DateTime.Parse("2012-09-01"), entry.Date.BookingDate);
            Assert.AreEqual(DateTime.Parse("2012-09-01"), entry.Date.OperationDate);
            Assert.AreEqual(@"Transakcja kartą debetową;obciążenie 1111 OTHER111111-1111666666 POL AA BB CC DDD 55555",
                entry.Description);
        }
    }
}
