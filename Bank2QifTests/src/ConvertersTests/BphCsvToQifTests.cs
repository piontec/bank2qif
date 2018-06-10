using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif.Converters;
using Bank2Qif.Converters.BPH;
using NUnit.Framework;

namespace Bank2QifTests.ConvertersTests
{
	[TestFixture]
	public class BphCsvToQifTests
	{
		private const string testLines =
			"//Unikalny identyfikator|Data księgowania|Data efektywnej operacji|Rachunek nadawcy/odbiorcy|Nazwa nadawcy/odbiorcy|Tytułem|Opis operacji|Kwota|Typ|Notatka|Saldo|"
			+ "\r\n"
			+ "1111111111111110001|28-12-2015|24-12-2015|11 2222 2222 2222 2222 2222 2222| NADAWCA |OPIS OD NADAWCY|Transakcja kartą                   |-22.33|Obciążenie|Transakcja kartą                   ||1 111.11|";

		private IConverter m_converter;


		[SetUp]
		public void SetUp()
		{
			m_converter = new BphCsvToQif();
		}


		[Test]
		public void BphShouldParseOneEntry()
		{
			var entries = m_converter.ConvertLinesToQif(testLines);
			Assert.AreEqual(1, entries.Count());
			var entry = entries.Single ();
			Assert.AreEqual(-22.33, entry.Amount);
			Assert.AreEqual("11 2222 2222 2222 2222 2222 2222", entry.AccountName);
			Assert.AreEqual("NADAWCA", entry.Payee);
			Assert.AreEqual ("Transakcja kartą: OPIS OD NADAWCY", entry.Description);
			Assert.AreEqual(DateTime.Parse ("2015-12-28"), entry.Date.BookingDate);
			Assert.AreEqual(DateTime.Parse ("2015-12-24"), entry.Date.OperationDate);
		}
	}
}



