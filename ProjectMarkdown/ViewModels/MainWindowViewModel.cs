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
            
            //CurrentDocumentPath = "Untitled.md";

            //using (var sr = new StreamReader("MarkdownTest.txt"))
            //{
            //    string line;
            //    do
            //    {
            //        line = sr.ReadLine();
            //        CurrentText += line + "\r\n";
            //    } while (line != null);
            //}

            //var mp = new MarkdownParser();
            //var html = mp.Parse(CurrentText);
            //using (var sw = new StreamWriter("MarkdownResult.html"))
            //{
            //    sw.Write(html);
            //}

            //CurrentSource = (AppDomain.CurrentDomain.BaseDirectory + "MarkdownResult.html").ToUri();
            //CurrentSource = (AppDomain.CurrentDomain.BaseDirectory + "Empty").ToUri();
            //CurrentFileName = "Untitled.md";
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
            //var mp = new MarkdownParser();
            //var html = mp.Parse(CurrentText);
            //using (var sw = new StreamWriter(_currentHtmlFilePath))
            //{
            //    sw.Write(html);
            //}
            //// Since Source property does not update when the same uri is called, we have to load some fake uri before we call the actual uri as a workaround
            //// https://github.com/awesomium/awesomium-pub/issues/52
            //// Will fix this when 1.7.5 is released
            //CurrentSource = "SomeFakeUri".ToUri();
            //CurrentSource = _currentHtmlFilePath.ToUri();
        }

        public bool CanSaveDocument(object obj)
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
