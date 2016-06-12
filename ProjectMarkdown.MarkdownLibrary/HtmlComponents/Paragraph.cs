using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Paragraph : HtmlComponent
    {
        private readonly string _text;

        public Paragraph(string text) : base(TagTypes.Paragraph)
        {
            _text = text;
        }

        public override string ToString()
        {
            return "<p>" + _text + "</p>";
        }
    }
}
