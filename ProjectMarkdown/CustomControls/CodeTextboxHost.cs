using System;
using System.Windows;
using System.Windows.Forms.Integration;
using FastColoredTextBoxNS;

namespace ProjectMarkdown.CustomControls
{
    public class CodeTextboxHost : WindowsFormsHost
    {
        private readonly FastColoredTextBox _innerTextbox = new FastColoredTextBox();

        public static readonly DependencyProperty WordWrapProperty = DependencyProperty.Register("WordWrap", typeof(bool), typeof(CodeTextboxHost), new PropertyMetadata(false, new PropertyChangedCallback(
            (d, e) =>
            {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null)
                {
                    textBoxHost._innerTextbox.WordWrap = (bool) textBoxHost.GetValue(e.Property);
                }
            }), null));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CodeTextboxHost), new PropertyMetadata("", new PropertyChangedCallback(
            (d, e) =>
            {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null)
                {
                    textBoxHost._innerTextbox.Text = textBoxHost.GetValue(e.Property) as string;
                }
            }), null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool WordWrap
        {
            get { return (bool)GetValue(WordWrapProperty); }
            set { SetValue(WordWrapProperty, value); }
        }

        public CodeTextboxHost()
        {
            Child = _innerTextbox;
            _innerTextbox.Language = FastColoredTextBoxNS.Language.Custom;
            _innerTextbox.DescriptionFile = AppDomain.CurrentDomain.BaseDirectory + "SyntaxConfig\\MarkdownSyntaxHighlighting.xml";
            _innerTextbox.HighlightingRangeType = HighlightingRangeType.AllTextRange;
            _innerTextbox.TextChanged += _innerTextbox_TextChanged;
            _innerTextbox.ClearUndo();
        }

        private void _innerTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValue(TextProperty, _innerTextbox.Text);
        }

        public void Undo()
        {
            if (_innerTextbox.UndoEnabled)
            {
                _innerTextbox.Undo();
            }
        }

        public void Redo()
        {
            if (_innerTextbox.RedoEnabled)
            {
                _innerTextbox.Redo();
            }
        }

        public void ClearUndoRedo()
        {
            _innerTextbox.ClearUndo();
        }
    }
}
