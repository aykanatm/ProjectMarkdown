using System;
using System.IO;
using System.IO.Compression;
using Awesomium.Core;
using CustomIO;
using Microsoft.Win32;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public class DocumentSaver
    {
        public static SaveResult SaveAs(DocumentModel document, string style)
        {
            var saveDialog = new SaveFileDialog
            {
                CreatePrompt = true,
                OverwritePrompt = true,
                Title = "Save a PMD file",
                Filter = "Project Markdown File | *.pmd"
            };

            var result = saveDialog.ShowDialog();

            if (result != null)
            {
                if (result == true)
                {
                    if (!Directory.Exists(saveDialog.FileName + "_temp"))
                    {
                        var parentFolder = Directory.CreateDirectory(saveDialog.FileName + "_temp").FullName;

                        var mp = new MarkdownParser();
                        // Generate HTML
                        var html = mp.Parse(document.Markdown.Markdown, style);

                        var markdownFileName = saveDialog.SafeFileName + ".md";
                        var markdownFilePath = parentFolder + "\\" + markdownFileName;
                        var htmlFileName = saveDialog.SafeFileName + ".html";
                        var htmlFilePath = parentFolder + "\\" + htmlFileName;
                        var xmlFileName = saveDialog.SafeFileName + ".xml";
                        var metadataFilePath = parentFolder + "\\" + xmlFileName;
                        // Generate MD file
                        using (var sw = new StreamWriter(markdownFilePath))
                        {
                            sw.Write(document.Markdown.Markdown);
                        }
                        // Generate HTML file
                        using (var sw = new StreamWriter(htmlFilePath))
                        {
                            sw.Write(html);
                        }
                        // Generate XML file
                        document.Metadata.FilePath = saveDialog.FileName;
                        document.Metadata.FileName = saveDialog.SafeFileName;
                        document.Metadata.IsNew = false;
                        var gxs = new GenericXmlSerializer<DocumentMetadata>();
                        gxs.Serialize(document.Metadata, metadataFilePath);
                        // Generate the package
                        if (File.Exists(document.Metadata.FilePath))
                        {
                            File.Delete(document.Metadata.FilePath);
                        }
                        ZipFile.CreateFromDirectory(parentFolder, saveDialog.FileName);
                        // Update the view
                        var saveResult = new SaveResult
                        {
                            FileName = saveDialog.SafeFileName,
                            Source = htmlFilePath.ToUri(),
                            TempFile = saveDialog.FileName + "_temp"
                        };
                        return saveResult;
                    }
                }
            }
            return null;
        }

        public static SaveResult Save(DocumentModel document, string style)
        {
            if (!Directory.Exists(document.Metadata.FilePath + "_temp"))
            {
                var parentFolder = Directory.CreateDirectory(document.Metadata.FilePath + "_temp").FullName;

                var mp = new MarkdownParser();
                // Generate HTML
                var html = mp.Parse(document.Markdown.Markdown, style);

                var markdownFileName = document.Metadata.FileName + ".md";
                var markdownFilePath = parentFolder + "\\" + markdownFileName;
                var htmlFileName = document.Metadata.FileName + ".html";
                var htmlFilePath = parentFolder + "\\" + htmlFileName;
                var xmlFileName = document.Metadata.FileName + ".xml";
                var metadataFilePath = parentFolder + "\\" + xmlFileName;
                // Generate MD file
                using (var sw = new StreamWriter(markdownFilePath))
                {
                    sw.Write(document.Markdown.Markdown);
                }
                // Generate HTML file
                using (var sw = new StreamWriter(htmlFilePath))
                {
                    sw.Write(html);
                }
                // Generate XML file
                document.Metadata.FilePath = document.Metadata.FilePath;
                document.Metadata.FileName = document.Metadata.FileName;
                document.Metadata.IsNew = false;
                var gxs = new GenericXmlSerializer<DocumentMetadata>();
                gxs.Serialize(document.Metadata, metadataFilePath);
                // Generate style
                var cssFilePath = AppDomain.CurrentDomain.BaseDirectory + "Styles\\github-markdown.css";
                if (!Directory.Exists(parentFolder + "\\Styles"))
                {
                    Directory.CreateDirectory(parentFolder + "\\Styles");
                }

                if (!File.Exists(parentFolder + "\\Styles\\github-markdown.css"))
                {
                    File.Copy(cssFilePath, parentFolder + "\\Styles\\github-markdown.css");
                }
                // Generate the package
                if (File.Exists(document.Metadata.FilePath))
                {
                    File.Delete(document.Metadata.FilePath);
                }
                ZipFile.CreateFromDirectory(parentFolder, document.Metadata.FilePath);
                // Update the view
                var saveResult = new SaveResult
                {
                    FileName = document.Metadata.FileName,
                    Source = htmlFilePath.ToUri(),
                    TempFile = document.Metadata.FilePath + "_temp"
                };
                return saveResult;
            }
            return null;
        }
    }
}
