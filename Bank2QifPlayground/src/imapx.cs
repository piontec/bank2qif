using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ImapX;
using ImapX.EmailParser;

namespace Bank2QifPlayground.src
{
    public class ImapxTest
    {
        public static string searchPayu = "Pay by link PayU";
        public void Run()
        {
            bool html;
            var found = FetchMessages("imap.gmail.com", "login", "pass",
                "PayU", DateTime.Parse ("2012.10.01"), DateTime.Parse ("2012.11.01"));

            var match = from m in found
                        let txt = m.GetDecodedBody(out html)
                        where txt.IndexOf("2694621696)") > 0
                        select m;

            foreach (var m in match)
            {
                Console.WriteLine(m.From.FirstOrDefault() + " " + m.Subject);
                Console.WriteLine(m.GetDecodedBody (out html));                
            }
        }


        private MessageCollection FetchMessages(string host, string login, string pass,
            string from, DateTime startDate, DateTime endDate)
        {
            var client = new ImapX.ImapClient(host, 993, true);
            client.Connection();
            client.LogIn(login, pass);
            string imapQuery = string.Format("FROM \"{0}\" BEFORE {1} SINCE {2}",
                from, endDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
                startDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture));            

            var messages = client.Folders["INBOX"].Search(imapQuery, false);
            foreach (var m in messages)
                m.Process();
        
            return messages;
        }
    }
}
