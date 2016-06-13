using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class UnorderedList : HtmlComponent
    {
        public int Count { get; private set; }
        private readonly List<ListItem> _items;

        public UnorderedList(List<ListItem> items) : base(TagTypes.UnorderedList)
        {
            _items = items;
            Count = _items.Count;
        }

        public override string ToString()
        {
            string output = "<ul>\r\n";

            for (var i = 0; i < _items.Count; i++)
            {
                output += _items[i] + "\r\n";
            }

            output += "</ul>";

            return output;
        }
    }
}
