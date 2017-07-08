using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FastColoredTextBoxNS;
using ProjectMarkdown.Annotations;
using ProjectMarkdown.ViewModels;

namespace ProjectMarkdown.Model
{
    public class DocumentModel : INotifyPropertyChanged
    {
        public bool IsOpenedFromMenu { get; set; }

        public bool IsSaved
        {
            get { return _isSaved; }
            set
            {
                _isSaved = value;
                OnPropertyChanged(nameof(IsSaved));
            }
        }

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        public DocumentMetadata Metadata
        {
            get { return _metadata; }
            set
            {
                _metadata = value;
                OnPropertyChanged(nameof(Metadata));
            }
        }

        public string Markdown
        {
            get { return _markdown; }
            set
            {
                _markdown = value;

                if (IsOpenedFromMenu)
                {
                    // This is here to protect IsSaved's value from changing 
                    // when a document is opened for the first time.
                    IsOpenedFromMenu = false;
                }
                else
                {
                    // If the document modified after opening it for the first time
                    IsSaved = false;
                }

                OnPropertyChanged(nameof(Markdown));
            }
        }

        public Uri Html
        {
            get { return _html; }
            set
            {
                _html = value;
                OnPropertyChanged(nameof(Html));
            }
        }

        public ObservableCollection<UndoableCommand> History
        {
            get { return _history; }
            set
            {
                _history = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UndoableCommand> RedoStack
        {
            get { return _redoStack; }
            set
            {
                _redoStack = value;
                OnPropertyChanged();
            }
        }

        private DocumentMetadata _metadata;
        private string _markdown;
        private Uri _html;
        private bool _isOpen;
        private bool _isSaved;
        private ObservableCollection<UndoableCommand> _history;
        private ObservableCollection<UndoableCommand> _redoStack;

        public ICommand CloseDocumentButtonCommand { get; set; }

        private readonly MainWindowViewModel _mainWindowViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public DocumentModel(MainWindowViewModel mainWindowViewModel, string documentName)
        {
            _mainWindowViewModel = mainWindowViewModel;

            LoadCommands();

            Metadata = new DocumentMetadata(documentName);
            Markdown = "";
            Html = new Uri("C:\\");
            IsSaved = false;
        }

        private void LoadCommands()
        {
            CloseDocumentButtonCommand = new RelayCommand(CloseDocumentButton, CanCloseDocumentButton);
        }

        public void CloseDocumentButton(object obj)
        {
            try
            {
                _mainWindowViewModel.RemoveDocumentFromDocuments(this);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while saving the document", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanCloseDocumentButton(object obj)
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
