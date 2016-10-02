using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Awesomium.Core;
using ProjectMarkdown.Annotations;

namespace ProjectMarkdown.Model
{
    public class DocumentModel : INotifyPropertyChanged
    {
        private string _header;
        private string _markdownText;
        private Uri _source;

        public string Header
        {
            get { return _header; }
            set
            {
                if (value == _header) return;
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public string MarkdownText
        {
            get { return _markdownText; }
            set
            {
                if (value == _markdownText) return;
                _markdownText = value;
                OnPropertyChanged(nameof(MarkdownText));
            }
        }

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

        public string MarkdownPath { get; set; }
        public string HtmlPath { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public DocumentModel(string documentName)
        {
            Header = documentName;
            MarkdownText = "";
            Source = "".ToUri();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
