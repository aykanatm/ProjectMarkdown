namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Paragraph : HtmlComponent
    {
        public Paragraph(string text) : base(text, TagTypes.Paragraph)
        {
            
        }

        public override string ToString()
        {
            return "<p>" + Text + "</p>";
        }
    }
}
