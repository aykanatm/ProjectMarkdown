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

        private Highlight _highlight;

        public BlockCode(string text, Highlight highlight) : base(TagTypes.BlockCode)
        {
            _text = text;
            _highlight = highlight;
        }

        public override string ToString()
        {
            switch (_highlight)
            {
                case Highlight.NoHighlight: return "<pre=\"no-highlight\"><code>" + _text + "</code></pre>";
                case Highlight.Python: return "<pre=\"highlight highlight-source-python\"><code>" + _text + "</code></pre>";
                case Highlight.Javascript: return "<pre=\"highlight highlight-source-js\"><code>" + _text + "</code></pre>";
                default:
                    return string.Empty;
            }
        }
    }
}
