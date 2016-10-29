using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProjectMarkdown.Annotations;

namespace ProjectMarkdown.Model
{
    [Serializable]
    public class DocumentMetadata : INotifyPropertyChanged
    {
        private string _fileName;
        private string _author;
        private string _password;
        private bool _isPasswordProtected;
        private bool _isNew;

        public string Author
        {
            get { return _author; }
            set
            {
                if (value == _author) return;
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (value == _fileName) return;
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                if (value == _isNew) return;
                _isNew = value;
                OnPropertyChanged(nameof(IsNew));
            }
        }

        public DocumentMetadata()
        {
            
        }

        public DocumentMetadata(string fileName)
        {
            FileName = fileName;
            IsNew = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
