using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using FastColoredTextBoxNS;
using LogUtils;
using ProjectMarkdown.ExtensionMethods;
using ProjectMarkdown.Services;
using ProjectMarkdown.Statics;

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

        public static readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(Font), typeof(CodeTextboxHost), new PropertyMetadata(new Font(new System.Drawing.FontFamily("Consolas"), 11), new PropertyChangedCallback(
            (d, e) =>
            {
                var textBoxHost = d as CodeTextboxHost;
                if (textBoxHost != null && textBoxHost._innerTextbox != null)
                {
                    var font = (Font) textBoxHost.GetValue(e.Property);
                    textBoxHost._innerTextbox.Font = font;
                }
            }), null));

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

        public Font Font
        {
            get { return (Font) GetValue(FontProperty); }
            set { SetValue(FontProperty, value);}
        }

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
            Logger.GetInstance().Debug("CodeTextboxHost() >>");

            try
            {
                Child = _innerTextbox;
                _innerTextbox.Language = FastColoredTextBoxNS.Language.Custom;
                _innerTextbox.DescriptionFile = FilePaths.MarkdownSyntaxDescriptionFilePath;
                _innerTextbox.HighlightingRangeType = HighlightingRangeType.AllTextRange;
                _innerTextbox.TextChanged += _innerTextbox_TextChanged;
                _innerTextbox.Scroll += _innerTextbox_Scroll;
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< CodeTextboxHost()");
        }

        private void _innerTextbox_Scroll(object sender, ScrollEventArgs e)
        {
            var maximum = _innerTextbox.VerticalScroll.Maximum;

            var viewableRatio = (double)_innerTextbox.Height / (double)maximum;

            var scrollBarArea = (double) maximum;

            var thumbHeight = scrollBarArea * viewableRatio;

            SharedEventHandler.GetInstance().RaiseOnCodeTextboxScrollChanged(new ScrollResult
                {
                    MaxValue = maximum - Convert.ToInt32(thumbHeight),
                    MinValue = _innerTextbox.VerticalScroll.Minimum,
                    Value = _innerTextbox.VerticalScroll.Value
                });
        }

        public void RefreshScrollPosition()
        {
            var maximum = _innerTextbox.VerticalScroll.Maximum;

            var viewableRatio = (double)_innerTextbox.Height / (double)maximum;

            var scrollBarArea = (double)maximum;

            var thumbHeight = scrollBarArea * viewableRatio;

            SharedEventHandler.GetInstance().RaiseOnCodeTextboxScrollChanged(new ScrollResult
            {
                MaxValue = maximum - Convert.ToInt32(thumbHeight),
                MinValue = _innerTextbox.VerticalScroll.Minimum,
                Value = _innerTextbox.VerticalScroll.Value
            });
        }

        private void _innerTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
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
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void Undo()
        {
            Logger.GetInstance().Debug("Undo() >>");

            try
            {
                if (_innerTextbox.UndoEnabled)
                {
                    _innerTextbox.Undo();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< Undo()");
        }

        public void Redo()
        {
            Logger.GetInstance().Debug("Redo() >>");

            try
            {
                if (_innerTextbox.RedoEnabled)
                {
                    _innerTextbox.Redo();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< Redo()");
        }

        public void Cut()
        {
            Logger.GetInstance().Debug("Cut() >>");

            try
            {
                _innerTextbox.Cut();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< Cut()");
        }

        public void Copy()
        {
            Logger.GetInstance().Debug("Copy() >>");

            try
            {
                _innerTextbox.Copy();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< Copy()");
        }

        public void Paste()
        {
            Logger.GetInstance().Debug("Paste() >>");

            try
            {
                _innerTextbox.Paste();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< Paste()");
        }

        public void Delete()
        {
            Logger.GetInstance().Debug("Delete() >>");

            try
            {
                _innerTextbox.ClearSelected();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< Delete()");
        }

        public void SelectAll()
        {
            Logger.GetInstance().Debug("SelectAll() >>");

            try
            {
                _innerTextbox.SelectAll();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< SelectAll()");
        }

        public void ShowFindDialog()
        {
            Logger.GetInstance().Debug("ShowFindDialog() >>");

            try
            {
                _innerTextbox.ShowFindDialog();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< ShowFindDialog()");
        }

        public void ShowReplaceDialog()
        {
            Logger.GetInstance().Debug("ShowReplaceDialog() >>");

            try
            {
                _innerTextbox.ShowReplaceDialog();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< ShowReplaceDialog()");
        }

        public void ClearUndoRedo()
        {
            Logger.GetInstance().Debug("ClearUndoRedo() >>");

            try
            {
                _innerTextbox.ClearUndo();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< ClearUndoRedo()");
        }

        public bool HasSelectedText()
        {
            try
            {
                if (!string.IsNullOrEmpty(_innerTextbox.SelectedText))
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
