using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public abstract class HtmlComponent
    {
        protected string Text;

        public enum TagTypes
        {
            Paragraph, Header, Link, Bold, Italic, ListItem, Image, InlineCode, BlockCode, StrikeThrough, HorizontalRule, List, RawHtml
        }

        public TagTypes TagType { get; private set; }

        protected HtmlComponent(string text, TagTypes tagType)
        {
            Text = text;
            if (Text != null)
            {
                if (this is Header)
                {
                    Text = text.Replace("&", "&amp").Replace("<", "&lt").Replace(">", "&gt");
                }
            }
            TagType = tagType;
        }
    }
}
