using System;
using System.Collections.Generic;
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

        public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register("History", typeof(LimitedStack<UndoableCommand>), typeof(CodeTextboxHost), new PropertyMetadata(new LimitedStack<UndoableCommand>(200), new PropertyChangedCallback(
            (d, e) =>
            {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null)
                {
                    var history = textBoxHost.GetValue(e.Property) as LimitedStack<UndoableCommand>;
                    if (history != null)
                    {
                        textBoxHost._innerTextbox.TextSource.Manager.History.Clear();
                        textBoxHost._innerTextbox.TextSource.Manager.History = history;
                        textBoxHost._innerTextbox.OnUndoRedoStateChanged();
                    }
                    else
                    {
                        textBoxHost._innerTextbox.ClearUndo();
                    }
                }
            }), null));

        public static readonly DependencyProperty RedoStackProperty = DependencyProperty.Register("RedoStack", typeof(Stack<UndoableCommand>), typeof(CodeTextboxHost), new PropertyMetadata(new Stack<UndoableCommand>(), new PropertyChangedCallback(
            (d, e) =>
            {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null)
                {
                    var redoStack = textBoxHost.GetValue(e.Property) as Stack<UndoableCommand>;
                    if (redoStack != null)
                    {
                        textBoxHost._innerTextbox.TextSource.Manager.RedoStack.Clear();
                        textBoxHost._innerTextbox.TextSource.Manager.RedoStack = redoStack;
                        textBoxHost._innerTextbox.OnUndoRedoStateChanged();
                    }
                    else
                    {
                        textBoxHost._innerTextbox.ClearUndo();
                    }
                }
            }), null));

        public LimitedStack<UndoableCommand> History
        {
            get { return (LimitedStack<UndoableCommand>) GetValue(HistoryProperty);}
            set { SetValue(HistoryProperty, value);}
        }

        public Stack<UndoableCommand> RedoStack
        {
            get { return (Stack<UndoableCommand>) GetValue(RedoStackProperty); }
            set { SetValue(RedoStackProperty, value);}
        }

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
        }

        private void _innerTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValue(TextProperty, _innerTextbox.Text);
        }

        public LimitedStack<UndoableCommand> GetHistory()
        {
            return _innerTextbox.TextSource.Manager.History;
        }

        public Stack<UndoableCommand> GetRedoStack()
        {
            return _innerTextbox.TextSource.Manager.RedoStack;
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
