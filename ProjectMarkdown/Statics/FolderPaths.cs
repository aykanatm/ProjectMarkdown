using System;

namespace ProjectMarkdown.Statics
{
    public static class FolderPaths
    {
        public static readonly string DefaultLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Log";
        public static readonly string PreferencesFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Configuration";
        public static readonly string StylesFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Styles";
        public static readonly string TempFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Temp";
    }
}
