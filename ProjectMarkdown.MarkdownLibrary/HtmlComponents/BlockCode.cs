using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class BlockCode : HtmlComponent
    {
        private readonly string _text;

        public enum Highlight
        {
            Javascript,Python,NoHighlight
        }

        private readonly Highlight _highlight;

        public BlockCode(string text, Highlight highlight) : base(TagTypes.BlockCode)
        {
            _text = text;
            _highlight = highlight;
        }

        public override string ToString()
        {
            switch (_highlight)
            {
                case Highlight.NoHighlight: return "<div>\r\n<pre lang=\"no-highlight\">\r\n<code>" + _text + "\r\n</code>\r\n</pre>\r\n</div>\r\n";
                case Highlight.Python: return "<div class=\"highlight highlight-source-python\">\r\n<pre>\r\n<code>" + _text + "</code>\r\n</pre>\r\n</div>\r\n";
                case Highlight.Javascript: return "<div class=\"highlight highlight-source-js\">\r\n<pre>\r\n<code>" + _text + "</code>\r\n</pre>\r\n</div>\r\n";
                default:
                    return string.Empty;
            }
        }
    }
}
