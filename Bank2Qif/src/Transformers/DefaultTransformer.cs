﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;

namespace Bank2Qif.Transformers
{
    [Transformer (0)]
    public class DefaultTransformer : ITransformer
    {
        private const string INI_ACCOUNT = "Account";
        private const string INI_ACCOUNT_DFLT = "QifImport";
        private readonly string m_accountName;        


        public DefaultTransformer(IConfig config)
        {            
            m_accountName = config.GetString(INI_ACCOUNT, INI_ACCOUNT_DFLT);
        }

        public IEnumerable<QifEntry> Transform(IEnumerable<QifEntry> entries)
        {
            return entries.AsParallel().Select(e => { e.AccountName = m_accountName; return e; });
        }
    }
}
