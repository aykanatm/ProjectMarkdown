using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class HorizontalRule : HtmlComponent
    {
        public HorizontalRule() : base(null, TagTypes.HorizontalRule)
        {
            
        }

        public override string ToString()
        {
            return "<hr>";
        }
    }
}
