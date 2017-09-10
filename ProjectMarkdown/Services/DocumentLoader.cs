using System;
using System.IO;
using System.IO.Compression;
using IOUtils;
using LogUtils;
using Microsoft.Win32;
using ProjectMarkdown.ExtensionMethods;
using ProjectMarkdown.Model;
using ProjectMarkdown.ViewModels;

namespace ProjectMarkdown.Services
{
    public static class DocumentLoader
    {
        public static DocumentModel Load(MainWindowViewModel mainWindowViewModel)
        {
            Logger.GetInstance().Debug("Load() >>");

            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Open a PMD file";
                openFileDialog.Filter = "Project Markdown File | *.pmd";
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
                                                currentHtml = zipSr.ReadToEnd().RemoveScripts();
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
                        var documentMarkdown = currentMarkdown;

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
                        var documentHtml = new Uri(tempSourceFilePath);

                        // Generate the model
                        var documentModel = new DocumentModel(mainWindowViewModel, documentMetadata.FileName)
                        {
                            Metadata = documentMetadata,
                            Html = documentHtml,
                            Markdown = documentMarkdown
                        };

                        Logger.GetInstance().Debug("<< Load()");
                        return documentModel;
                    }

                    Logger.GetInstance().Debug("<< Load()");
                    return null;
                }
                throw new Exception("OpenFileDialog returned null!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
