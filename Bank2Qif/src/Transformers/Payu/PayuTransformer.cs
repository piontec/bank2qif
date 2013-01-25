using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nini.Config;
using ImapX;
using Bank2Qif.Parsers;
using Sprache;
using Bank2Qif.Services.ImapSearcher;


namespace Bank2Qif.Transformers.Payu
{
    [Transformer(10)]
    public class PayuTransformer : BaseTransformer, ITransformer
    {   
        private readonly string PAYU_MARK = "PayU XX";
        private readonly string PAYU_ALLEGRO_MARK = "PayU w Allegro XX";
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
                {
                    allegroPayuEntries.Add(entry);
                    continue;
                }
                if (IsPayuTransaction(entry))
                    plainPayuEntries.Add(entry);
            }

            ProcessEntries(allegroPayuEntries, plainPayuEntries);

            return entries;
        }


        private void ProcessEntries(IList<QifEntry> allegroPayuEntries, IList<QifEntry> plainPayuEntries)
        {
            if (allegroPayuEntries.Count() == 0 && plainPayuEntries.Count() == 0)
                return;

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
            try
            {
                Tuple<string, string> ids = PayuParsers.SyncAndAllegroIdParser.Parse(entry.Description);

                bool html;
                var validEmailBodys = from mail in emails
                                      let txt = mail.GetDecodedBody(out html)
                                      where txt.IndexOf(ids.Item2) > 0
                                      select txt;

                if (validEmailBodys.Count() > 0)
                    ProcessPayuAllegroEmails(entry, validEmailBodys);
            }
            catch (ParseException) { }
        }


        private void ProcessPayuAllegroEmails(QifEntry entry, IEnumerable<string> validEmailBodys)
        {
            foreach (var body in validEmailBodys)
            {
                string description = string.Empty;
                try
                {
                    description = PayuParsers.QuickAndDirtyHtmlInfoParser.Parse(body);
                }
                catch (ParseException) { }

                if (description != string.Empty)
                {
                    entry.Description = string.Format("Allegro: {0} - {1}", description, entry.Description);
                    break;
                }
            }
        }


        private void ProcessPayuTransaction(QifEntry entry, MessageCollection emails)
        {
            string payuId = PayuParsers.SyncIdParser.Parse(entry.Description);

            bool html;
            var validEmailBodys = from mail in emails
                                  let txt = mail.GetDecodedBody(out html)
                                  where txt.IndexOf(payuId) > 0
                                  select txt;

            if (validEmailBodys.Count() > 0)
                ProcessPayuEmails(entry, validEmailBodys);
        }


        private void ProcessPayuEmails(QifEntry entry, IEnumerable<string> validEmailBodys)
        {
            foreach (var body in validEmailBodys)
            {
                string receiver = string.Empty;
                try
                {
                    receiver = PayuParsers.SyncPaymentReceiver.Parse(body);
                }
                catch (ParseException) { }

                if (receiver == string.Empty)
                    try
                    {
                        receiver = PayuParsers.SyncPaymentStupidInfo.Parse(body);
                    }
                    catch (ParseException) { }

                if (receiver != string.Empty)
                {
                    entry.Description = string.Format("{0} - {1}", receiver, entry.Description);
                    break;
                }
            }
        }
                


        private bool IsPayuAllegroTransaction(QifEntry entry)
        {
            return entry.Description.Contains(PAYU_ALLEGRO_MARK);
        }
        

        private bool IsPayuTransaction(QifEntry entry)
        {
            return entry.Description.Contains(PAYU_MARK);
        }


        private string GetImapQueryForDateRange(DateTime startDate, DateTime endDate)
        {
            return string.Format("FROM \"PayU\" BEFORE {0} SINCE {1}",
                    endDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                    startDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture));
        }
    }
}
