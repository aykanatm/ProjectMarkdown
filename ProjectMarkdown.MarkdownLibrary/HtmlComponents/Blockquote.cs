using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Blockquote : HtmlComponent
    {
        private readonly string _text;

        public Blockquote(string text) : base(TagTypes.BlockCode)
        {
            _text = text;
        }

        public override string ToString()
        {
            return "<blockquote>" + _text + "</blockquote>";
        }
    }
}
