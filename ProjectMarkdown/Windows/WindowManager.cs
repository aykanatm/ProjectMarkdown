using System;
using System.Windows;
using ProjectMarkdown.Views;

namespace ProjectMarkdown.Windows
{
    public class WindowManager
    {
        private static WindowManager _windowManager;

        private static readonly object LockObject = new object();

        public enum WindowTypes
        {
            About,
            Preferences,
            UrlSelector,
            ImageInserter
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
        }

        public void CloseWindow(Guid id)
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

        private WindowManager() { }
    }
}
