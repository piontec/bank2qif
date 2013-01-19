using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImapX;

namespace Bank2Qif.Services
{
    public class ImapxSearcher : IImapSearcher
    {        
        public MessageCollection FetchMessages(string host, string login, string pass, string imapQuery)
        {
            var client = new ImapX.ImapClient(host, 993, true);
            client.Connection();
            client.LogIn(login, pass);            

            var messages = client.Folders["INBOX"].Search(imapQuery, false);
            foreach (var m in messages)
                m.Process();

            client.Disconnect();           

            return messages;
        }
    }
}
