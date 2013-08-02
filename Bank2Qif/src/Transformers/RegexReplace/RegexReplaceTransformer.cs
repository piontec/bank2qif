using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nini.Config;


namespace Bank2Qif.Transformers.RegexReplace
{
    [Transformer (priority: 110)]
    public class RegexReplaceTransformer : BaseTransformer, ITransformer
    {
        public RegexReplaceTransformer (IConfig config)
        {
            Initialize (config);
        }


        public IEnumerable<QifEntry> Transform (IEnumerable<QifEntry> entries)
        {
            foreach (var entry in entries)
            {
                entry.Description = ReplaceMultispace (entry.Description);
                entry.Payee = ReplaceMultispace (entry.Payee);
            }

            return entries;
        }


        private string ReplaceMultispace (string text)
        {
            return Regex.Replace (text, " +", " ");
        }
    }
}