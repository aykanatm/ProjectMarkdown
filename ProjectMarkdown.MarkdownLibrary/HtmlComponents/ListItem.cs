using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class ListItem : HtmlComponent
    {
        private readonly string _text;

        public ListItem(string text) : base(TagTypes.ListItem)
        {
            _text = text;
        }

        public override string ToString()
        {
            return "<li>" + _text + "</li>";
        }
    }
}
