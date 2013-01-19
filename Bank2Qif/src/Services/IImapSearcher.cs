using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImapX;

namespace Bank2Qif.Services
{
    /// <summary>
    /// Provides search capabilities for mail boxes
    /// </summary>
    public interface IImapSearcher : IService
    {
        /// <summary>
        /// Fetches messeges from a given host and described by an IMAP query. Throws communication
        /// exceptions.
        /// </summary>
        /// <param name="host">IMAP host</param>
        /// <param name="login">IMAP account login</param>
        /// <param name="pass">IMAP account password</param>
        /// <param name="imapQuery">IMAP query</param>
        /// <returns>A collection of messages found</returns>
        MessageCollection FetchMessages(string host, string login, string pass, string imapQuery);
        
    }
}
