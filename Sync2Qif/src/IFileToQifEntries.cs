using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;


namespace Sync2Qif
{
	public interface IFileToQifEntries
	{
		IList<QifEntry> ConvertFileToQif (string fileName);
	}
}

