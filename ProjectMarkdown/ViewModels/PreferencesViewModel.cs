using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IOUtils;
using ProjectMarkdown.Annotations;
using ProjectMarkdown.ExtensionMethods;
using ProjectMarkdown.Model;
using ProjectMarkdown.Statics;

namespace ProjectMarkdown.ViewModels
{
    public class PreferencesViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _languages;
        private bool _isToolbarHidden;
        private ObservableCollection<string> _logLevels;
        private string _logFilePath;
        private string _author;
        private bool _isTabBarLocked;
        private bool _isDoubleClickToCloseDocument;
        private bool _isExitOnCloseTheLastTab;
        private ObservableCollection<string> _themes;
        private bool _isWordWrap;
        private bool _isSyncTextAndHtml;
        private ObservableCollection<string> _fonts;
        private ObservableCollection<string> _fontSizes;
        private string _currentLogLevel;
        private string _currentTheme;
        private string _currentLanguage;
        private string _currentFont;
        private string _currentFontSize;


        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                _currentLanguage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
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

        public ObservableCollection<string> Themes
        {
            get { return _themes; }
            set
            {
                _themes = value;
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

        public ObservableCollection<string> Fonts
        {
            get { return _fonts; }
            set
            {
                _fonts = value;
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

        public ObservableCollection<string> FontSizes
        {
            get { return _fontSizes; }
            set
            {
                _fontSizes = value;
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

        public ObservableCollection<string> LogLevels
        {
            get { return _logLevels; }
            set
            {
                _logLevels = value;
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

        public bool IsSyncTextAndHtml
        {
            get { return _isSyncTextAndHtml; }
            set
            {
                _isSyncTextAndHtml = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectLogFileCommand { get; set; }
        public ICommand CancelPreferenceChangesCommand { get; set; }
        public ICommand SavePreferencesCommand { get; set; }

        public PreferencesViewModel()
        {
            LoadCommands();

            var settingsPath = FolderPaths.SettingsFolderPath + "\\settings.xml";

            var gxs = new GenericXmlSerializer<SettingsModel>();
            if (!File.Exists(settingsPath))
            {
                var fonts = new List<string>();

                foreach (var fontFamily in FontFamily.Families)
                {
                    fonts.Add(fontFamily.Name);
                }

                var defaultSettings = new SettingsModel
                {
                    Author = Environment.UserName,
                    LogLevels = new List<string> { "DEBUG", "INFO", "ERROR"},
                    CurrentLogLevel = "DEBUG",
                    LogFilePath = AppDomain.CurrentDomain.BaseDirectory + "Log\\app.log",
                    CurrentFont = "Consolas",
                    Fonts = fonts,
                    CurrentFontSize = "11",
                    FontSizes =
                        new List<string> {"8", "9", "10", "11", "12", "14", "18", "24", "30", "36", "48", "60", "72"},
                    IsDoubleClickToCloseDocument = true,
                    IsExitOnCloseTheLastTab = false,
                    IsSyncTextAndHtml = false,
                    IsTabBarLocked = false,
                    IsToolbarHidden = false,
                    IsWordWrap = true,
                    CurrentLanguage = "English",
                    Languages = new List<string> { "English"},
                    CurrentTheme = "Default",
                    Themes = new List<string> { "Default"}
                };

                gxs.Serialize(defaultSettings, settingsPath);
                LoadSettings(defaultSettings);
            }
            else
            {
                var settings = gxs.DeSerialize(AppDomain.CurrentDomain.BaseDirectory + "settings.xml");
                LoadSettings(settings);
            }
        }

        private void LoadSettings(SettingsModel settings)
        {
            Author = settings.Author;
            CurrentLogLevel = settings.CurrentLogLevel;
            LogLevels = settings.LogLevels.ToObservableCollection();
            LogFilePath = settings.LogFilePath;
            CurrentFont = settings.CurrentFont;
            Fonts = settings.Fonts.ToObservableCollection();
            CurrentFontSize = settings.CurrentFontSize;
            FontSizes = settings.FontSizes.ToObservableCollection();
            IsDoubleClickToCloseDocument = settings.IsDoubleClickToCloseDocument;
            IsExitOnCloseTheLastTab = settings.IsExitOnCloseTheLastTab;
            IsSyncTextAndHtml = settings.IsSyncTextAndHtml;
            IsTabBarLocked = settings.IsTabBarLocked;
            IsToolbarHidden = settings.IsToolbarHidden;
            IsWordWrap = settings.IsWordWrap;
            CurrentLanguage = settings.CurrentLanguage;
            Languages = settings.Languages.ToObservableCollection();
            CurrentTheme = settings.CurrentTheme;
            Themes = settings.Themes.ToObservableCollection();
        }

        private void LoadCommands()
        {
            SelectLogFileCommand = new RelayCommand(SelectLogFile, CanSelectLogFile);
            CancelPreferenceChangesCommand = new RelayCommand(CancelPreferences, CanCancelPreferences);
            SavePreferencesCommand = new RelayCommand(SavePreferences, CanSavePreferences);
        }

        public bool CanSelectLogFile(object obj)
        {
            return true;
        }

        public void SelectLogFile(object obj)
        {
            
        }

        public bool CanCancelPreferences(object obj)
        {
            return true;
        }

        public void CancelPreferences(object obj)
        {
            
        }

        public bool CanSavePreferences(object obj)
        {
            return true;
        }

        public void SavePreferences(object obj)
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
