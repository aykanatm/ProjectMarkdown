using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class OrderedList : HtmlComponent
    {
        private readonly List<ListItem> _items;

        public OrderedList(List<ListItem> items) : base(TagTypes.OrderedList)
        {
            _items = items;
        }

        public override string ToString()
        {
            string output = "<ul>";

            for (var i = 0; i < _items.Count; i++)
            {
                output += _items[i].ToString();
            }

            output += "</ul>";

            return output;
        }
    }
}
