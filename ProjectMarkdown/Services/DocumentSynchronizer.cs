using System.IO;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;
using ProjectMarkdown.Statics;

namespace ProjectMarkdown.Services
{
    public static class DocumentSynchronizer
    {
        public static string Sync(DocumentModel document, string style)
        {
            var mp = new MarkdownParser();
            var html = mp.Parse(document.Markdown, style);
            var htmlFileName = "SyncTemp.html";
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
