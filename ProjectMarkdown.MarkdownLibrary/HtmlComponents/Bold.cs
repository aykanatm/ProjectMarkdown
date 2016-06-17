namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Bold : HtmlComponent
    {
        public Bold(string text) : base(text, TagTypes.Bold)
        {
            
        }

        public override string ToString()
        {
            return "<b>" + Text + "</b>";
        }
    }
}
