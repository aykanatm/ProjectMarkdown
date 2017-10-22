using System;
using System.IO;
using TuesPechkin;

namespace ProjectMarkdown.HtmlToPdfConverter
{
    public class HtmlToPdfConverter
    {
        public void Convert(string htmlString, string filePath)
        {
            try
            {
                IConverter converter = new StandardConverter(new PdfToolset(new WinAnyCPUEmbeddedDeployment(new TempFolderDeployment())));

                var document = new HtmlToPdfDocument
                {
                    GlobalSettings =
                    {
                        ProduceOutline = true
                    },
                    Objects =
                    {
                        new ObjectSettings
                        {
                            HtmlText = htmlString
                        }
                    }
                };
                
                byte[] bytes = converter.Convert(document);
                File.WriteAllBytes(filePath, bytes);
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while converting HTML to PDF. " + e.Message);
            }
        }
    }
}
