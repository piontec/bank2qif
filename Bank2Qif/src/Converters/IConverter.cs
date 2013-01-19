using System;
using System.Collections;
using System.Collections.Generic;


namespace Bank2Qif.Converters
{
	public interface IConverter
	{
		IEnumerable<QifEntry> ConvertFileToQif (string fileName);
	}
}

