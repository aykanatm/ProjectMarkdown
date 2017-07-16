using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms.Integration;
using FastColoredTextBoxNS;
using ProjectMarkdown.ExtensionMethods;

namespace ProjectMarkdown.CustomControls
{
    public class CodeTextboxHost : WindowsFormsHost
    {
        private readonly FastColoredTextBox _innerTextbox = new FastColoredTextBox();
        private string _oldFilePath;
        private string _newFilePath;

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(CodeTextboxHost), new PropertyMetadata("", new PropertyChangedCallback(
            (d, e) =>
            {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null)
                {
                    textBoxHost._oldFilePath = textBoxHost._newFilePath;
                    textBoxHost._newFilePath = textBoxHost.GetValue(e.Property) as string;
                }
            }), null));

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

        public static readonly DependencyProperty HistoryProperty = DependencyProperty.Register("History", typeof(IEnumerable<UndoableCommand>), typeof(CodeTextboxHost), new FrameworkPropertyMetadata(new ObservableCollection<UndoableCommand>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(
            (d, e) =>
            {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null)
                {
                    var history = textBoxHost.GetValue(e.Property) as ObservableCollection<UndoableCommand>;
                    if (history != null)
                    {
                        textBoxHost._innerTextbox.TextSource.Manager.History = history.ToLimitedStack(200);
                        textBoxHost._innerTextbox.OnUndoRedoStateChanged();
                    }
                    else
                    {
                        textBoxHost._innerTextbox.ClearUndo();
                    }
                }
            }), null));

        public static readonly DependencyProperty RedoStackProperty = DependencyProperty.Register("RedoStack", typeof(IEnumerable<UndoableCommand>), typeof(CodeTextboxHost), new FrameworkPropertyMetadata(new ObservableCollection<UndoableCommand>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(
            (d, e) =>
            {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null)
                {
                    var redoStack = textBoxHost.GetValue(e.Property) as ObservableCollection<UndoableCommand>;
                    if (redoStack != null)
                    {
                        textBoxHost._innerTextbox.TextSource.Manager.RedoStack = redoStack.ToStack();
                        textBoxHost._innerTextbox.OnUndoRedoStateChanged();
                    }
                    else
                    {
                        textBoxHost._innerTextbox.ClearUndo();
                    }
                }
            }), null));

        public string FilePath
        {
            get { return (string) GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value);}
        }

        public ObservableCollection<UndoableCommand> History
        {
            get { return (ObservableCollection<UndoableCommand>) GetValue(HistoryProperty);}
            set { SetCurrentValue(HistoryProperty, new ObservableCollection<UndoableCommand>(value));}
        }

        public ObservableCollection<UndoableCommand> RedoStack
        {
            get { return (ObservableCollection<UndoableCommand>) GetValue(RedoStackProperty); }
            set { SetCurrentValue(RedoStackProperty, new ObservableCollection<UndoableCommand>(value));}
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
            Text = _innerTextbox.Text;

            if (_oldFilePath == null)
            {
                _innerTextbox.ClearUndo();
                _oldFilePath = _newFilePath;
            }

            History = _innerTextbox.TextSource.Manager.History.ToOveObservableCollection();
            RedoStack = _innerTextbox.TextSource.Manager.RedoStack.ToObservableCollection();

            if (_oldFilePath != _newFilePath)
            {
                _innerTextbox.ClearUndo();
            }
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
