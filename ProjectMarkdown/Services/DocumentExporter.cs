using System.IO;
using Microsoft.Win32;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public class DocumentExporter
    {
        public void ExportHtml(DocumentModel document)
        {
            var saveDialog = new SaveFileDialog
            {
                CreatePrompt = true,
                OverwritePrompt = true,
                Filter = "Hyper Text Markup Language File | *.html"
            };

            var result = saveDialog.ShowDialog();
            if (result != null)
            {
                if (result == true)
                {
                    var mp = new MarkdownParser();
                    var html = mp.Parse(document.Markdown.Markdown);
                    using (var sw = new StreamWriter(saveDialog.FileName))
                    {
                        sw.Write(html);
                    }
                }
            }
        }

        public void ExportMarkdown(DocumentModel document)
        {
            var saveDialog = new SaveFileDialog
            {
                CreatePrompt = true,
                OverwritePrompt = true,
                Filter = "Markdown File | *.md"
            };

            var result = saveDialog.ShowDialog();
            if (result != null)
            {
                if (result == true)
                {
                    using (var sw = new StreamWriter(saveDialog.FileName))
                    {
                        sw.Write(document.Markdown.Markdown);
                    }
                }
            }
        }
    }
}
