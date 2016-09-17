using System;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace WpfCodeTextbox.Rules
{
    public class RuleOptions
    {
        public bool IgnoreCase { get; private set; }
        public Brush Foreground { get; private set; }
        public FontWeight FontWeight { get; private set; }
        public FontStyle FontStyle { get; private set; }

        public RuleOptions(XElement rule)
        {
            var ignoreCaseElement = rule.Element("IgnoreCase");
            var foregroundElement = rule.Element("Foreground");
            var fontWeightElement = rule.Element("FontWeight");
            var fontStyleElement = rule.Element("FontStyle");
            if (ignoreCaseElement != null && foregroundElement != null && fontWeightElement != null && fontStyleElement != null)
            {
                string ignoreCaseStr = ignoreCaseElement.Value.Trim();
                string foregroundStr = foregroundElement.Value.Trim();
                string fontWeightStr = fontWeightElement.Value.Trim();
                string fontStyleStr = fontStyleElement.Value.Trim();

                IgnoreCase = bool.Parse(ignoreCaseStr);
                Foreground = (Brush)new BrushConverter().ConvertFrom(foregroundStr);
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFrom(fontWeightStr);
                FontStyle = (FontStyle)new FontStyleConverter().ConvertFrom(fontStyleStr);
            }
            else
            {
                throw new NullReferenceException("One or more of rule elements are null!");
            }
        }
    }
}
