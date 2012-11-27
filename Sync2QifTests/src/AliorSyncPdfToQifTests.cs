using System;
using NUnit.Framework;
using Sync2Qif.Converters;
using Sync2Qif;
using System.Xml.Linq;
using System.Collections.Generic;


namespace Sync2QifTests
{
	[TestFixture]
	public class AliorSyncPdfToQifTests
	{
		AliorSyncPdfToQif converter;
		XDocument xml;
		IList<QifEntry> res;

		[SetUp]
		public void SetUp ()
		{
            //xml = XDocument.Load("../../data/wyciag1.xml", LoadOptions.PreserveWhitespace);
			//converter = new AliorSyncPdfToQif ();
			//res = converter.ConvertFileToQif (xml);
		}

		[Test]
		public void Should_return_all_operations ()
		{

			Assert.AreEqual (9, res.Count);
		}
	}
}

