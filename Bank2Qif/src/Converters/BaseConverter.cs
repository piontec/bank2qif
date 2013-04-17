using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Bank2Qif.Converters
{
    public abstract class BaseConverter : IConverter
    {
        public IList<QifEntry> ConvertFileToQif(string fileName)
        {
            string lines = File.ReadAllText(fileName);

            return ConvertLinesToQif(lines);
        }


        public abstract IList<QifEntry> ConvertLinesToQif(string lines);
    }
}
