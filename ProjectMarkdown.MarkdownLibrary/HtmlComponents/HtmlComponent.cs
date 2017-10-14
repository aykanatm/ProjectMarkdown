using ProjectMarkdown.MarkdownLibrary.ExtensionMethods;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public abstract class HtmlComponent
    {
        protected string Text;

        public enum TagTypes
        {
            Paragraph, Header, Link, Bold, Italic, ListItem, Image, InlineCode, BlockCode, StrikeThrough, HorizontalRule, List, RawHtml, Table
        }

        public TagTypes TagType { get; private set; }

        protected HtmlComponent(string text, TagTypes tagType)
        {
            Text = text;
            TagType = tagType;
        }
    }
}
