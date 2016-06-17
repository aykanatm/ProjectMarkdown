namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Italic : HtmlComponent
    {
        public Italic(string text) : base(text, TagTypes.Italic)
        {
            
        }

        public override string ToString()
        {
            return "<i>" + Text + "</i>";
        }
    }
}
