using System;
using System.Xml.Linq;

namespace WpfCodeTextbox.Rules
{
    public class AdvancedHighlightRule
    {
        public string Expression { get; private set; }
        public RuleOptions Options { get; private set; }

        public AdvancedHighlightRule(XElement rule)
        {
            if (rule != null)
            {
                var element = rule.Element("Expression");
                if (element != null)
                {
                    Expression = element.Value.Trim();
                    Options = new RuleOptions(rule);
                }
                else
                {
                    throw new NullReferenceException("Element 'Expression' is null!");
                }
            }
            else
            {
                throw new NullReferenceException("Rule is null!");
            }
        }
    }
}
