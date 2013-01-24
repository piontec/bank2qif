using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;
using System.IO;

namespace Bank2Qif.Transformers
{
    [Transformer(100)]
    public class SimpleMatchTransformer : BaseTransformer, ITransformer
    {
        private static readonly string s_rulesFile;
        private static readonly string CFG_RULES_FILE_NAME = "RulesFile";
        private static readonly string CFG_RULES_FILE_NAME_DFLT = "rules.txt";

        public SimpleMatchTransformer(IConfig config)
        {
            base.Initialize(config);
            s_rulesFile = m_config.GetString(CFG_RULES_FILE_NAME, CFG_RULES_FILE_NAME_DFLT);
        }


        public IEnumerable<QifEntry> Transform(IEnumerable<QifEntry> entries)
        {
            throw new NotImplementedException();
        }
    }
}
