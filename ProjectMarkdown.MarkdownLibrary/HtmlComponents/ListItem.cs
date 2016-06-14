using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class ListItem : HtmlComponent
    {
        public ListItem(string text) : base(text, TagTypes.ListItem)
        {
            
        }

        public override string ToString()
        {
            return "<li>" + Text + "</li>";
        }
    }
}
