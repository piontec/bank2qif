using System;
using NUnit.Framework;
using Bank2Qif;

namespace Sync2QifTests
{
	[TestFixture]
	public class Sync2QifParserTests
	{
		[Test]
		public void QifNameTest ()
		{
            Assert.AreEqual("test.qif", QifFile.GetQifFileName("test.pdf"));
            Assert.AreEqual("sciezka/test.qif", QifFile.GetQifFileName("sciezka/test.pdf"));
            Assert.AreEqual("TEST.qif", QifFile.GetQifFileName("TEST.PDF"));
            Assert.Throws<ArgumentException>(() => QifFile.GetQifFileName("test"));
		}
	}
}

