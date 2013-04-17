using System;
using System.Collections;
using System.Collections.Generic;


namespace Bank2Qif.Converters
{
	public interface IConverter
	{
		IList<QifEntry> ConvertFileToQif (string fileName);
        IList<QifEntry> ConvertLinesToQif (string lines);
	}
}

