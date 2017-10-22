using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using LogUtils;
using PdfiumViewer;
using PrintDialog = System.Windows.Forms.PrintDialog;

namespace ProjectMarkdown.Services
{
    public static class DocumentPrinter
    {
        public static void Print(string tempFilePath)
        {
            Logger.GetInstance().Debug("Print() >>");

            var pdfDocument = PdfDocument.Load(tempFilePath);
            try
            {
                var printDialog = new PrintDialog
                {
                    AllowPrintToFile = true,
                    AllowSomePages = true,
                    PrinterSettings =
                    {
                        MinimumPage = 1,
                        MaximumPage = pdfDocument.PageCount,
                        FromPage = 1,
                        ToPage = pdfDocument.PageCount
                    }
                };

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    var pageSettings = new PageSettings {Margins = new Margins(0, 0, 0, 0)};

                    var printDocument = pdfDocument.CreatePrintDocument();
                    printDocument.PrinterSettings = printDialog.PrinterSettings;
                    printDocument.DefaultPageSettings = pageSettings;
                    printDocument.PrintController = new StandardPrintController();
                    printDocument.Print();
                }

                
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                pdfDocument.Dispose();
            }

            Logger.GetInstance().Debug("<< Print()");
        }
    }
}
