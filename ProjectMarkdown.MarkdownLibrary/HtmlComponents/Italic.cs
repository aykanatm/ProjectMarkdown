using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Italic : HtmlComponent
    {
        private readonly string _text;

        public Italic(string text) : base(TagTypes.Italic)
        {
            _text = text;
        }

        public override string ToString()
        {
            return "<i>" + _text + "</i>";
        }
    }
}
