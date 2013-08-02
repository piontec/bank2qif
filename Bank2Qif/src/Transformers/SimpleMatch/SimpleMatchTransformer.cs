using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nini.Config;
using Sprache;


namespace Bank2Qif.Transformers.SimpleMatch
{
    [Transformer (priority: 100)]
    public class SimpleMatchTransformer : BaseTransformer, ITransformer
    {
        private readonly string m_rulesFile;
        private const string CFG_RULES_FILE_NAME = "RulesFile";
        private const string CFG_RULES_FILE_NAME_DFLT = "rules.txt";
        private const string COMMENT_START = "#";


        public SimpleMatchTransformer (IConfig config)
        {
            Initialize (config);
            m_rulesFile = Runner.CONFIG_DIR + m_config.GetString (CFG_RULES_FILE_NAME, CFG_RULES_FILE_NAME_DFLT);
        }


        public IEnumerable<QifEntry> Transform (IEnumerable<QifEntry> entries)
        {
            var nonCommentLines = File.ReadAllLines (m_rulesFile).
                                       Where (line => line.Trim ().StartsWith (COMMENT_START) == false).
                                       Aggregate ((s1, s2) => string.Format ("{0}{1}{2}", s1, Environment.NewLine, s2));

            var rules = SimpleMatchRuleParsers.SimpleRules.Parse (nonCommentLines);
            foreach (var rule in rules)
                entries = rule.Transform (entries);

            return entries;
        }
    }
}