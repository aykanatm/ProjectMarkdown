using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using CefSharp.Wpf;
using Dragablz;
using MathUtils;
using ProjectMarkdown.CustomControls.CefHandlers;
using ProjectMarkdown.Model;
using ProjectMarkdown.Services;
using WPFUtils.ExtensionMethods;

namespace ProjectMarkdown.CustomControls
{
    public class CefChromiumBrowserManager
    {
        private static CefChromiumBrowserManager _cefChromiumBrowserManager;

        private static readonly object LockObject = new object();

        public static CefChromiumBrowserManager GetInstance()
        {
            try
            {
                if (_cefChromiumBrowserManager == null)
                {
                    lock (LockObject)
                    {
                        if (_cefChromiumBrowserManager == null)
                        {
                            _cefChromiumBrowserManager = new CefChromiumBrowserManager();
                        }
                    }
                }

                return _cefChromiumBrowserManager;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Scroll(DocumentModel document, ScrollResult scrollResult)
        {
            var browser = GetCurrentBrowser(document);

            var getScrollHeightScript = @"(function(){
                                                return document.getElementsByTagName('body')[0].scrollHeight - document.getElementsByTagName('body')[0].clientHeight;
                                           })();";

            browser.GetMainFrame().EvaluateScriptAsync(getScrollHeightScript).ContinueWith(x =>
            {
                var browserMinScrollPosition = 0;
                var browserMaxScrollPosition = 0;

                var response = x.Result;
                if (response.Success && response.Result != null)
                {
                    browserMaxScrollPosition = (int) response.Result;
                }

                var browserScrollPosition = GenericRescaler<int>.Rescale(scrollResult.Value, scrollResult.MinValue, scrollResult.MaxValue, browserMinScrollPosition, browserMaxScrollPosition);

                var setBrowserScrollPosition = "document.getElementsByTagName('body')[0].scrollTop = " + browserScrollPosition;

                browser.GetMainFrame().ExecuteJavaScriptAsync(setBrowserScrollPosition);
            });
        }

        public void SetCustomRequestHandler(DocumentModel document)
        {
            var browser = GetCurrentBrowser(document);
            browser.RequestHandler = new BrowserRequestHandler();
        }

        private ChromiumWebBrowser GetCurrentBrowser(DocumentModel document)
        {
            try
            {
                ChromiumWebBrowser cefChromiumWebBrowser = null;

                var tabControl = (TabablzControl)Application.Current.MainWindow.FindName("TabDocuments");

                if (tabControl != null)
                {
                    var browsers = tabControl.GetVisualChildren<ChromiumWebBrowser>();
                    var cefBrowsers = browsers as IList<ChromiumWebBrowser> ?? browsers.ToList();
                    if (cefBrowsers.Any())
                    {
                        foreach (var cefBrowser in cefBrowsers)
                        {
                            if (((ContentPresenter)cefBrowser.TemplatedParent).Content == document)
                            {
                                cefChromiumWebBrowser = cefBrowser;
                            }
                        }
                    }
                }

                return cefChromiumWebBrowser;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
