using System.IO;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;
using ProjectMarkdown.Statics;

namespace ProjectMarkdown.Services
{
    public static class DocumentSynchronizer
    {
        private static string _syncTemp1 = "SyncTemp_1.html";
        private static string _syncTemp2 = "SyncTemp_2.html";
        private static string _currentTempFileName;

        public static string Sync(DocumentModel document, string style)
        {
            if (string.IsNullOrEmpty(_currentTempFileName))
            {
                _currentTempFileName = _syncTemp1;
            }
            else
            {
                if (_currentTempFileName == _syncTemp1)
                {
                    _currentTempFileName = _syncTemp2;
                }
                else
                {
                    _currentTempFileName = _syncTemp1;
                }
            }

            var mp = new MarkdownParser();
            var html = mp.Parse(document.Markdown, style);
            var htmlFileName = _currentTempFileName;
            // Generate HTML file
            var htmlFilePath = FolderPaths.TempFolderPath + "\\" + htmlFileName;
            using (var sw = new StreamWriter(htmlFilePath))
            {
                sw.Write(html);
            }

            return htmlFilePath;
        }
    }
}
