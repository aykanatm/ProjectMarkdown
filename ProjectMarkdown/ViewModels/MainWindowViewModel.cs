using System;
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

        // Events
        public ICommand MainWindowClosingEventCommand { get; set; }

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
            SharedEventHandler.GetInstance().OnCodeTextboxScrollChanged += OnCodeTextboxScrollChanged;

            Documents = new ObservableCollection<DocumentModel>();

            Logger.GetInstance().Debug("<< MainWindowViewModel()");
        }

        private void OnCodeTextboxScrollChanged(ScrollResult scrollResult)
        {
            CefChromiumBrowserManager.GetInstance().Scroll(CurrentDocument, scrollResult);
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

            // Events
            MainWindowClosingEventCommand = new RelayCommand(MainWindowClosingEvent, CanMainWindowClosingEvent);

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
                    var fonts = new ObservableCollection<string>();
                    foreach (var fontFamily in FontFamily.Families)
                    {
                        fonts.Add(fontFamily.Name);
                    }

                    CurrentPreferences = new PreferencesModel
                    {
                        Author = Environment.UserName,
                        LogLevels = new ObservableCollection<string> { "DEBUG", "INFO", "ERROR" },
                        CurrentLogLevel = "DEBUG",
                        LogFilePath = AppDomain.CurrentDomain.BaseDirectory + "Log\\app.log",
                        CurrentFont = "Consolas",
                        Fonts = fonts,
                        CurrentFontSize = "11",
                        FontSizes =
                            new ObservableCollection<string> { "8", "9", "10", "11", "12", "14", "18", "24", "30", "36", "48", "60", "72" },
                        IsDoubleClickToCloseDocument = true,
                        IsExitOnCloseTheLastTab = false,
                        IsSyncTextAndHtml = false,
                        IsTabBarLocked = false,
                        IsToolbarHidden = false,
                        IsWordWrap = true,
                        CurrentLanguage = "English",
                        Languages = new ObservableCollection<string> { "English" },
                        CurrentTheme = "Default",
                        Themes = new ObservableCollection<string> { "Default" },
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
        
        private void OnPreferecesSaved(PreferencesModel preferences)
        {
            Logger.GetInstance().Debug("OnPreferencesSaved() >>");
            LoadPreferences(preferences);
            Logger.GetInstance().Debug("<< OnPreferencesSaved()");
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
                    var result = MessageBox.Show("Do you want to save your documents before closimg all documents?", "Document not saved warning",
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
                var windowFactory = new ProductionWindowFactory();
                windowFactory.CreateWindow(ProductionWindowFactory.WindowTypes.Preferences);
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
                var windowFactory = new ProductionWindowFactory();
                windowFactory.CreateWindow(ProductionWindowFactory.WindowTypes.About);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while opening the about window", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< OpenAboutWindow()");
        }

        // EVENTS
        public void MainWindowClosingEvent(object obj)
        {
            
        }

        public bool CanMainWindowClosingEvent(object obj)
        {
            return true;
        }

        private string GetCss()
        {
            Logger.GetInstance().Debug("GetCss() >>");
            try
            {
                string css;
                var cssPath = AppDomain.CurrentDomain.BaseDirectory + "Styles\\github-markdown.css";
                using (var sr = new StreamReader(cssPath))
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
