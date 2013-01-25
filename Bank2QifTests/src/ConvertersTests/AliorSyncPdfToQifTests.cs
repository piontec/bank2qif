using System;
using NUnit.Framework;
using Bank2Qif;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bank2Qif.Converters.AliorSync;


namespace Bank2QifTests
{
	[TestFixture]
	public class AliorSyncPdfToQifTests
	{
		AliorSyncPdfToQif converter;				
        
		[SetUp]
		public void SetUp ()
		{
            
            converter = new AliorSyncPdfToQif();
		}

		[Test]
		public void TestBankStatement1 ()
		{
            CheckParsedDates(9, @"data\wyciag1.txt");
		}


        [Test]
        public void TestBankStatement2()
        {
            CheckParsedDates(10, @"data\wyciag2.txt");
        }


        private void CheckParsedDates(int expectedNumber, string fileName)
        {
            string[] lines = File.ReadAllLines(fileName, System.Text.Encoding.UTF8);
            IEnumerable<QifEntry> result = converter.ConvertExtractedTextToQif(lines);

            Assert.AreEqual(expectedNumber, result.Count());
            DateTime expectedDate = DateTime.Parse("2012.01.01");
            var table = result.ToArray();
            for (int i = 0; i < table.Count(); i++)
            {
                Assert.AreEqual(expectedDate.AddDays(i), table[i].Date.OperationDate);
                Assert.AreEqual(expectedDate.AddDays(i), table[i].Date.BookingDate);
            }
        }
	}
}

