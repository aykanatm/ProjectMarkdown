using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.Statics
{
    public static class FolderPaths
    {
        public static readonly string DefaultLogFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Log";
        public static readonly string SettingsFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Configuration";
    }
}
