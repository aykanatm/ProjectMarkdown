using System;
using System.Collections.Generic;
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

namespace ProjectMarkdown.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _currentDocumentPath;
        private string _currentText;
        private Uri _currentSource;

        public string CurrentDocumentPath
        {
            get { return _currentDocumentPath; }
            set
            {
                if (value == _currentDocumentPath) return;
                _currentDocumentPath = value;
                OnPropertyChanged(nameof(CurrentDocumentPath));
            }
        }

        public string CurrentText
        {
            get { return _currentText; }
            set
            {
                if (value == _currentText) return;
                _currentText = value;
                OnPropertyChanged(nameof(CurrentText));
            }
        }

        public Uri CurrentSource
        {
            get { return _currentSource; }
            set
            {
                if (value == _currentSource) return;
                _currentSource = value;
                OnPropertyChanged(nameof(CurrentSource));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SaveDocumentCommand { get; set; }

        public MainWindowViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            
            LoadCommands();
            CurrentDocumentPath = "Untitled.md";

            

            using (var sr = new StreamReader("MarkdownTest.txt"))
            {
                string line;
                do
                {
                    line = sr.ReadLine();
                    CurrentText += line + "\r\n";
                } while (line != null);
            }

            var mp = new MarkdownParser();
            var html = mp.Parse(CurrentText);
            using (var sw = new StreamWriter("MarkdownResult.html"))
            {
                sw.Write(html);
            }

            CurrentSource = (AppDomain.CurrentDomain.BaseDirectory + "MarkdownResult.html").ToUri();
        }

        private void LoadCommands()
        {
            SaveDocumentCommand = new RelayCommand(SaveDocument, CanSaveDocument);
        }

        public void SaveDocument(object obj)
        {
            
        }

        public bool CanSaveDocument(object obj)
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
