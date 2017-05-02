using System.Windows.Forms;
using Spire.Pdf;
using PrintDialog = System.Windows.Forms.PrintDialog;

namespace ProjectMarkdown.Services
{
    public class DocumentPrinter
    {
        public static void Print(string tempFilePath)
        {
            var pdfDocument = new PdfDocument();
            pdfDocument.LoadFromFile(tempFilePath);

            var printDialog = new PrintDialog
            {
                AllowPrintToFile = true,
                AllowSomePages = true,
                PrinterSettings =
                {
                    MinimumPage = 1,
                    MaximumPage = pdfDocument.Pages.Count,
                    FromPage = 1,
                    ToPage = pdfDocument.Pages.Count
                }
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pdfDocument.PrintFromPage = printDialog.PrinterSettings.FromPage;
                pdfDocument.PrintToPage = printDialog.PrinterSettings.ToPage;
                pdfDocument.PrinterName = printDialog.PrinterSettings.PrinterName;

                var printDocument = pdfDocument.PrintDocument;
                printDialog.Document = printDocument;
                printDocument.Print();
            }
        }
    }
}
