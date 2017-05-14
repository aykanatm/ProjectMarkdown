using System;
using System.IO;
using LogUtils;
using Microsoft.Win32;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public static class DocumentExporter
    {
        public static void ExportHtml(DocumentModel document, string style)
        {
            Logger.GetInstance().Debug("ExportHtml() >>");

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
                throw e;
            }

            Logger.GetInstance().Debug("<< ExportHtml()");
        }

        public static void ExportMarkdown(DocumentModel document)
        {
            Logger.GetInstance().Debug("ExportMarkdown() >>");

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
                throw e;
            }

            Logger.GetInstance().Debug("<< ExportMarkdown()");
        }

        public static void ExportPdf(DocumentModel document, string style)
        {
            Logger.GetInstance().Debug("ExportPdf() >>");

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
                throw e;
            }

            Logger.GetInstance().Debug("<< ExportPdf()");
        }
        public static string ExportPdfTemp(DocumentModel document, string style)
        {
            Logger.GetInstance().Debug("ExportPdfTemp() >>");

            try
            {
                var tempFilePath = AppDomain.CurrentDomain.BaseDirectory + "Temp\\TempPrintPdf.pdf";
                var mp = new MarkdownParser();
                var html = mp.Parse(document.Markdown, style);
                var converter = new HtmlToPdfConverter.HtmlToPdfConverter();
                converter.Convert(html, tempFilePath);

                Logger.GetInstance().Debug("<< ExportPdfTemp()");
                return tempFilePath;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
