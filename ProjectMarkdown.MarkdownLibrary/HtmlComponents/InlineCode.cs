using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class InlineCode : HtmlComponent
    {
        private readonly string _text;

        public InlineCode(string text) : base(TagTypes.InlineCode)
        {
            _text = text;
        }

        public override string ToString()
        {
            return "<code>" + _text + "</code>";
        }
    }
}
