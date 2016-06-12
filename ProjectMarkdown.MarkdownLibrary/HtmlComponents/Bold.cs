using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Bold : HtmlComponent
    {
        private readonly string _text;

        public Bold(string text) : base(TagTypes.Bold)
        {
            _text = text;
        }

        public override string ToString()
        {
            return "<b>" + _text + "</b>";
        }
    }
}
