using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Bold : HtmlComponent
    {
        public Bold(string text) : base(text, TagTypes.Bold)
        {
            
        }

        public override string ToString()
        {
            return "<b>" + Text + "</b>";
        }
    }
}
