using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Xml.Linq;
using WpfCodeTextbox.Rules;

namespace WpfCodeTextbox
{
    public class XmlHighlighter : IHighlighter
    {
        private readonly List<HighlightWordsRule> _wordsRules = new List<HighlightWordsRule>();
        private readonly List<HighlightLineRule> _lineRules = new List<HighlightLineRule>();
        private readonly List<AdvancedHighlightRule> _regexRules = new List<AdvancedHighlightRule>();

        public XmlHighlighter(XElement root)
        {
            foreach (var element in root.Elements())
            {
                switch (element.Name.ToString())
                {
                    case "HighlightWordsRule":
                        _wordsRules.Add(new HighlightWordsRule(element));
                        break;
                    case "HighlightLineRule":
                        _lineRules.Add(new HighlightLineRule(element));
                        break;
                    case "AdvancedHighlightRule":
                        _regexRules.Add(new AdvancedHighlightRule(element));
                        break;
                }
            }
        }

        public int Highlight(FormattedText text, int previousBlockCode)
        {
            // WORDS RULES
            var wordsRgx = new Regex("[a-zA-Z_][a-zA-Z0-9_]*");
            foreach (Match m in wordsRgx.Matches(text.Text))
            {
                foreach (var rule in _wordsRules)
                {
                    foreach (string word in rule.Words)
                    {
                        if (rule.Options.IgnoreCase)
                        {
                            if (m.Value.Equals(word, StringComparison.InvariantCultureIgnoreCase))
                            {
                                text.SetForegroundBrush(rule.Options.Foreground, m.Index, m.Length);
                                text.SetFontWeight(rule.Options.FontWeight, m.Index, m.Length);
                                text.SetFontStyle(rule.Options.FontStyle, m.Index, m.Length);
                            }
                        }
                        else
                        {
                            if (m.Value == word)
                            {
                                text.SetForegroundBrush(rule.Options.Foreground, m.Index, m.Length);
                                text.SetFontWeight(rule.Options.FontWeight, m.Index, m.Length);
                                text.SetFontStyle(rule.Options.FontStyle, m.Index, m.Length);
                            }
                        }
                    }
                }
            }
            
            // REGEX RULES
            foreach (var rule in _regexRules)
            {
                var regexRgx = new Regex(rule.Expression);
                foreach (Match m in regexRgx.Matches(text.Text))
                {
                    text.SetForegroundBrush(rule.Options.Foreground, m.Index, m.Length);
                    text.SetFontWeight(rule.Options.FontWeight, m.Index, m.Length);
                    text.SetFontStyle(rule.Options.FontStyle, m.Index, m.Length);
                }
            }
            
            // LINES RULES
            foreach (var rule in _lineRules)
            {
                var lineRgx = new Regex(Regex.Escape(rule.LineStart) + ".*");
                foreach (Match m in lineRgx.Matches(text.Text))
                {
                    text.SetForegroundBrush(rule.Options.Foreground, m.Index, m.Length);
                    text.SetFontWeight(rule.Options.FontWeight, m.Index, m.Length);
                    text.SetFontStyle(rule.Options.FontStyle, m.Index, m.Length);
                }
            }

            return -1;
        }
    }
}