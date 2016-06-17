using System.Text.RegularExpressions;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class InlineCode : HtmlComponent
    {
        public InlineCode(string text) : base(text, TagTypes.InlineCode)
        {
            
        }

        public override string ToString()
        {
            Text = Regex.Replace(Text, "<.*?>", string.Empty);
            return "<code>" + Text + "</code>";
        }
    }
}
