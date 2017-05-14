using System;
using System.Windows.Forms;
using LogUtils;
using Spire.Pdf;
using PrintDialog = System.Windows.Forms.PrintDialog;

namespace ProjectMarkdown.Services
{
    public static class DocumentPrinter
    {
        public static void Print(string tempFilePath)
        {
            Logger.GetInstance().Debug("Print() >>");

            try
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
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< Print()");
        }
    }
}
