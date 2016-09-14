using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfCodeTextbox
{
    public interface IHighlighter
    {
        int Highlight(FormattedText text, int previousBlockCode);
    }
}
