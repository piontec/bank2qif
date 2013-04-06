using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Bank2Qif.Converters
{
    public abstract class BaseConverter : IConverter
    {
        public IEnumerable<QifEntry> ConvertFileToQif(string fileName)
        {
            string lines = File.ReadAllText(fileName, System.Text.Encoding.Default);

            return ConvertLinesToQif(lines);
        }


        public abstract IEnumerable<QifEntry> ConvertLinesToQif(string lines);
    }
}
