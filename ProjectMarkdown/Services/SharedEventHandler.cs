using System;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public class SharedEventHandler
    {
        private static SharedEventHandler _sharedEventHandler;

        private static readonly object LockObject = new object();

        public Action<PreferencesModel> OnPreferecesSaved;
        public Action<ScrollResult> OnCodeTextboxScrollChanged;
        public Action OnTextboxTextChanged;

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

        public void RaiseOnCodeTextboxScrollChanged(ScrollResult scrollResult)
        {
            OnCodeTextboxScrollChanged(scrollResult);
        }

        public void RaiseOnPreferencesSaved(PreferencesModel preferences)
        {
            OnPreferecesSaved(preferences);
        }

        public void RaiseOnTextboxTextChanged()
        {
            OnTextboxTextChanged();
        }
    }
}
