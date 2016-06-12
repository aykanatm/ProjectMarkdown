using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Link : HtmlComponent
    {
        private readonly string _text;
        private readonly string _url;

        public Link(string text, string url) : base(TagTypes.Link)
        {
            _text = text;
            _url = url;
        }

        public override string ToString()
        {
            return "<a href=\"" + _url + "\">" + _text + "</a>";
        }
    }
}
