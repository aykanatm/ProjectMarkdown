﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using IOUtils;
using LogUtils;
using ProjectMarkdown.Annotations;
using ProjectMarkdown.CustomControls;
using ProjectMarkdown.ExtensionMethods;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;
using ProjectMarkdown.Services;
using ProjectMarkdown.Statics;
using ProjectMarkdown.Windows;
using System.Drawing;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using FontFamily = System.Drawing.FontFamily;
using MessageBox = System.Windows.MessageBox;

namespace ProjectMarkdown.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DocumentModel> _documents;
        private DocumentModel _currentDocument;
        private string _title;
        private PreferencesModel _currentPreferences;
        private string _selectedHeadingFormatting;
        private ObservableCollection<string> _headingFormats;

        public ObservableCollection<string> HeadingFormats
        {
            get { return _headingFormats; }
            set
            {
                _headingFormats = value;
                OnPropertyChanged();
            }
        }

        public string SelectedHeadingFormatting
        {
            get { return _selectedHeadingFormatting; }
            set
            {
                _selectedHeadingFormatting = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DocumentModel> Documents
        {
            get { return _documents; }
            set
            {
                _documents = value;
                OnPropertyChanged(nameof(Documents));
            }
        }

        public DocumentModel CurrentDocument
        {
            get { return _currentDocument; }
            set
            {
                _currentDocument = value;
                // if value is null, it means the last document is being closed
                if (value != null)
                {
                    ResetDocumentsOpenState();
                    _currentDocument.IsOpen = true;
                    RefreshCurrentHtmlView();

                    var filePath = _currentDocument.Metadata.FilePath;
                    var author = _currentDocument.Metadata.Author;
                    if (string.IsNullOrEmpty(author))
                    {
                        author = "Unknown";
                    }

                    Title = filePath + " by " + author;
                }
                else
                {
                    Title = "";
                }

                OnPropertyChanged(nameof(CurrentDocument));
            }
        }

        public PreferencesModel CurrentPreferences
        {
            get { return _currentPreferences; }
            set
            {
                _currentPreferences = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // File
        public ICommand CreateNewDocumentCommand { get; set; }
        public ICommand SaveDocumentCommand { get; set; }
        public ICommand SaveAllDocumentsCommand { get; set; }
        public ICommand SaveAsDocumentCommand { get; set; }
        public ICommand OpenDocumentCommand { get; set; }
        public ICommand OpenContainingFolderCommand { get; set; }
        public ICommand ExportMarkdownCommand { get; set; }
        public ICommand ExportHtmlCommand { get; set; }
        public ICommand ExportPdfCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand CloseActiveDocumentCommand { get; set; }
        public ICommand CloseAllDocumentsCommand { get; set; }
        public ICommand CloseAllButActiveDocumentCommand { get; set; }

        // Edit
        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }
        public ICommand CutCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand PasteCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }

        // Search
        public ICommand FindCommmand { get; set; }
        public ICommand ReplaceCommand { get; set; }

        // Settings
        public ICommand OpenPreferencesWindowCommand { get; set; }

        // Help
        public ICommand OpenUserGuideCommand { get; set; }
        public ICommand OpenAboutWindowCommand { get; set; }

        // Formatting
        public ICommand FormatBlockCodeCommand { get; set; }
        public ICommand FormatBlockQuoteCommand { get; set; }
        public ICommand FormatBoldCommand { get; set; }
        public ICommand InsertHorizontalRuleCommand { get; set; }
        public ICommand InsertImageCommand { get; set; }
        public ICommand FormatInlineCodeCommand { get; set; }
        public ICommand FormatItalicCommand { get; set; }
        public ICommand ApplyLinkCommand { get; set; }
        public ICommand FormatOrderedListCommand { get; set; }
        public ICommand FormatUnorderedListCommand { get; set; }
        public ICommand FormatStrikeThroughCommand { get; set; }
        public ICommand InsertTableCommand { get; set; }

        // Events
        public ICommand MainWindowResizedCommand { get; set; }
        public ICommand MainWindowClosingEventCommand { get; set; }
        public ICommand HeaderChangedEventCommand { get; set; }

        public MainWindowViewModel()
        {
            Logger.GetInstance().Debug("MainWindowViewModel() >>");

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            
            LoadCommands();
            LoadPreferences();

            SharedEventHandler.GetInstance().OnPreferecesSaved += OnPreferecesSaved;
            SharedEventHandler.GetInstance().OnApplyLinkUrlSelected += OnApplyLinkUrlSelected;
            SharedEventHandler.GetInstance().OnInsertImageUrlSelected += OnInsertImageUrlSelected;
            SharedEventHandler.GetInstance().OnInsertTableDimensionsSelected += OnInsertTableDimensionsSelected;
            SharedEventHandler.GetInstance().OnCodeTextboxScrollChanged += OnCodeTextboxScrollChanged;
            SharedEventHandler.GetInstance().OnTextboxTextChanged += OnTextboxTextChanged;

            Documents = new ObservableCollection<DocumentModel>();
            HeadingFormats = new ObservableCollection<string>{"Heading 1", "Heading 2", "Heading 3", "Heading 4", "Heading 5", "Heading 6"};
            SelectedHeadingFormatting = "Heading 1";

            Logger.GetInstance().Debug("<< MainWindowViewModel()");
        }

        private void OnInsertTableDimensionsSelected(int rows, int columns)
        {
            Logger.GetInstance().Debug("OnInsertTableDimensionsSelected() >>");

            try
            {
                var table = "";
                for (var i = 0; i < rows; i++)
                {
                    for (var j = 0; j < columns; j++)
                    {
                        // Write header
                        if (i == 0)
                        {
                            table += "| Header " + (j + 1) + " |";
                        }
                        else
                        {
                            table += "| Element (" + (i+1) + "/" + (j+1) + ") |";
                        }
                    }
                    if (i != rows - 1)
                    {
                        table += "\r\n";
                    }
                }

                CodeTextboxManager.GetInstance().InsertText(CurrentDocument, table);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< OnInsertTableDimensionsSelected()");
        }

        private void OnInsertImageUrlSelected(string url, string alt)
        {
            Logger.GetInstance().Debug("OnInsertImageUrlSelected() >>");

            try
            {
                var image = "![" + (string.IsNullOrEmpty(alt) ? "Image Alternate Text" : alt) + "](" + url + ")";
                CodeTextboxManager.GetInstance().InsertText(CurrentDocument, image);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< OnInsertImageUrlSelected()");
        }

        private void OnApplyLinkUrlSelected(string url)
        {
            Logger.GetInstance().Debug("OnApplyLinkUrlSelected() >>");

            try
            {
                var selectedText = CodeTextboxManager.GetInstance().GetSelectedText(CurrentDocument);
                var formattedText = "[" + selectedText + "](" + url + ")";
                CodeTextboxManager.GetInstance().ReplaceText(CurrentDocument, formattedText);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< OnApplyLinkUrlSelected()");
        }

        private void OnTextboxTextChanged()
        {
            if (CurrentPreferences.IsSyncTextAndHtml)
            {
                var style = GetCss();
                var htmlFilePath = DocumentSynchronizer.Sync(CurrentDocument, style);
                CurrentDocument.Html = new Uri(htmlFilePath);
            }
        }

        private void OnCodeTextboxScrollChanged(ScrollResult scrollResult)
        {
            if (CurrentPreferences.IsScrollBarsSynced)
            {
                CefChromiumBrowserManager.GetInstance().Scroll(CurrentDocument, scrollResult);
            }
        }

        private void OnPreferecesSaved(PreferencesModel preferences)
        {
            Logger.GetInstance().Debug("OnPreferencesSaved() >>");
            LoadPreferences(preferences);
            Logger.GetInstance().Debug("<< OnPreferencesSaved()");
        }

        private void LoadCommands()
        {
            Logger.GetInstance().Debug("LoadCommands() >>");

            // File
            CreateNewDocumentCommand = new RelayCommand(CreateNewDocument, CanCreateNewDocument);
            SaveDocumentCommand = new RelayCommand(SaveDocument, CanSaveDocument);
            SaveAllDocumentsCommand = new RelayCommand(SaveAllDocuments, CanSaveAllDocuments);
            SaveAsDocumentCommand = new RelayCommand(SaveAsDocument, CanSaveAsDocument);
            OpenDocumentCommand = new RelayCommand(OpenDocument, CanOpenDocument);
            OpenContainingFolderCommand = new RelayCommand(OpenContainingFolder, CanOpenContainingFolder);
            ExportHtmlCommand = new RelayCommand(ExportHtml, CanExportHtml);
            ExportMarkdownCommand = new RelayCommand(ExportMarkdown, CanExportMarkdown);
            ExportPdfCommand = new RelayCommand(ExportPdf, CanExportPdf);
            PrintCommand = new RelayCommand(Print, CanPrint);
            ExitCommand = new RelayCommand(Exit, CanExit);
            CloseActiveDocumentCommand = new RelayCommand(CloseActiveDocument, CanCloseActiveDocument);
            CloseAllDocumentsCommand = new RelayCommand(CloseAllDocuments, CanCloseAllDocuments);
            CloseAllButActiveDocumentCommand = new RelayCommand(CloseAllButActiveDocument, CanCloseAllButActiveDocument);

            // Edit
            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);
            CutCommand = new RelayCommand(Cut, CanCutCopyDelete);
            CopyCommand = new RelayCommand(Copy, CanCutCopyDelete);
            PasteCommand = new RelayCommand(Paste, CanPaste);
            DeleteCommand = new RelayCommand(Delete, CanCutCopyDelete);
            SelectAllCommand = new RelayCommand(SelectAll, CanSelectAll);

            // Search
            FindCommmand = new RelayCommand(Find, CanFind);
            ReplaceCommand = new RelayCommand(Replace, CanReplace);

            // Settings
            OpenPreferencesWindowCommand = new RelayCommand(OpenPreferencesWindow, CanOpenPreferencesWindow);

            // Help
            OpenUserGuideCommand = new RelayCommand(OpenUserGuide, CanOpenUserGuide);
            OpenAboutWindowCommand = new RelayCommand(OpenAboutWindow, CanOpenAboutWindow);

            // Formatting
            FormatBlockCodeCommand = new RelayCommand(FormatText, CanFormatText);
            FormatBlockQuoteCommand = new RelayCommand(FormatText, CanFormatText);
            FormatBoldCommand = new RelayCommand(FormatText, CanFormatText);
            InsertHorizontalRuleCommand = new RelayCommand(InsertHorizontalRule, CanInsertHorizontalRule);
            InsertImageCommand = new RelayCommand(InsertImage, CanInsertImage);
            FormatInlineCodeCommand = new RelayCommand(FormatText, CanFormatText);
            FormatItalicCommand = new RelayCommand(FormatText, CanFormatText);
            ApplyLinkCommand = new RelayCommand(ApplyLink, CanFormatText);
            FormatOrderedListCommand = new RelayCommand(FormatText, CanFormatText);
            FormatUnorderedListCommand = new RelayCommand(FormatText, CanFormatText);
            FormatStrikeThroughCommand = new RelayCommand(FormatText, CanFormatText);
            InsertTableCommand = new RelayCommand(InsertTable, CanInsertTable);

            // Events
            MainWindowClosingEventCommand = new RelayCommand(MainWindowClosingEvent, CanMainWindowClosingEvent);
            MainWindowResizedCommand = new RelayCommand(MainWindowResizedEvent, CanMainWindowResizedEvent);
            HeaderChangedEventCommand = new RelayCommand(HeaderFormattingChangedEvent, CanFormatText);

            Logger.GetInstance().Debug("<< LoadCommands()");
        }

        private void LoadPreferences()
        {
            Logger.GetInstance().Debug("LoadPreferences() >>");
            try
            {
                var gxs = new GenericXmlSerializer<PreferencesModel>();

                if (!File.Exists(FilePaths.PreferencesFilePath))
                {
                    CurrentPreferences = new PreferencesModel
                    {
                        Author = Environment.UserName,
                        CurrentLogLevel = "DEBUG",
                        LogFilePath = FilePaths.DefaultLogFilePath,
                        CurrentFont = "Consolas",
                        CurrentFontSize = "11",
                        IsDoubleClickToCloseDocument = true,
                        IsExitOnCloseTheLastTab = false,
                        IsSyncTextAndHtml = false,
                        IsScrollBarsSynced = true,
                        IsTabBarLocked = false,
                        IsToolbarHidden = false,
                        IsWordWrap = true,
                        CurrentLanguage = "English",
                        CurrentTheme = "Default",
                        IsLoggingEnabled = true
                    };

                    gxs.Serialize(CurrentPreferences, FilePaths.PreferencesFilePath);

                    if (Documents != null)
                    {
                        foreach (var document in Documents)
                        {
                            document.IsWordWrap = CurrentPreferences.IsWordWrap;
                            document.CurrentFont =  new Font(new FontFamily(CurrentPreferences.CurrentFont), float.Parse(CurrentPreferences.CurrentFontSize));
                        }
                    }
                }
                else
                {
                    CurrentPreferences = gxs.DeSerialize(FilePaths.PreferencesFilePath);

                    if (Documents != null)
                    {
                        foreach (var document in Documents)
                        {
                            document.IsWordWrap = CurrentPreferences.IsWordWrap;
                            document.CurrentFont = new Font(new FontFamily(CurrentPreferences.CurrentFont), float.Parse(CurrentPreferences.CurrentFontSize));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while retrieving preferences", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< LoadPreferences()");
        }

        private void LoadPreferences(PreferencesModel preferences)
        {
            Logger.GetInstance().Debug("LoadPreferences() >>");
            CurrentPreferences = preferences;

            if (Documents != null)
            {
                foreach (var document in Documents)
                {
                    document.IsWordWrap = CurrentPreferences.IsWordWrap;
                    document.CurrentFont = new Font(new FontFamily(CurrentPreferences.CurrentFont), float.Parse(CurrentPreferences.CurrentFontSize));
                }
            }

            Logger.GetInstance().Debug("<< LoadPreferences()");
        }
        
        // FILE
        public void CreateNewDocument(object obj)
        {
            Logger.GetInstance().Debug("CreateNewDocument() >>");
            try
            {
                int documentCount = 1;
                string documentFileName = "untitled" + documentCount + ".md";
                bool fileNameFound = false;
                while (!fileNameFound)
                {
                    var checkForDuplicateFileName = Documents.FirstOrDefault(d => d.Metadata.FileName == documentFileName);
                    if (checkForDuplicateFileName == null)
                    {
                        fileNameFound = true;
                    }
                    else
                    {
                        documentCount += 1;
                        documentFileName = "untitled" + documentCount + ".md";
                    }
                }
                var document = new DocumentModel(this, documentFileName);
                Documents.Add(document);
                CurrentDocument = document;

                // Set current document as "open"
                ResetDocumentsOpenState();
                CurrentDocument.IsOpen = true;
                CurrentDocument.IsSaved = false;
                CurrentDocument.IsWordWrap = CurrentPreferences.IsWordWrap;
                CurrentDocument.Metadata.Author = CurrentPreferences.Author;
                CurrentDocument.CurrentFont = new Font(new FontFamily(CurrentPreferences.CurrentFont), float.Parse(CurrentPreferences.CurrentFontSize));
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while creating document", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< CreateNewDocument()");
        }
        public bool CanCreateNewDocument(object obj)
        {
            return true;
        }

        public void OpenContainingFolder(object obj)
        {
            Logger.GetInstance().Debug("OpenContainingFolder() >>");

            try
            {
                var parent = Directory.GetParent(CurrentDocument.Metadata.FilePath);
                Process.Start("explorer.exe", parent.FullName);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while opening the containing folder",MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< OpenContainingFolder()");
        }

        public bool CanOpenContainingFolder(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void SaveDocument(object obj)
        {
            Logger.GetInstance().Debug("SaveDocument() >>");

            try
            {
                CurrentDocument = SaveDocument(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while saving the document", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< SaveDocument()");
        }

        public bool CanSaveDocument(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void SaveAllDocuments(object obj)
        {
            Logger.GetInstance().Debug("SaveAllDocuments() >>");

            try
            {
                foreach (var document in Documents)
                {
                    SaveDocument(document);
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during saving all documents", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< SaveAllDocuments()");
        }

        public bool CanSaveAllDocuments(object obj)
        {
            if (Documents.Any())
            {
                return true;
            }
            return false;
        }

        public void SaveAsDocument(object obj)
        {
            Logger.GetInstance().Debug("SaveAsDocument() >>");

            try
            {
                CurrentDocument = SaveAsDocument(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while saving the document", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< SaveAsDocument()");
        }

        public bool CanSaveAsDocument(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void OpenDocument(object obj)
        {
            Logger.GetInstance().Debug("OpenDocument() >>");

            try
            {
                var currentDocument = DocumentLoader.Load(this);
                if (currentDocument != null)
                {
                    var documentsWithCurrentFilePath = (from d in Documents
                                                        where d.Metadata.FilePath == currentDocument.Metadata.FilePath
                                                        select d);
                    if (documentsWithCurrentFilePath.Any())
                    {
                        MessageBox.Show("The document '" + currentDocument.Metadata.FilePath + "' is already open.",
                            "Duplicate File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Documents.Add(currentDocument);
                        CurrentDocument = currentDocument;

                        // Set current document as "open"
                        ResetDocumentsOpenState();
                        CurrentDocument.IsOpen = true;
                        CurrentDocument.IsOpenedFromMenu = true;
                        CurrentDocument.IsSaved = true;
                        CurrentDocument.IsWordWrap = CurrentPreferences.IsWordWrap;
                        CurrentDocument.CurrentFont = new Font(new FontFamily(CurrentPreferences.CurrentFont), float.Parse(CurrentPreferences.CurrentFontSize));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while opening document", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< OpenDocument()");
        }

        public bool CanOpenDocument(object obj)
        {
            return true;
        }

        public void ExportMarkdown(object obj)
        {
            Logger.GetInstance().Debug("ExportMarkdown() >>");

            try
            {
                DocumentExporter.ExportMarkdown(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while exporting markdown", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< ExportMarkdown()");
        }

        public bool CanExportMarkdown(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void ExportHtml(object obj)
        {
            Logger.GetInstance().Debug("ExportHtml() >>");

            try
            {
                var css = GetCss();
                DocumentExporter.ExportHtml(CurrentDocument, css);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while exporting HTML", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< ExportHtml()");
        }

        public bool CanExportHtml(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void ExportPdf(object obj)
        {
            Logger.GetInstance().Debug("ExportPdf() >>");

            try
            {
                var css = GetCss();
                DocumentExporter.ExportPdf(CurrentDocument, css);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while exporting PDF", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< ExportPdf()");
        }

        public bool CanExportPdf(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void Print(object obj)
        {
            Logger.GetInstance().Debug("Print() >>");

            try
            {
                var css = GetCss();
                var tempFile = DocumentExporter.ExportPdfTemp(CurrentDocument, css);
                DocumentPrinter.Print(tempFile);
                // Delete temp file
                File.Delete(tempFile);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while printing", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< Print()");
        }

        public bool CanPrint(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void Exit(object obj)
        {
            Logger.GetInstance().Debug("Exit() >>");

            try
            {
                var notSavedDocuments = (from d in Documents
                                         where d.IsSaved == false
                                         select d);

                if (notSavedDocuments.Any())
                {
                    var result = MessageBox.Show("Do you want to save your documents before exiting the application?", "Document not saved warning",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        SaveAllDocuments(obj);
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during application shutdown", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< Exit()");
        }

        public bool CanExit(object obj)
        {
            return true;
        }

        public void CloseActiveDocument(object obj)
        {
            Logger.GetInstance().Debug("CloseActiveDocument() >>");

            try
            {
                RemoveDocumentFromDocuments(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during closing of the document", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< CloseActiveDocument()");
        }

        public bool CanCloseActiveDocument(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }

            return false;
        }

        public void CloseAllDocuments(object obj)
        {
            Logger.GetInstance().Debug("CloseAllDocuments() >>");

            try
            {
                var notSavedDocuments = (from d in Documents
                                         where d.IsSaved == false
                                         select d);

                if (notSavedDocuments.Any())
                {
                    var result = MessageBox.Show("Do you want to save your documents before closing all documents?", "Document not saved warning",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        SaveAllDocuments(obj);
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        Documents.Clear();
                    }
                }
                else
                {
                    Documents.Clear();
                }

                if (CurrentPreferences.IsExitOnCloseTheLastTab)
                {
                    Application.Current.Shutdown();
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show("An error occured during closing of all documents." + e.Message);
            }

            Logger.GetInstance().Debug("<< CloseAllDocuments()");
        }

        public bool CanCloseAllDocuments(object obj)
        {
            if (Documents.Any())
            {
                return true;
            }
            return false;
        }

        public void CloseAllButActiveDocument(object obj)
        {
            Logger.GetInstance().Debug("CloseAllButActiveDocument() >>");

            try
            {
                var allExceptActiveDocument = (from d in Documents
                                               where d != CurrentDocument
                                               select d);

                var exceptActiveDocument = allExceptActiveDocument as DocumentModel[] ?? allExceptActiveDocument.ToArray();

                var notSavedDocuments = (from d in exceptActiveDocument
                                         where d.IsSaved == false
                                         select d);

                var savedDocuments = notSavedDocuments as DocumentModel[] ?? notSavedDocuments.ToArray();

                if (savedDocuments.Any())
                {
                    var result = MessageBox.Show("Do you want to save your documents before closimg documents?", "Document not saved warning",
                            MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (var document in savedDocuments)
                        {
                            SaveDocument(document);
                        }
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        foreach (var document in exceptActiveDocument)
                        {
                            Documents.Remove(document);
                        }
                    }
                }
                else
                {
                    foreach (var document in exceptActiveDocument)
                    {
                        Documents.Remove(document);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during closing all but the current document",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< CloseAllButActiveDocument()");
        }

        public bool CanCloseAllButActiveDocument(object obj)
        {
            if (Documents.Any() && CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        // EDIT

        public bool CanUndo(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }

            return false;
        }

        public void Undo(object obj)
        {
            Logger.GetInstance().Debug("Undo() >>");

            try
            {
                CodeTextboxManager.GetInstance().Undo(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during undo operation", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< Undo()");
        }

        public bool CanRedo(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }

            return false;
        }

        public void Redo(object obj)
        {
            Logger.GetInstance().Debug("Redo() >>");

            try
            {
                CodeTextboxManager.GetInstance().Redo(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during redo operation", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< Redo()");
        }

        public bool CanCutCopyDelete(object obj)
        {
            if (CurrentDocument != null)
            {
                return CodeTextboxManager.GetInstance().HasSelectedText(CurrentDocument);
            }

            return false;
        }

        public void Cut(object obj)
        {
            Logger.GetInstance().Debug("Cut() >>");

            try
            {
                CodeTextboxManager.GetInstance().Cut(CurrentDocument);
            }
            catch (Exception e)
            {

                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during cut operation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< Cut()");
        }

        public void Copy(object obj)
        {
            Logger.GetInstance().Debug("Copy() >>");

            try
            {
                CodeTextboxManager.GetInstance().Copy(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during copy operation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< Copy()");
        }

        public void Delete(object obj)
        {
            Logger.GetInstance().Debug("Delete() >>");

            try
            {
                CodeTextboxManager.GetInstance().Delete(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during delete operation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< Delete()");
        }

        public bool CanPaste(object obj)
        {
            if (CurrentDocument != null)
            {
                if(!string.IsNullOrEmpty(Clipboard.GetText()))
                {
                    return true;
                }
            }

            return false;
        }

        public void Paste(object obj)
        {
            Logger.GetInstance().Debug("Paste() >>");

            try
            {
                CodeTextboxManager.GetInstance().Paste(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during paste operation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< Paste()");
        }

        public bool CanSelectAll(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }

            return false;
        }

        public void SelectAll(object obj)
        {
            Logger.GetInstance().Debug("SelectAll() >>");

            try
            {
                CodeTextboxManager.GetInstance().SelectAll(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured during select all operation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< SelectAll()");
        }

        // SEARCH
        public bool CanFind(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }

            return false;
        }

        public void Find(object obj)
        {
            Logger.GetInstance().Debug("Find() >>");

            try
            {
                CodeTextboxManager.GetInstance().ShowFindDialog(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while opening find window", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< Find()");
        }

        public bool CanReplace(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }

            return false;
        }

        public void Replace(object obj)
        {
            Logger.GetInstance().Debug("Replace() >>");

            try
            {
                CodeTextboxManager.GetInstance().ShowReplaceDialog(CurrentDocument);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while opening replace window", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< Replace()");
        }

        // SETTINGS
        public bool CanOpenPreferencesWindow(object obj)
        {
            return true;
        }

        public void OpenPreferencesWindow(object obj)
        {
            Logger.GetInstance().Debug("OpenPreferencesWindow() >>");
            try
            {
                WindowManager.GetInstance().OpenWindow(WindowManager.WindowTypes.Preferences);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while opening the preferences window", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< OpenPreferencesWindow()");
        }

        // HELP
        public bool CanOpenUserGuide(object obj)
        {
            return true;
        }

        public void OpenUserGuide(object obj)
        {
            Logger.GetInstance().Debug("OpenUserGuide() >>");

            try
            {
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "Documentation\\Project Markdown User Guide.pdf");
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while opening the documentation", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< OpenUserGuide()");
        }

        public bool CanOpenAboutWindow(object obj)
        {
            return true;
        }

        public void OpenAboutWindow(object obj)
        {
            Logger.GetInstance().Debug("OpenAboutWindow() >>");
            try
            {
                WindowManager.GetInstance().OpenWindow(WindowManager.WindowTypes.About);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while opening the about window", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< OpenAboutWindow()");
        }

        // FORMATTING

        public void FormatText(object obj)
        {
            Logger.GetInstance().Debug("FormatText() >>");

            try
            {
                var buttonName = (string)obj;
                var selectedText = CodeTextboxManager.GetInstance().GetSelectedText(CurrentDocument);
                if (!string.IsNullOrEmpty(selectedText))
                {
                    var formattedText = "";
                    if (buttonName == FormatTextButtons.BlockCode)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.BlockCode);
                    }
                    else if (buttonName == FormatTextButtons.BlockQuote)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.BlockQuote);
                    }
                    else if (buttonName == FormatTextButtons.Heading1)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading1);
                    }
                    else if (buttonName == FormatTextButtons.Heading2)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading2);
                    }
                    else if (buttonName == FormatTextButtons.Heading3)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading3);
                    }
                    else if (buttonName == FormatTextButtons.Heading4)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading4);
                    }
                    else if (buttonName == FormatTextButtons.Heading5)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading5);
                    }
                    else if (buttonName == FormatTextButtons.Heading6)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading6);
                    }
                    else if (buttonName == FormatTextButtons.InlineCode)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.InlineCode);
                    }
                    else if (buttonName == FormatTextButtons.Italic)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Italic);
                    }
                    else if (buttonName == FormatTextButtons.OrderedList)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.OrderedList);
                    }
                    else if (buttonName == FormatTextButtons.UnorderedList)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.UnorderedList);
                    }
                    else if (buttonName == FormatTextButtons.StrikeThrough)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.StrikeThrough);
                    }
                    else if (buttonName == FormatTextButtons.Bold)
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Bold);
                    }

                    CodeTextboxManager.GetInstance().ReplaceText(CurrentDocument, formattedText);
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< FormatText()");
        }

        public bool CanFormatText(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void ApplyLink(object obj)
        {
            Logger.GetInstance().Debug("ApplyLink() >>");

            try
            {
                var selectedText = CodeTextboxManager.GetInstance().GetSelectedText(CurrentDocument);
                if (!string.IsNullOrEmpty(selectedText))
                {
                    WindowManager.GetInstance().OpenWindow(WindowManager.WindowTypes.UrlSelector);
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< ApplyLink()");
        }

        public void InsertHorizontalRule(object obj)
        {
            Logger.GetInstance().Debug("InsertHorizontalRule() >>");

            try
            {
                CodeTextboxManager.GetInstance().InsertText(CurrentDocument, "---");
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< InsertHorizontalRule()");
        }

        public bool CanInsertHorizontalRule(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void InsertImage(object obj)
        {
            Logger.GetInstance().Debug("InsertImage() >>");

            try
            {
                WindowManager.GetInstance().OpenWindow(WindowManager.WindowTypes.ImageInserter);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< InsertImage()");
        }

        public bool CanInsertImage(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        public void InsertTable(object obj)
        {
            Logger.GetInstance().Debug("InsertTable() >>");

            try
            {
                WindowManager.GetInstance().OpenWindow(WindowManager.WindowTypes.TableInserter);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< InsertTable()");
        }

        public bool CanInsertTable(object obj)
        {
            if (CurrentDocument != null)
            {
                return true;
            }
            return false;
        }

        

        // EVENTS
        public void MainWindowClosingEvent(object obj)
        {
            CloseAllDocuments(obj);
        }

        public bool CanMainWindowClosingEvent(object obj)
        {
            return true;
        }

        public void MainWindowResizedEvent(object obj)
        {
            CodeTextboxManager.GetInstance().RefreshScrollPosition(CurrentDocument);
        }

        public bool CanMainWindowResizedEvent(object obj)
        {
            return true;
        }

        public void HeaderFormattingChangedEvent(object obj)
        {
            Logger.GetInstance().Debug("HeaderFormattingChangedEvent() >>");

            try
            {
                var headingValue = (string)obj;
                var selectedText = CodeTextboxManager.GetInstance().GetSelectedText(CurrentDocument);

                if (!string.IsNullOrEmpty(selectedText))
                {
                    var formattedText = "";

                    if (headingValue == "Heading 1")
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading1);
                    }
                    else if (headingValue == "Heading 2")
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading2);
                    }
                    else if (headingValue == "Heading 3")
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading3);
                    }
                    else if (headingValue == "Heading 4")
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading4);
                    }
                    else if (headingValue == "Heading 5")
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading5);
                    }
                    else if (headingValue == "Heading 6")
                    {
                        formattedText = TextFormatter.Format(selectedText, TextFormatter.TextFormats.Heading6);
                    }
                    else
                    {
                        throw new Exception("Object returned as as unexpected value '" + headingValue + "'.");
                    }

                    CodeTextboxManager.GetInstance().ReplaceText(CurrentDocument, formattedText);
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while formatting the selected text", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< HeaderFormattingChangedEvent()");
        }

        // UTILITIES

        private string GetCss()
        {
            Logger.GetInstance().Debug("GetCss() >>");
            try
            {
                string css;

                using (var sr = new StreamReader(FilePaths.GithubStyleFilePath))
                {
                    css = sr.ReadToEnd();
                }

                Logger.GetInstance().Debug("<< GetCss()");
                return css;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while retrieving the CSS. " + e.Message);
            }
        }

        private void RefreshCurrentHtmlView()
        {
            Logger.GetInstance().Debug("RefreshCurrentHtmlView() >>");

            try
            {
                var css = GetCss();

                var mp = new MarkdownParser();
                var html = mp.Parse(_currentDocument.Markdown, css);
                var tempSourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "Temp\\tempsource.html";
                using (var sw = new StreamWriter(tempSourceFilePath))
                {
                    sw.Write(html);
                }

                _currentDocument.Html = tempSourceFilePath.ToUri();
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while refreshing the HTML view. " + e.Message);
            }

            Logger.GetInstance().Debug("<< RefreshCurrentHtmlView()");
        }

        private void RefreshCurrentHtmlView(SaveResult result)
        {
            Logger.GetInstance().Debug("RefreshCurrentHtmlView() >>");

            try
            {
                // Update source
                CurrentDocument.Html = result.Source;
                // Update tab header
                CurrentDocument.Metadata.FileName = result.FileName;
                // Delete temp files
                DeleteTempSaveFile(result);
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while refreshing the HTML view. " + e.Message);
            }

            Logger.GetInstance().Debug("<< RefreshCurrentHtmlView()");
        }

        private static void DeleteTempSaveFile(SaveResult result)
        {
            Logger.GetInstance().Debug("DeleteTempSaveFile() >>");

            try
            {
                Thread.Sleep(100);
                Directory.Delete(result.TempFile, true);
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< DeleteTempSaveFile()");
        }

        private DocumentModel SaveAsDocument(DocumentModel document)
        {
            Logger.GetInstance().Debug("SaveAsDocument() >>");

            try
            {
                var css = GetCss();

                var result = DocumentSaver.SaveAs(document, css);

                if (result != null)
                {
                    if (document.IsOpen)
                    {
                        RefreshCurrentHtmlView(result);
                    }
                    else
                    {
                        DeleteTempSaveFile(result);
                    }

                    document.IsSaved = true;
                }

                Logger.GetInstance().Debug("<< SaveAsDocument()");

                return document;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private DocumentModel SaveDocument(DocumentModel document)
        {
            Logger.GetInstance().Debug("SaveDocument() >>");

            try
            {
                if (document.Metadata.IsNew)
                {
                    SaveAsDocument(document);
                }
                else
                {
                    var css = GetCss();

                    var result = DocumentSaver.Save(document, css);

                    if (result != null)
                    {
                        if (document.IsOpen)
                        {
                            RefreshCurrentHtmlView(result);
                        }
                        else
                        {
                            DeleteTempSaveFile(result);
                        }

                        document.IsSaved = true;
                    }
                }

                Logger.GetInstance().Debug("<< SaveDocument()");
                return document;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void RemoveDocumentFromDocuments(DocumentModel document)
        {
            Logger.GetInstance().Debug("RemoveDocumentFromDocuments() >>");

            try
            {
                var result = MessageBoxResult.None;

                if (!document.IsSaved)
                {
                    result = MessageBox.Show("Do you want to save your document before closing it down?",
                        "Document not saved warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        SaveDocument(document);
                    }
                }

                if (result != MessageBoxResult.Cancel)
                {
                    var documentToRemove = document;

                    if (documentToRemove == CurrentDocument)
                    {
                        if (Documents.Count > 1)
                        {
                            CurrentDocument = Documents[Documents.Count - 2];
                            Documents.Remove(documentToRemove);
                        }
                        else
                        {
                            if (CurrentPreferences.IsExitOnCloseTheLastTab)
                            {
                                Application.Current.Shutdown();
                            }
                            else
                            {
                                Documents.Clear();
                                CurrentDocument = null;
                            }
                        }
                    }
                    else
                    {
                        Documents.Remove(documentToRemove);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< RemoveDocumentFromDocuments()");
        }

        private void ResetDocumentsOpenState()
        {
            Logger.GetInstance().Debug("ResetDocumentsOpenState() >>");

            foreach (var document in Documents)
            {
                document.IsOpen = false;
            }

            Logger.GetInstance().Debug("<< ResetDocumentsOpenState()");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
