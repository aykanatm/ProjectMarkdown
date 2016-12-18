using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectMarkdown.Services
{
    public class DocumentPrinter
    {
        public static void Print(string tempFilePath)
        {
            //var diaglog  = new PrintDialog();
            //var result = diaglog.ShowDialog();
            //if (result != null)
            //{
            //    if (result == true)
            //    {
            //        var printProcess = new Process();
            //        printProcess.StartInfo.FileName = tempFilePath;
            //        printProcess.StartInfo.Arguments = diaglog.PrintQueue.Name;
            //        printProcess.StartInfo.Verb = "Print";
            //        printProcess.StartInfo.CreateNoWindow = false;
            //        printProcess.Start();
            //    }
            //}
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
            info.FileName = tempFilePath;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();

            p.WaitForInputIdle();
            Thread.Sleep(3000);
            if (false == p.CloseMainWindow())
                p.Kill();
        }
    }
}
