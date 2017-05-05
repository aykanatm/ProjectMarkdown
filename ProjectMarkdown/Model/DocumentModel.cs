using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectMarkdown.Annotations;

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

        
        private DocumentMetadata _metadata;
        private string _markdown;
        private Uri _html;
        private bool _isOpen;
        private bool _isSaved;


        public event PropertyChangedEventHandler PropertyChanged;

        public DocumentModel(string documentName)
        {
            Metadata = new DocumentMetadata(documentName);
            // Markdown = "";
            // Html = new Uri("C:\\");
            IsSaved = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
