using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank2Qif.Transformers.SimpleMatch
{
    public class SimpleMatchRule
    {
        public enum Operator
        {
            Like,
            Equal
        }

        public string FieldName { get; private set; }
        public Operator RuleOperator { get; private set; }
        public string Pattern { get; private set; }
        public string Result { get; private set; }

        public SimpleMatchRule(string fieldName, string pattern, Operator ruleOperator, string result)
        {
            FieldName = fieldName;
            Pattern = pattern;
            RuleOperator = ruleOperator;
            Result = result;
        }
    }
}
