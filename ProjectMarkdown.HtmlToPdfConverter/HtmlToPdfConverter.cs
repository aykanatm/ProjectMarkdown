using System;
using System.IO;
using Pechkin;

namespace ProjectMarkdown.HtmlToPdfConverter
{
    public class HtmlToPdfConverter
    {
        public void Convert(string htmlString, string filePath)
        {
            try
            {
                byte[] pdfBuf = new SimplePechkin(new GlobalConfig()).Convert(htmlString);
                File.WriteAllBytes(filePath, pdfBuf);
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while converting HTML to PDF. " + e.Message);
            }
        }
    }
}
