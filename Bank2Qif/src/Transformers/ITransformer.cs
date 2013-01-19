using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank2Qif;

namespace Bank2Qif.Transformers
{
    public interface ITransformer
    {
        IEnumerable<QifEntry> Transform(IEnumerable<QifEntry> entries);
    }
}
