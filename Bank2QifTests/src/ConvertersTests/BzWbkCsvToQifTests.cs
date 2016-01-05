using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Converters;
using Bank2Qif.Converters.BzWbk;
using NUnit.Framework;

namespace Bank2QifTests.ConvertersTests
{
	[TestFixture]
	public class BzWbkCsvToQifTests
	{
		private const string testLines =
			"2016-01-05,18-11-2015,'11 2222 2222 2222 2222 2222 2222,Pan JAN KOWALSKI ŻADNA 1/2 11-222 PCIM,PLN,\"0,00\",\"1111,11\",11,"
			+ "\r\n"
			+ "25-11-2015,23-11-2015,TO JEST OPIS,TO JEST NADAWCA I ADRES ELIXIR 24-11-2015,11 2222 2222 2222 2222 2222 2222,\"1111,00\",\"2222,00\",2,";


		private IConverter m_converter;


		[SetUp]
		public void SetUp()
		{
			m_converter = new BzWbkCsvToQif();
		}


		[Test]
		public void BzWbkShouldParseOneEntry()
		{
			var entries = m_converter.ConvertLinesToQif(testLines);
			Assert.AreEqual(1, entries.Count());
			var entry = entries.Single ();
			Assert.AreEqual(1111, entry.Amount);
			Assert.AreEqual("11 2222 2222 2222 2222 2222 2222", entry.AccountName);
			Assert.AreEqual("TO JEST NADAWCA I ADRES ELIXIR 24-11-2015", entry.Payee);
			Assert.AreEqual(DateTime.Parse ("2015-11-25"), entry.Date.BookingDate);
			Assert.AreEqual(DateTime.Parse ("2015-11-23"), entry.Date.OperationDate);
		}
	}
}



