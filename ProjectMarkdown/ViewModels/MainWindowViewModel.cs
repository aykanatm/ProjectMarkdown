using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ProjectMarkdown.Annotations;

namespace ProjectMarkdown.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _currentDocumentPath;
        private string _currentText;
        private string _currentHtml;

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

        public string CurrentHtml
        {
            get { return _currentHtml; }
            set
            {
                if (value == _currentHtml) return;
                _currentHtml = value;
                OnPropertyChanged(nameof(CurrentHtml));
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
            using (var sr = new StreamReader("Example.html"))
            {
                CurrentHtml = sr.ReadToEnd();
            }
            LoadCommands();
            CurrentDocumentPath = "Untitled.md";
        }

        private void LoadCommands()
        {
            SaveDocumentCommand = new RelayCommand(SaveDocument, CanSaveDocument);
        }

        public void SaveDocument(object obj)
        {
            using (var sr = new StreamReader("Example.html"))
            {
                CurrentHtml = sr.ReadToEnd();
            }
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
