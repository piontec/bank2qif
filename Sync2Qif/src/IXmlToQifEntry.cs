using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;


namespace Sync2Qif
{
	public interface IXmlToQifEntry
	{
		IList<QifEntry> ConvertXmlToQif (XDocument xml);
	}
}

