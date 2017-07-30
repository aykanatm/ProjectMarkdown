using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectMarkdown.Annotations;

namespace ProjectMarkdown.Model
{
    [Serializable]
    public class DocumentMetadata : INotifyPropertyChanged
    {
        private string _fileName;
        private string _author;
        private bool _isNew;
        private string _filePath;

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                OnPropertyChanged(nameof(IsNew));
            }
        }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        public DocumentMetadata()
        {
            
        }

        public DocumentMetadata(string fileName)
        {
            FileName = fileName;
            IsNew = true;
            Author = Environment.UserName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
