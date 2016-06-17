namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Link : HtmlComponent
    {
        private readonly string _url;

        public Link(string text, string url) : base(text, TagTypes.Link)
        {
            _url = url;
        }

        public override string ToString()
        {
            return "<a href=\"" + _url + "\">" + Text + "</a>";
        }
    }
}
