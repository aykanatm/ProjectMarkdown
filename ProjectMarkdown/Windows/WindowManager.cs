using System;
using System.Windows;

namespace ProjectMarkdown.Windows
{
    public class WindowManager
    {
        private static WindowManager _windowManager;

        private static readonly object LockObject = new object();

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
