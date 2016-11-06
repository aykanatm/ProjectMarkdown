using System;
using System.IO;
using System.IO.Compression;
using CustomIO;
using Microsoft.Win32;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public class DocumentLoader
    {
        public static DocumentModel Load()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Markdown File | *.pmd";
            var result = openFileDialog.ShowDialog();

            if (result != null)
            {
                if (result == true)
                {
                    string currentMarkdown = "";
                    string currentHtml = "";
                    string currentXml = "";

                    using (var fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        using (var archive = new ZipArchive(fs))
                        {
                            foreach (var entry in archive.Entries)
                            {
                                if (entry.Name.EndsWith(".md"))
                                {
                                    using (var stream = entry.Open())
                                    {
                                        using (var zipSr = new StreamReader(stream))
                                        {
                                            currentMarkdown = zipSr.ReadToEnd();
                                        }
                                    }
                                }
                                if (entry.Name.EndsWith(".html"))
                                {
                                    using (var stream = entry.Open())
                                    {
                                        using (var zipSr = new StreamReader(stream))
                                        {
                                            currentHtml = zipSr.ReadToEnd();
                                        }
                                    }
                                }
                                if (entry.Name.EndsWith(".xml"))
                                {
                                    using (var stream = entry.Open())
                                    {
                                        using (var zipSr = new StreamReader(stream))
                                        {
                                            currentXml = zipSr.ReadToEnd();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Get metadata
                    var gxs = new GenericXmlSerializer<DocumentMetadata>();
                    var documentMetadata = gxs.DeSerializeFromString(currentXml);

                    // Get markdown text
                    var documentMarkdown = new DocumentMarkdown(currentMarkdown);

                    // Get source URI
                    var tempFolderPath = AppDomain.CurrentDomain.BaseDirectory + "Temp";
                    if (!Directory.Exists(tempFolderPath))
                    {
                        Directory.CreateDirectory(tempFolderPath);
                    }
                    var tempSourceFilePath = tempFolderPath + "\\tempsource.html";
                    using (var sw = new StreamWriter(tempSourceFilePath))
                    {
                        sw.Write(currentHtml);
                    }
                    var documentHtml = new DocumentHtml(new Uri(tempSourceFilePath));

                    // Generate the model
                    var documentModel = new DocumentModel(documentMetadata.FileName)
                    {
                        Metadata = documentMetadata,
                        Html = documentHtml,
                        Markdown = documentMarkdown
                    };
                    return documentModel;
                }
                return null;
            }
            return null;
        }
    }
}
