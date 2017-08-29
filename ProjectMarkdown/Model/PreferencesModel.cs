using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectMarkdown.Annotations;

namespace ProjectMarkdown.Model
{
    [Serializable]
    public class PreferencesModel : INotifyPropertyChanged
    {
        private bool _isToolbarHidden;
        private string _logFilePath;
        private string _author;
        private bool _isTabBarLocked;
        private bool _isDoubleClickToCloseDocument;
        private bool _isExitOnCloseTheLastTab;
        private bool _isWordWrap;
        private bool _isSyncTextAndHtml;
        private string _currentLogLevel;
        private string _currentLanguage;
        private string _currentFont;
        private string _currentFontSize;
        private string _currentTheme;
        private bool _isLoggingEnabled;
        private bool _isScrollBarsSynced;

        public bool IsLoggingEnabled
        {
            get { return _isLoggingEnabled; }
            set
            {
                _isLoggingEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsToolbarHidden
        {
            get { return _isToolbarHidden; }
            set
            {
                _isToolbarHidden = value;
                OnPropertyChanged();
            }
        }

        public string LogFilePath
        {
            get { return _logFilePath; }
            set
            {
                _logFilePath = value;
                OnPropertyChanged();
            }
        }

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged();
            }
        }

        public bool IsTabBarLocked
        {
            get { return _isTabBarLocked; }
            set
            {
                _isTabBarLocked = value;
                OnPropertyChanged();
            }
        }

        public bool IsDoubleClickToCloseDocument
        {
            get { return _isDoubleClickToCloseDocument; }
            set
            {
                _isDoubleClickToCloseDocument = value;
                OnPropertyChanged();
            }
        }

        public bool IsExitOnCloseTheLastTab
        {
            get { return _isExitOnCloseTheLastTab; }
            set
            {
                _isExitOnCloseTheLastTab = value;
                OnPropertyChanged();
            }
        }

        public bool IsWordWrap
        {
            get { return _isWordWrap; }
            set
            {
                _isWordWrap = value;
                OnPropertyChanged();
            }
        }

        public bool IsScrollBarsSynced
        {
            get { return _isScrollBarsSynced; }
            set
            {
                _isScrollBarsSynced = value;
                OnPropertyChanged();
            }
        }

        public bool IsSyncTextAndHtml
        {
            get { return _isSyncTextAndHtml; }
            set
            {
                _isSyncTextAndHtml = value;
                OnPropertyChanged();
            }
        }

        public string CurrentLogLevel
        {
            get { return _currentLogLevel; }
            set
            {
                _currentLogLevel = value;
                OnPropertyChanged();
            }
        }

        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                _currentLanguage = value;
                OnPropertyChanged();
            }
        }

        public string CurrentFont
        {
            get { return _currentFont; }
            set
            {
                _currentFont = value;
                OnPropertyChanged();
            }
        }

        public string CurrentFontSize
        {
            get { return _currentFontSize; }
            set
            {
                _currentFontSize = value;
                OnPropertyChanged();
            }
        }

        public string CurrentTheme
        {
            get { return _currentTheme; }
            set
            {
                _currentTheme = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
