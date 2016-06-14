using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Blockquote : HtmlComponent
    {
        public Blockquote(string text) : base(text, TagTypes.BlockCode)
        {
            
        }

        public override string ToString()
        {
            return "<blockquote><p>" + Text + "</p></blockquote>";
        }
    }
}
