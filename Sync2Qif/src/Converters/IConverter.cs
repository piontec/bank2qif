using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;


namespace Bank2Qif.Converters
{
	public interface IConverter
	{
		IList<QifEntry> ConvertFileToQif (string fileName);
	}
}

