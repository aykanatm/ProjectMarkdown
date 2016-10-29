using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium.Core;
using CustomIO;
using Microsoft.Win32;
using ProjectMarkdown.MarkdownLibrary;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public class DocumentSaver
    {
        public SaveResult Save(DocumentModel document)
        {
            if (!document.Metadata.IsNew)
            {
                var saveDialog = new SaveFileDialog
                {
                    CreatePrompt = true,
                    OverwritePrompt = true,
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
                            var html = mp.Parse(document.Markdown.Markdown);

                            var markdownFilePath = parentFolder + "\\" + saveDialog.SafeFileName + ".md";
                            var htmlFilePath = parentFolder + "\\" + saveDialog.SafeFileName + ".html";
                            var metadataFilePath = parentFolder + "\\" + saveDialog.SafeFileName + ".xml";
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
                            document.Metadata.FileName = saveDialog.SafeFileName;
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
            else
            {
                
            }
            
            return null;
        }
    }
}
