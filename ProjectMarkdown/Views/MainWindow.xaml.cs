using System.Windows;
using AurelienRibon.Ui.SyntaxHighlightBox;

namespace ProjectMarkdown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            shbox.CurrentHighlighter = HighlighterManager.Instance.Highlighters["MarkdownSyntax"];
        }
    }
}
