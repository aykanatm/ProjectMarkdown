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
using ProjectMarkdown.Annotations;
using ProjectMarkdown.ExtensionMethods;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;
using ProjectMarkdown.Services;

namespace ProjectMarkdown.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DocumentModel> _documents;
        private DocumentModel _currentDocument;

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
                ResetDocumentsOpenState();
                _currentDocument.IsOpen = true;

                OnPropertyChanged(nameof(CurrentDocument));
                RefreshCurrentHtmlView();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
        
        // Events
        public ICommand MainWindowClosingEventCommand { get; set; }

        public MainWindowViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            
            LoadCommands();
            Documents = new ObservableCollection<DocumentModel>();
        }

        private void LoadCommands()
        {
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
            // Events
            MainWindowClosingEventCommand = new RelayCommand(MainWindowClosingEvent, CanMainWindowClosingEvent);
        }

        public void CreateNewDocument(object obj)
        {
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
                var document = new DocumentModel(documentFileName);
                Documents.Add(document);
                CurrentDocument = document;

                // Set current document as "open"
                ResetDocumentsOpenState();
                CurrentDocument.IsOpen = true;
                CurrentDocument.IsSaved = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while creating document", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public bool CanCreateNewDocument(object obj)
        {
            return true;
        }

        public void OpenContainingFolder(object obj)
        {
            try
            {
                var parent = Directory.GetParent(CurrentDocument.Metadata.FilePath);
                Process.Start("explorer.exe", parent.FullName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while opening the containing folder",MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            try
            {
                if (CurrentDocument.Metadata.IsNew)
                {
                    SaveAsDocument(obj);
                }
                else
                {
                    var css = GetCss();

                    var result = DocumentSaver.Save(CurrentDocument, css);

                    RefreshCurrentHtmlView(result);

                    CurrentDocument.IsSaved = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while saving the document", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
            try
            {
                var css = GetCss();

                foreach (var document in Documents)
                {
                    if (document.Metadata.IsNew)
                    {
                        var result = DocumentSaver.SaveAs(document, css);

                        if (document.IsOpen)
                        {
                            RefreshCurrentHtmlView(result);
                        }
                        else
                        {
                            DeleteTempSaveFile(result);
                        }
                    }
                    else
                    {
                        var result = DocumentSaver.Save(document, css);
                        if (document.IsOpen)
                        {
                            RefreshCurrentHtmlView(result);
                        }
                        else
                        {
                            DeleteTempSaveFile(result);
                        }
                    }

                    document.IsSaved = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured during saving all documents", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
            try
            {
                var css = GetCss();

                var result = DocumentSaver.SaveAs(CurrentDocument, css);

                if (result != null)
                {
                    RefreshCurrentHtmlView(result);
                    CurrentDocument.IsSaved = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while saving the document", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            try
            {
                var currentDocument = DocumentLoader.Load();
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
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while opening document", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public bool CanOpenDocument(object obj)
        {
            return true;
        }

        public void ExportMarkdown(object obj)
        {
            try
            {
                DocumentExporter.ExportMarkdown(CurrentDocument);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while exporting markdown", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
            try
            {
                var css = GetCss();
                DocumentExporter.ExportHtml(CurrentDocument, css);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while exporting HTML", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
            try
            {
                var css = GetCss();
                DocumentExporter.ExportPdf(CurrentDocument, css);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while exporting PDF", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
                MessageBox.Show(e.Message, "An error occured while printing", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanPrint(object obj)
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
            
        }

        public bool CanMainWindowClosingEvent(object obj)
        {
            return true;
        }

        private string GetCss()
        {
            try
            {
                string css;
                var cssPath = AppDomain.CurrentDomain.BaseDirectory + "Styles\\github-markdown.css";
                using (var sr = new StreamReader(cssPath))
                {
                    css = sr.ReadToEnd();
                }

                return css;
            }
            catch (Exception e)
            {
                
                throw new Exception("An error occured while retrieving the CSS. " + e.Message);
            }
        }

        private void RefreshCurrentHtmlView()
        {
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
        }

        private void RefreshCurrentHtmlView(SaveResult result)
        {
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
        }

        private static void DeleteTempSaveFile(SaveResult result)
        {
            Thread.Sleep(100);
            Directory.Delete(result.TempFile, true);
        }

        private void ResetDocumentsOpenState()
        {
            foreach (var document in Documents)
            {
                document.IsOpen = false;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
