using System.Windows.Media;

namespace WpfCodeTextbox
{
    public interface IHighlighter
    {
        int Highlight(FormattedText text, int previousBlockCode);
    }
}
