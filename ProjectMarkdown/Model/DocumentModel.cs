using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectMarkdown.Annotations;
using ProjectMarkdown.ExtensionMethods;

namespace ProjectMarkdown.Model
{
    public class DocumentModel : INotifyPropertyChanged
    {
        public DocumentMetadata Metadata
        {
            get { return _metadata; }
            set
            {
                _metadata = value;
                OnPropertyChanged(nameof(Metadata));
            }
        }

        public DocumentMarkdown Markdown
        {
            get { return _markdown; }
            set
            {
                _markdown = value;
                OnPropertyChanged(nameof(Markdown));
            }
        }

        public DocumentHtml Html
        {
            get { return _html; }
            set
            {
                _html = value;
                OnPropertyChanged(nameof(Html));
            }
        }

        
        private DocumentMetadata _metadata;
        private DocumentMarkdown _markdown;
        private DocumentHtml _html;


        public event PropertyChangedEventHandler PropertyChanged;

        public DocumentModel(string documentName)
        {
            Metadata = new DocumentMetadata(documentName);
            Markdown = new DocumentMarkdown("");
            // TODO Fix the ToUri() method
            Html = new DocumentHtml("C:\\".ToUri());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
