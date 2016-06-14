using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class OrderedList : HtmlComponent
    {
        public int Count { get; private set; }
        private readonly List<ListItem> _items;

        public OrderedList(List<ListItem> items) : base(null, TagTypes.OrderedList)
        {
            _items = items;
            Count = _items.Count;
        }

        public override string ToString()
        {
            string output = "<ol>\r\n";

            for (var i = 0; i < _items.Count; i++)
            {
                output += _items[i] + "\r\n";
            }

            output += "</ol>";

            return output;
        }
    }
}
