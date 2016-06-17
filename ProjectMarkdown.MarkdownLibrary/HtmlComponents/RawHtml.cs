namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class RawHtml : HtmlComponent
    {
        private readonly string _rawHtml;

        public RawHtml(string rawHtml) : base(rawHtml, TagTypes.RawHtml)
        {
            _rawHtml = rawHtml;
        }

        public override string ToString()
        {
            return _rawHtml;
        }
    }
}
