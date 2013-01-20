using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nini.Config;
using Bank2Qif.Services;
using ImapX;


namespace Bank2Qif.Transformers
{
    [Transformer(10)]
    public class PayuTransformer : BaseTransformer, ITransformer
    {
        private readonly string PAYU_PREFIX = "PRZELEW WEWNĘTRZNY - PŁACĘ Z ALIOR BANKIEM:";
        private readonly string PAYU_MARK = "Pay by link PayU X";
        private readonly string PAYU_ALLEGRO_MARK = "Pay by link PayU w Allegro X";
        private readonly string CFG_HOST = "ImapServer";
        private readonly string CFG_USER = "ImapUser";
        private readonly string CFG_PASS = "ImapPassword";
        private readonly IImapSearcher m_mailSearcher;
        

        public PayuTransformer(IConfig config, IImapSearcher mailSearcher)
        {
            Initialize(config);
            m_mailSearcher = mailSearcher;
        }


        public IEnumerable<QifEntry> Transform(IEnumerable<QifEntry> entries)
        {
            if (!m_enabled)
                return entries;

            IList<QifEntry> plainPayuEntries = new List<QifEntry>();
            IList<QifEntry> allegroPayuEntries = new List<QifEntry>();

            foreach (var entry in entries)
            {
                if (IsPayuAllegroTransaction(entry))
                    allegroPayuEntries.Add(entry);
                if (IsPayuTransaction(entry))
                    plainPayuEntries.Add(entry);
            }

            ProcessEntries(allegroPayuEntries, plainPayuEntries);

            return entries;
        }


        private void ProcessEntries(IList<QifEntry> allegroPayuEntries, IList<QifEntry> plainPayuEntries)
        {
            MessageCollection emails = FetchEmails(allegroPayuEntries, plainPayuEntries);

            if (emails == null)
                return;

            foreach (var entry in allegroPayuEntries)
                ProcessPayuAllegroTransaction(entry, emails);

            foreach (var entry in plainPayuEntries)
                ProcessPayuTransaction(entry, emails);
                
        }


        private MessageCollection FetchEmails(IList<QifEntry> allegroPayuEntries, IList<QifEntry> plainPayuEntries)
        {
            MessageCollection result = null;

            DateTime minDate = allegroPayuEntries.Concat(plainPayuEntries).Min(e => e.Date.OperationDate);
            DateTime maxDate = allegroPayuEntries.Concat(plainPayuEntries).Max(e => e.Date.OperationDate);

            // we assume, that the required mails about transactions cannot be received later than one month
            // after the operation and one week before
            minDate = minDate.AddDays(-7);
            maxDate = maxDate.AddMonths(1);

            string host, user, pass;

            try
            {
                host = m_config.GetString(CFG_HOST);
                user = m_config.GetString(CFG_USER);
                pass = m_config.GetString(CFG_PASS);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine("Error in configuration file of Payu Transformer");
                return result;
            }

            try
            {
                result = m_mailSearcher.FetchMessages(host, user, pass, GetImapQueryForDateRange(minDate, maxDate));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine("Error communicating with mail server");
            }

            return result;
        }


        private void ProcessPayuAllegroTransaction(QifEntry entry, MessageCollection emails)
        {
            throw new NotImplementedException();
        }


        private bool IsPayuAllegroTransaction(QifEntry entry)
        {
            return entry.Description.StartsWith(PAYU_PREFIX) && entry.Description.Contains(PAYU_ALLEGRO_MARK);
        }


        private void ProcessPayuTransaction(QifEntry entry, MessageCollection emails)
        {
            throw new NotImplementedException();
        }


        private bool IsPayuTransaction(QifEntry entry)
        {
            return entry.Description.StartsWith(PAYU_PREFIX) && entry.Description.Contains(PAYU_MARK);
        }


        private string GetImapQueryForDateRange(DateTime startDate, DateTime endDate)
        {
            return string.Format("FROM \"PayU\" BEFORE {0} SINCE {1}",
                    endDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                    startDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture));
        }
    }
}
