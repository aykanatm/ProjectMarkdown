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
            CreateNewDocumentCommand = new RelayCommand(CreateNewDocument, CanCreateNewDocument);
            SaveDocumentCommand = new RelayCommand(SaveDocument, CanSaveDocument);
            SaveAsDocumentCommand = new RelayCommand(SaveAsDocument, CanSaveAsDocument);
            OpenDocumentCommand = new RelayCommand(OpenDocument, CanOpenDocument);
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
                var documentSaver = new DocumentSaver();
                var result = documentSaver.Save(CurrentDocument);

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
            var documentSaver = new DocumentSaver();
            var result = documentSaver.SaveAs(CurrentDocument);

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
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Markdown File | *.md";
            var result = openFileDialog.ShowDialog();
            
            if (result != null)
            {
                if (result == true)
                {
                    var displayFileName = openFileDialog.SafeFileName;
                    var markdownFileName = openFileDialog.FileName;
                    var htmlFileName = openFileDialog.FileName + ".html";

                    var document = new DocumentModel(displayFileName);
                    using (var sr = new StreamReader(markdownFileName))
                    {
                        document.Markdown.Markdown = sr.ReadToEnd();
                    }

                    document.Markdown.MarkdownPath = markdownFileName;

                    if (File.Exists(htmlFileName))
                    {
                        document.Html.HtmlPath = htmlFileName;
                        document.Html.Source = htmlFileName.ToUri();
                    }
                    else
                    {
                        MessageBox.Show("Unable to retrive the HTML component of the markdown document.",
                            "Unable to retrive the HTML component", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    Documents.Add(document);
                    CurrentDocument = document;
                }
            }
        }

        public bool CanOpenDocument(object obj)
        {
            return true;
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
