using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pechkin;

namespace ProjectMarkdown.HtmlToPdfConverter
{
    public class HtmlToPdfConverter
    {
        public void Convert(string htmlString, string filePath)
        {
            byte[] pdfBuf = new SimplePechkin(new GlobalConfig()).Convert(htmlString);
            File.WriteAllBytes(filePath, pdfBuf);
        }
    }
}
