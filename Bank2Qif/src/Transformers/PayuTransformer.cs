using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;


namespace Bank2Qif.Transformers
{
    [Transformer(10)]
    public class PayuTransformer : BaseTransformer, ITransformer
    {
        public PayuTransformer(IConfig config)
        {
            Initialize(config);
        }

        public IEnumerable<QifEntry> Transform(IEnumerable<QifEntry> entries)
        {
            if (!m_enabled)
                return entries;

            foreach (var entry in entries)
                if (IsPayuTransaction(entry))
                    ProcessPayuTransaction(entry);

            return entries;
        }


        private void ProcessPayuTransaction(QifEntry entry)
        {
            throw new NotImplementedException();
        }


        private bool IsPayuTransaction(QifEntry entry)
        {
            throw new NotImplementedException();
        }
    }
}
