using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Paragraph : HtmlComponent
    {
        public Paragraph(string text) : base(text, TagTypes.Paragraph)
        {
            
        }

        public override string ToString()
        {
            return "<p>" + Text + "</p>";
        }
    }
}
