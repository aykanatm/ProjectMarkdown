using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectMarkdown.Annotations;

namespace ProjectMarkdown.Model
{
    public class DocumentHtml : INotifyPropertyChanged
    {
        private string _htmlPath;
        private Uri _source;

        public Uri Source
        {
            get { return _source; }
            set
            {
                if (Equals(value, _source)) return;
                _source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public string HtmlPath
        {
            get { return _htmlPath; }
            set
            {
                if (value == _htmlPath) return;
                _htmlPath = value;
                OnPropertyChanged(nameof(HtmlPath));
            }
        }

        public DocumentHtml(Uri source)
        {
            Source = source;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
