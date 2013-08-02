using System.Collections.Generic;


namespace Bank2Qif.Transformers
{
    public interface ITransformer
    {
        IEnumerable<QifEntry> Transform (IEnumerable<QifEntry> entries);
    }
}