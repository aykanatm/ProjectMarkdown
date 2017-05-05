using System;
using System.IO;
using Microsoft.Win32;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public static class DocumentExporter
    {
        public static void ExportHtml(DocumentModel document, string style)
        {
            try
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
                        var html = mp.Parse(document.Markdown, style);
                        using (var sw = new StreamWriter(saveDialog.FileName))
                        {
                            sw.Write(html);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while exporting HTML. " + e.Message);
            }
        }

        public static void ExportMarkdown(DocumentModel document)
        {
            try
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
                            sw.Write(document.Markdown);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while exporting Markdown. " + e.Message);
            }
        }

        public static void ExportPdf(DocumentModel document, string style)
        {
            try
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
                        var html = mp.Parse(document.Markdown, style);
                        var converter = new HtmlToPdfConverter.HtmlToPdfConverter();
                        converter.Convert(html, saveDialog.FileName);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while exporting PDF. " + e.Message);
            }
        }
        public static string ExportPdfTemp(DocumentModel document, string style)
        {
            try
            {
                var tempFilePath = AppDomain.CurrentDomain.BaseDirectory + "Temp\\TempPrintPdf.pdf";
                var mp = new MarkdownParser();
                var html = mp.Parse(document.Markdown, style);
                var converter = new HtmlToPdfConverter.HtmlToPdfConverter();
                converter.Convert(html, tempFilePath);

                return tempFilePath;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while exporting temporary PDF. " + e.Message);
            }
        }
    }
}
