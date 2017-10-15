using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LogUtils;
using ProjectMarkdown.Services;
using ProjectMarkdown.Views;
using WPFUtils.ExtensionMethods;

namespace ProjectMarkdown.Windows
{
    public class WindowManager
    {
        private static WindowManager _windowManager;

        private static double _totalToolbarWidth = 0;
        public int ToolbarTrayHeightDoubleBands { get; private set; }
        public int ToolbarTrayHeightSingleBand { get; private set; }
        public bool IsSingleBand { get; private set; }

        private static readonly object LockObject = new object();

        public enum WindowTypes
        {
            About,
            Preferences,
            UrlSelector,
            ImageInserter,
            TableInserter
        }

        public static WindowManager GetInstance()
        {
            if (_windowManager == null)
            {
                lock (LockObject)
                {
                    _windowManager = new WindowManager();
                }
            }

            return _windowManager;
        }

        public void OpenWindow(WindowTypes windowType)
        {
            Logger.GetInstance().Debug("OpenWindow() >>");

            try
            {
                if (windowType == WindowTypes.About)
                {
                    var aboutWindow = new About();
                    aboutWindow.Show();
                }
                else if (windowType == WindowTypes.Preferences)
                {
                    var preferencesWindow = new Preferences();
                    preferencesWindow.Show();
                }
                else if (windowType == WindowTypes.UrlSelector)
                {
                    var urlSelectorWindow = new UrlSelector();
                    urlSelectorWindow.Show();
                }
                else if (windowType == WindowTypes.ImageInserter)
                {
                    var imageInserterWindow = new ImageInserter();
                    imageInserterWindow.Show();
                }
                else if (windowType == WindowTypes.TableInserter)
                {
                    var tableInserterWindow = new TableInserter();
                    tableInserterWindow.Show();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("OpenWindow() >>");
        }

        public void CloseWindow(Guid id)
        {
            Logger.GetInstance().Debug("CloseWindow() >>");

            try
            {
                foreach (Window window in Application.Current.Windows)
                {
                    var w_id = window.DataContext as IRequireViewIdentification;
                    if (w_id != null && w_id.ViewID.Equals(id))
                    {
                        window.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< CloseWindow()");
        }

        public void RefreshToolbarPositions(Guid id)
        {
            try
            {
                var window = GetWindow(id);
                var toolbars = GetControls<ToolBar>(id);

                var dependencyObjects = toolbars as IList<DependencyObject> ?? toolbars.ToList();

                if (_totalToolbarWidth <= 0)
                {
                    foreach (var dependencyObject in dependencyObjects)
                    {
                        var toolbar = (ToolBar)dependencyObject;
                        _totalToolbarWidth += toolbar.ActualWidth;
                    }
                }


                if (window.ActualWidth > _totalToolbarWidth)
                {
                    foreach (var dependencyObject in dependencyObjects)
                    {
                        var toolbar = (ToolBar)dependencyObject;
                        toolbar.Band = 0;
                    }

                    IsSingleBand = true;
                }
                else
                {
                    foreach (var dependencyObject in dependencyObjects)
                    {
                        var toolbar = (ToolBar)dependencyObject;
                        if (toolbar.Name == "FormattingToolbar")
                        {
                            toolbar.Band = 1;
                        }
                        else
                        {
                            toolbar.Band = 0;
                        }
                    }

                    IsSingleBand = false;
                }

                // This ensures that the toolbar tray is resized to correct height after window resize
                SharedEventHandler.GetInstance().RaiseOnToolbarPositionsChanged();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private IEnumerable<DependencyObject> GetControls<T>(Guid id) where T : DependencyObject
        {
            try
            {
                var window = GetWindow(id);
                var children = window.GetChildren<T>();

                return children;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Window GetWindow(Guid id)
        {
            try
            {
                foreach (Window window in Application.Current.Windows)
                {
                    var w_id = window.DataContext as IRequireViewIdentification;
                    if (w_id != null && w_id.ViewID.Equals(id))
                    {
                        return window;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private WindowManager()
        {
            try
            {
                ToolbarTrayHeightDoubleBands = 70;
                ToolbarTrayHeightSingleBand = 35;
                IsSingleBand = false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
