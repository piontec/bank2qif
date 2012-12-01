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
			var parser = new Sync2QifParser ();

			Assert.AreEqual ("test.qif", parser.GetQifFileName ("test.pdf"));
			Assert.AreEqual ("sciezka/test.qif", parser.GetQifFileName ("sciezka/test.pdf"));
			Assert.AreEqual ("TEST.qif", parser.GetQifFileName ("TEST.PDF"));
			Assert.Throws<ApplicationException> ( () => parser.GetQifFileName ("test.fty"));
		}
	}
}

