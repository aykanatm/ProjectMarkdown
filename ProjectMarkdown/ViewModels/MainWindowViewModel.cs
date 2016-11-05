using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Awesomium.Core;
using Microsoft.Win32;
using ProjectMarkdown.Annotations;
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
                if (Equals(value, _documents)) return;
                _documents = value;
                OnPropertyChanged(nameof(Documents));
            }
        }

        public DocumentModel CurrentDocument
        {
            get { return _currentDocument; }
            set
            {
                if (Equals(value, _currentDocument)) return;
                _currentDocument = value;
                OnPropertyChanged(nameof(CurrentDocument));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CreateNewDocumentCommand { get; set; }
        public ICommand SaveDocumentCommand { get; set; }
        public ICommand SaveAsDocumentCommand { get; set; }
        public ICommand OpenDocumentCommand { get; set; }
        public ICommand ExportMarkdownCommand { get; set; }
        public ICommand ExportHtmlCommand { get; set; }
        public ICommand ExportPdfCommand { get; set; }
        
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
            SaveAsDocumentCommand = new RelayCommand(SaveAsDocument, CanSaveAsDocument);
            OpenDocumentCommand = new RelayCommand(OpenDocument, CanOpenDocument);
            ExportHtmlCommand = new RelayCommand(ExportHtml, CanExportHtml);
            ExportMarkdownCommand = new RelayCommand(ExportMarkdown, CanExportMarkdown);
            ExportPdfCommand = new RelayCommand(ExportPdf, CanExportPdf);
            // Events
            MainWindowClosingEventCommand = new RelayCommand(MainWindowClosingEvent, CanMainWindowClosingEvent);
        }

        public void CreateNewDocument(object obj)
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
        }
        public bool CanCreateNewDocument(object obj)
        {
            return true;
        }

        public void SaveDocument(object obj)
        {
            if (CurrentDocument.Metadata.IsNew)
            {
                SaveAsDocument(obj);
            }
            else
            {
                string css;
                var cssPath = AppDomain.CurrentDomain.BaseDirectory + "Styles\\github-markdown.css";
                using (var sr = new StreamReader(cssPath))
                {
                    css = sr.ReadToEnd();
                }

                var documentSaver = new DocumentSaver();
                var result = documentSaver.Save(CurrentDocument, css);

                // Since Source property does not update when the same uri is called, we have to load some fake uri before we call the actual uri as a workaround
                // https://github.com/awesomium/awesomium-pub/issues/52
                // Will fix this when 1.7.5 is released
                CurrentDocument.Html.Source = "SomeFakeUri".ToUri();
                CurrentDocument.Html.Source = result.Source;
                // Update tab header
                CurrentDocument.Metadata.FileName = result.FileName;
                // Delete temp files
                Thread.Sleep(100);
                Directory.Delete(result.TempFile, true);
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

        public void SaveAsDocument(object obj)
        {
            string css;
            var cssPath = AppDomain.CurrentDomain.BaseDirectory + "Styles\\github-markdown.css";
            using (var sr = new StreamReader(cssPath))
            {
                css = sr.ReadToEnd();
            }

            var documentSaver = new DocumentSaver();
            var result = documentSaver.SaveAs(CurrentDocument, css);

            // Since Source property does not update when the same uri is called, we have to load some fake uri before we call the actual uri as a workaround
            // https://github.com/awesomium/awesomium-pub/issues/52
            // Will fix this when 1.7.5 is released
            CurrentDocument.Html.Source = "SomeFakeUri".ToUri();
            CurrentDocument.Html.Source = result.Source;
            // Update tab header
            CurrentDocument.Metadata.FileName = result.FileName;
            // Delete temp files
            Thread.Sleep(100);
            Directory.Delete(result.TempFile, true);
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
            var documentLoader = new DocumentLoader();
            var currentDocument = documentLoader.Load();
            if (currentDocument != null)
            {
                Documents.Add(currentDocument);
                CurrentDocument = currentDocument;
            }
        }

        public bool CanOpenDocument(object obj)
        {
            return true;
        }

        public void ExportMarkdown(object obj)
        {
            var documentExporter = new DocumentExporter();
            documentExporter.ExportMarkdown(CurrentDocument);
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
            string css;
            var cssPath = AppDomain.CurrentDomain.BaseDirectory + "Styles\\github-markdown.css";
            using (var sr = new StreamReader(cssPath))
            {
                css = sr.ReadToEnd();
            }

            var documentExporter = new DocumentExporter();
            documentExporter.ExportHtml(CurrentDocument, css);
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
            string css;
            var cssPath = AppDomain.CurrentDomain.BaseDirectory + "Styles\\github-markdown.css";
            using (var sr = new StreamReader(cssPath))
            {
                css = sr.ReadToEnd();
            }

            var documentExporter = new DocumentExporter();
            documentExporter.ExportPdf(CurrentDocument, css);
        }

        public bool CanExportPdf(object obj)
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
