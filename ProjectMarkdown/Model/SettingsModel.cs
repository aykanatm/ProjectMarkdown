using System;
using System.Collections.Generic;

namespace ProjectMarkdown.Model
{
    [Serializable]
    public class SettingsModel
    {
        public bool IsToolbarHidden { get; set; }
        public string LogFilePath { get; set; }
        public string Author { get; set; }
        public bool IsTabBarLocked { get; set; }
        public bool IsDoubleClickToCloseDocument { get; set; }
        public bool IsExitOnCloseTheLastTab { get; set; }
        public bool IsWordWrap { get; set; }
        public bool IsSyncTextAndHtml { get; set; }
        public string CurrentLogLevel { get; set; }
        public List<string> LogLevels { get; set; }
        public string CurrentLanguage { get; set; }
        public List<string> Languages { get; set; }
        public string CurrentFont { get; set; }
        public List<string> Fonts { get; set; }
        public string CurrentFontSize { get; set; }
        public List<string> FontSizes { get; set; }
        public string CurrentTheme { get; set; }
        public List<string> Themes { get; set; }
    }
}
