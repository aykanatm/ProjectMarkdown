using System.Collections.Generic;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class List : HtmlComponent
    {
        private int _count;
        public int Count
        {
            get
            {
                int totalItemCount = 0;
                foreach (var item in _items)
                {
                    if (item is ListItem)
                    {
                        totalItemCount += 1;
                    }
                    else
                    {
                        var list = (List) item;
                        totalItemCount += list.Count;
                    }
                }
                return totalItemCount;
            }
            private set { _count = value; }
        }

        private readonly List<HtmlComponent> _items;

        public enum ListTypes
        {
            Ordered, Unordered
        }

        private readonly ListTypes _listType;

        public List(List<HtmlComponent> items, ListTypes listType) : base(null, TagTypes.List)
        {
            _items = items;
            Count = _items.Count;
            _listType = listType;
        }
        public override string ToString()
        {
            string output = string.Empty;
            if (_listType == ListTypes.Unordered)
            {
                output = "<ul>\r\n";

                for (var i = 0; i < _items.Count; i++)
                {
                    output += _items[i] + "\r\n";
                }

                output += "</ul>";
            }
            else
            {
                output = "<ol>\r\n";

                for (var i = 0; i < _items.Count; i++)
                {
                    output += _items[i] + "\r\n";
                }

                output += "</ol>";
            }
            

            return output;
        }
    }
}
