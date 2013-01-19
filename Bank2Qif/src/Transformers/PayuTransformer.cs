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
        private readonly string PAYU_PREFIX = "PRZELEW WEWNĘTRZNY - PŁACĘ Z ALIOR BANKIEM:";
        private readonly string PAYU_MARK = "Pay by link PayU X";
        private readonly string PAYU_ALLEGRO_MARK = "Pay by link PayU w Allegro X";

        public PayuTransformer(IConfig config)
        {
            Initialize(config);
        }

        public IEnumerable<QifEntry> Transform(IEnumerable<QifEntry> entries)
        {
            if (!m_enabled)
                return entries;

            foreach (var entry in entries)
            {
                if (IsPayuAllegroTransaction(entry))
                    ProcessPayuAllegroTransaction(entry);
                if (IsPayuTransaction(entry))
                    ProcessPayuTransaction(entry);
            }

            return entries;
        }

        private void ProcessPayuAllegroTransaction(QifEntry entry)
        {
            throw new NotImplementedException();
        }

        private bool IsPayuAllegroTransaction(QifEntry entry)
        {
            return entry.Description.StartsWith(PAYU_PREFIX) && entry.Description.Contains(PAYU_ALLEGRO_MARK);
        }


        private void ProcessPayuTransaction(QifEntry entry)
        {
            throw new NotImplementedException();
        }


        private bool IsPayuTransaction(QifEntry entry)
        {
            return entry.Description.StartsWith(PAYU_PREFIX) && entry.Description.Contains(PAYU_MARK);
        }
    }
}
