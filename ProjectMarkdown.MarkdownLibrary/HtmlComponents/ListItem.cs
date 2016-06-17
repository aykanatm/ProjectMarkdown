namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class ListItem : HtmlComponent
    {
        public ListItem(string text) : base(text, TagTypes.ListItem)
        {
            
        }

        public override string ToString()
        {
            return "<li>" + Text + "</li>";
        }
    }
}
