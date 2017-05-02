using System;
using System.IO;
using Microsoft.Win32;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public class DocumentExporter
    {
        public static void ExportHtml(DocumentModel document, string style)
        {
            var saveDialog = new SaveFileDialog
            {
                CreatePrompt = true,
                OverwritePrompt = true,
                Title = "Export HTML",
                Filter = "Hyper Text Markup Language File | *.html"
            };

            var result = saveDialog.ShowDialog();
            if (result != null)
            {
                if (result == true)
                {
                    var mp = new MarkdownParser();
                    var html = mp.Parse(document.Markdown.Markdown, style);
                    using (var sw = new StreamWriter(saveDialog.FileName))
                    {
                        sw.Write(html);
                    }
                }
            }
        }
        public static void ExportMarkdown(DocumentModel document)
        {
            var saveDialog = new SaveFileDialog
            {
                CreatePrompt = true,
                OverwritePrompt = true,
                Title = "Export MD",
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
        public static void ExportPdf(DocumentModel document, string style)
        {
            var saveDialog = new SaveFileDialog
            {
                CreatePrompt = true,
                OverwritePrompt = true,
                Title = "Export PDF",
                Filter = "PDF File | *.pdf"
            };

            var result = saveDialog.ShowDialog();
            if (result != null)
            {
                if (result == true)
                {
                    var mp = new MarkdownParser();
                    var html = mp.Parse(document.Markdown.Markdown, style);
                    var converter = new HtmlToPdfConverter.HtmlToPdfConverter();
                    converter.Convert(html, saveDialog.FileName);
                }
            }
        }
        public static string ExportPdfTemp(DocumentModel document, string style)
        {
            var tempFilePath = AppDomain.CurrentDomain.BaseDirectory + "Temp\\TempPrintPdf.pdf";
            var mp = new MarkdownParser();
            var html = mp.Parse(document.Markdown.Markdown, style);
            var converter = new HtmlToPdfConverter.HtmlToPdfConverter();
            converter.Convert(html, tempFilePath);

            return tempFilePath;
        }
    }
}
