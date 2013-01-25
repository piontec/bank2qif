﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;
using System.IO;
using Sprache;

namespace Bank2Qif.Transformers.SimpleMatch
{
    [Transformer(100)]
    public class SimpleMatchTransformer : BaseTransformer, ITransformer
    {
        private readonly string m_rulesFile;
        private static readonly string CFG_RULES_FILE_NAME = "RulesFile";
        private static readonly string CFG_RULES_FILE_NAME_DFLT = "rules.txt";
        private static readonly string COMMENT_START = "#";

        public SimpleMatchTransformer(IConfig config)
        {
            base.Initialize(config);
            m_rulesFile = m_config.GetString(CFG_RULES_FILE_NAME, CFG_RULES_FILE_NAME_DFLT);
        }


        public IEnumerable<QifEntry> Transform(IEnumerable<QifEntry> entries)
        {
            var nonCommentLines = File.ReadAllLines(m_rulesFile).
                Where(line => line.Trim().StartsWith(COMMENT_START) == false).
                Aggregate((s1, s2) => string.Format("{0}{1}", s1, s2));

            foreach (var rule in SimpleMatchRuleParsers.SimpleRules.Parse(nonCommentLines))
                entries = rule.Transform(entries);

            return entries;
        }
    }
}
