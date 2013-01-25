using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bank2Qif.Transformers.SimpleMatch
{
    public class SimpleMatchRule : ITransformer
    {
        public enum Operator
        {
            None,
            Like,
            Equal
        }

        public string FieldName { get; private set; }
        public Operator RuleOperator { get; private set; }
        public string Pattern { get; private set; }
        public string Result { get; private set; }
        private readonly PropertyInfo m_property;
        private static readonly string ANY_FIELD_NAME = "any";

        public SimpleMatchRule(string fieldName, Operator ruleOperator, string pattern, string result)
        {
            FieldName = fieldName;
            Pattern = pattern;
            RuleOperator = ruleOperator;
            Result = result;
            m_property = typeof(QifEntry).GetProperty(fieldName);
            if (m_property == null)
                throw new ArgumentException(string.Format("{0} is not a correct field to match on.",
                    m_property), "fieldName");
        }


        public IEnumerable<QifEntry> Transform(IEnumerable<QifEntry> entries)
        {
            foreach (var entry in entries)
            {
                string propValue = FieldName == ANY_FIELD_NAME ? entry.ToString()
                    : m_property.GetGetMethod().Invoke(entry, null).ToString ();

                bool isMatch = RuleOperator == SimpleMatchRule.Operator.Equal ?
                    propValue == Pattern :
                    propValue.IndexOf(Pattern, StringComparison.CurrentCultureIgnoreCase) >= 0;

                if (isMatch)
                    entry.AccountName = Result;
            }

            return entries;
        }
    }
}
