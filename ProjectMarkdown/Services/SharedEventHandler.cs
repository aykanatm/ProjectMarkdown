using System;
using ProjectMarkdown.Model;

namespace ProjectMarkdown.Services
{
    public class SharedEventHandler
    {
        private static SharedEventHandler _sharedEventHandler;

        private static readonly object LockObject = new object();

        public Action<PreferencesModel> OnPreferecesSaved;
        public Action<string> OnApplyLinkUrlSelected;
        public Action<string, string> OnInsertImageUrlSelected;
        public Action<int, int> OnInsertTableDimensionsSelected;
        public Action<ScrollResult> OnCodeTextboxScrollChanged;
        public Action OnTextboxTextChanged;
        public Action OnToolbarPositionsChanged;

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

        public void RaiseOnInsertTableDimensionsSelected(int rows, int columns)
        {
            OnInsertTableDimensionsSelected(rows, columns);
        }

        public void RaiseOnApplyLinkUrlSelected(string url)
        {
            OnApplyLinkUrlSelected(url);
        }

        public void RaiseOnInsertImageUrlSelected(string url, string alt)
        {
            OnInsertImageUrlSelected(url, alt);
        }

        public void RaiseOnPreferencesSaved(PreferencesModel preferences)
        {
            OnPreferecesSaved(preferences);
        }

        public void RaiseOnCodeTextboxScrollChanged(ScrollResult scrollResult)
        {
            OnCodeTextboxScrollChanged(scrollResult);
        }

        public void RaiseOnTextboxTextChanged()
        {
            OnTextboxTextChanged();
        }

        public void RaiseOnToolbarPositionsChanged()
        {
            OnToolbarPositionsChanged();
        }
    }
}
