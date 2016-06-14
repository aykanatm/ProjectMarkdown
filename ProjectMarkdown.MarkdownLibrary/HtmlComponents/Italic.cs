using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Italic : HtmlComponent
    {
        public Italic(string text) : base(text, TagTypes.Italic)
        {
            
        }

        public override string ToString()
        {
            return "<i>" + Text + "</i>";
        }
    }
}
