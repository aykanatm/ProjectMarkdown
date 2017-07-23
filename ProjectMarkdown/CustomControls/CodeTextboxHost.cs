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

        // This is true when user switches from one tab to the other and the TextBox is cleared inserting an empty string ("")
        // If this is true then the textbox will clear its undo and redo stacks to prevent user to undo to an empty textbox
        private bool _textBoxLoadedWithEmptyString;
        // This is true when user switches from one tab to the other and the textbox is loaded with new content that is fetched from the document
        // If this is true then the textbox will clear its undo and redo stacks to prevent previous document's undo and redo stacks to load into the new document
        private bool _textBoxLoadedWithNewDocumentText;

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
            
            if (_oldFilePath != _newFilePath)
            {
                // If _oldFilePath is null, it means we opened a new document for the first time
                // When a new document is open there is no need for us to look for the empty string and the document loaded cases
                if (_oldFilePath != null)
                {
                    _textBoxLoadedWithEmptyString = true;
                    _textBoxLoadedWithNewDocumentText = true;
                }
                else
                {
                    _textBoxLoadedWithEmptyString = false;
                    _textBoxLoadedWithNewDocumentText = false;
                }
                
                _innerTextbox.ClearUndo();
                _oldFilePath = _newFilePath;
            }
            else
            {
                if (!_textBoxLoadedWithEmptyString && !_textBoxLoadedWithNewDocumentText)
                {
                    History = _innerTextbox.TextSource.Manager.History.ToOveObservableCollection();
                    RedoStack = _innerTextbox.TextSource.Manager.RedoStack.ToObservableCollection();
                }
                else
                {
                    if (!_textBoxLoadedWithEmptyString)
                    {
                        _textBoxLoadedWithNewDocumentText = false;
                    }
                    else
                    {
                        _textBoxLoadedWithEmptyString = false;
                    }
                    _innerTextbox.ClearUndo();
                }
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

        public void Cut()
        {
            _innerTextbox.Cut();
        }

        public void Copy()
        {
            _innerTextbox.Copy();
        }

        public void ClearUndoRedo()
        {
            _innerTextbox.ClearUndo();
        }
    }
}
