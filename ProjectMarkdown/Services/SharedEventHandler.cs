using System;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public class SharedEventHandler
    {
        private static SharedEventHandler _sharedEventHandler;

        private static readonly object LockObject = new object();

        public Action<PreferencesModel> OnPreferecesSaved;

        public static SharedEventHandler GetInstance()
        {
            if (_sharedEventHandler == null)
            {
                lock (LockObject)
                {
                    _sharedEventHandler = new SharedEventHandler();
                }
            }

            return _sharedEventHandler;
        }

        private SharedEventHandler() { }

        public void RaiseOnPreferencesSaved(PreferencesModel preferences)
        {
            OnPreferecesSaved(preferences);
        }
    }
}
