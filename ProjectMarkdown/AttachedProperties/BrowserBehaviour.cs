using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProjectMarkdown.AttachedProperties
{
    public static class BrowserBehaviour
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached("Html",
                                                                                                    typeof(string),
                                                                                                    typeof(BrowserBehaviour),
                                                                                                    new FrameworkPropertyMetadata(OnHtmlChanged));

        public static string GetHtml(WebBrowser webBrowser)
        {
            return (string) webBrowser.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser webBrowser, string value)
        {
            webBrowser.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wb = d as WebBrowser;
            if (wb != null)
            {
                wb.NavigateToString(e.NewValue as string);
            }
        }
    }
}
