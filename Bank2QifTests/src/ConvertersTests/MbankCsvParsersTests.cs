using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Converters;
using Bank2Qif.Converters.MBank;
using NUnit.Framework;

namespace Bank2QifTests.ConvertersTests
{
    [TestFixture]
    public class MbankCsvParsersTest
    {
        private const string testLines =
        "#Data operacji;#Data księgowania;#Opis operacji;#Tytuł;#Nadawca/Odbiorca;#Numer konta;#Kwota;#Saldo po operacji;"
        + "\r\n"
        + "2012-01-01;2012-01-02;PRZELEW MTRANSFER WYCHODZACY;\"PLACE Z ALLEGRO XX111100111XX WPŁATA ŁĄCZNA OD XXX\";\"PAYU SPÓŁKA AKCYJNA  UL.MARCELIŃSKA 90                  60-324 POZNAŃ POLSKA\";'11222233334444555566667777';-10,50;11,10;";
 

        private IConverter m_converter;


        [SetUp]
        public void SetUp()
        {
            m_converter = new MBankCsvToQif();
        }


        [Test]
        public void ShouldParseOneEntry()
        {
            var entries = m_converter.ConvertLinesToQif(testLines);
            Assert.AreEqual(entries.Count(), 1);
            var entry = entries.Single ();
            Assert.AreEqual(-10.5, entry.Amount);
            Assert.AreEqual("11222233334444555566667777", entry.AccountName);
            Assert.AreEqual("PAYU SPÓŁKA AKCYJNA  UL.MARCELIŃSKA 90                  60-324 POZNAŃ POLSKA", entry.Payee);
            Assert.AreEqual(DateTime.Parse ("2012-01-02"), entry.Date.BookingDate);
            Assert.AreEqual(DateTime.Parse ("2012-01-01"), entry.Date.OperationDate);
        }
    }
}

			

