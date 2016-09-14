using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfCodeTextbox.Rules
{
    public class HighlightWordsRule
    {
        public List<string> Words { get; private set; }
        public RuleOptions Options { get; private set; }
        public HighlightWordsRule(XElement rule)
        {
            if (rule != null)
            {
                Words = new List<string>();
                Options = new RuleOptions(rule);

                var element = rule.Element("Words");
                if (element != null)
                {
                    string wordsStr = element.Value;
                    string[] words = Regex.Split(wordsStr, "\\s+");

                    foreach (string word in words)
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            Words.Add(word.Trim());
                        }
                    }
                }
                else
                {
                    throw new NullReferenceException("Element 'Words' is null!");
                }
            }
            else
            {
                throw new NullReferenceException("Rule is null!");
            }
        }
    }
}
