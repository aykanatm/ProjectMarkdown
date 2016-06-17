namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class StrikeThrough : HtmlComponent
    {
        public StrikeThrough(string text) : base(text, TagTypes.StrikeThrough)
        {
            
        }

        public override string ToString()
        {
            return "<del>" + Text + "</del>";
        }
    }
}
