using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Awesomium.Core;
using Microsoft.Win32;
using ProjectMarkdown.Annotations;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;

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

        public ICommand SaveDocumentCommand { get; set; }
        public ICommand CreateNewDocumentCommand { get; set; }
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
            SaveDocumentCommand = new RelayCommand(SaveDocument, CanSaveDocument);
            CreateNewDocumentCommand = new RelayCommand(CreateNewDocument, CanCreateNewDocument);
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
                var checkForDuplicateFileName = Documents.FirstOrDefault(d => d.Header == documentFileName);
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
        }
        public bool CanCreateNewDocument(object obj)
        {
            return true;
        }

        public void SaveDocument(object obj)
        {
            if (CurrentDocument != null)
            {
                var mp = new MarkdownParser();
                var html = mp.Parse(CurrentDocument.MarkdownText);
                var saveDialog = new SaveFileDialog
                {
                    CreatePrompt = true,
                    OverwritePrompt = true,
                    Filter = "Markdown File | *.md"
                };
                var result = saveDialog.ShowDialog();
                if (result != null)
                {
                    if (result == true)
                    {
                        var markdownFileName = saveDialog.FileName;
                        var htmlFileName = saveDialog.FileName + ".html";
                        using (var sw = new StreamWriter(markdownFileName))
                        {
                            sw.Write(CurrentDocument.MarkdownText);
                        }
                        using (var sw = new StreamWriter(htmlFileName))
                        {
                            sw.Write(html);
                        }

                        var cssFilePath = AppDomain.CurrentDomain.BaseDirectory + "Styles\\github-markdown.css";
                        var fileInfo = new FileInfo(markdownFileName);
                        var parentDirectoryPath = fileInfo.DirectoryName;

                        if (!Directory.Exists(parentDirectoryPath + "\\Styles"))
                        {
                            Directory.CreateDirectory(parentDirectoryPath + "\\Styles");
                        }

                        if (!File.Exists(parentDirectoryPath + "\\Styles\\github-markdown.css"))
                        {
                            File.Copy(cssFilePath, parentDirectoryPath + "\\Styles\\github-markdown.css");
                        }

                        // Since Source property does not update when the same uri is called, we have to load some fake uri before we call the actual uri as a workaround
                        // https://github.com/awesomium/awesomium-pub/issues/52
                        // Will fix this when 1.7.5 is released
                        CurrentDocument.Source = "SomeFakeUri".ToUri();
                        CurrentDocument.Source = htmlFileName.ToUri();
                    }
                }
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
