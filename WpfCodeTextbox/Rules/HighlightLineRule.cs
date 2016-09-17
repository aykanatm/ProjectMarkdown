using System;
using System.Xml.Linq;

namespace WpfCodeTextbox.Rules
{
    public class HighlightLineRule
    {
        public string LineStart { get; private set; }
        public RuleOptions Options { get; private set; }

        public HighlightLineRule(XElement rule)
        {
            if (rule != null)
            {
                var element = rule.Element("LineStart");
                if (element != null)
                {
                    LineStart = element.Value.Trim();
                    Options = new RuleOptions(rule);
                }
                else
                {
                    throw new NullReferenceException("Element 'LineStart' is null!");
                }
            }
            else
            {
                throw new NullReferenceException("Rule is null!");
            }
        }
    }
}
