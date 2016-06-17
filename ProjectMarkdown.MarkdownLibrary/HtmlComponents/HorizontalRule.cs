namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class HorizontalRule : HtmlComponent
    {
        public HorizontalRule() : base(null, TagTypes.HorizontalRule)
        {
            
        }

        public override string ToString()
        {
            return "<hr>";
        }
    }
}
