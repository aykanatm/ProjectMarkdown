using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectMarkdown.Annotations;

namespace ProjectMarkdown.Model
{
    public class DocumentMarkdown : INotifyPropertyChanged
    {
        private string _markdown;
        private string _markdownPath;

        public string MarkdownPath
        {
            get { return _markdownPath; }
            set
            {
                if (value == _markdownPath) return;
                _markdownPath = value;
                OnPropertyChanged(nameof(MarkdownPath));
            }
        }

        public string Markdown
        {
            get { return _markdown; }
            set
            {
                if (value == _markdown) return;
                _markdown = value;
                OnPropertyChanged(nameof(Markdown));
            }
        }

        public DocumentMarkdown(string markdown)
        {
            Markdown = markdown;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
