using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class HtmlComponent
    {
        public enum TagTypes
        {
            Paragraph, Header, Link, Bold, Italic, UnorderedList, OrderedList, ListItem, Image, InlineCode, BlockCode
        }

        public TagTypes TagType { get; private set; }

        protected HtmlComponent(TagTypes tagType)
        {
            TagType = tagType;
        }
    }
}
