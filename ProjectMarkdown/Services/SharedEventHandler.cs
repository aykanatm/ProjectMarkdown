using System;
using LogUtils;
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
            Logger.GetInstance().Debug("RaiseOnInsertTableDimensionsSelected() >>");
            OnInsertTableDimensionsSelected(rows, columns);
            Logger.GetInstance().Debug("<< RaiseOnInsertTableDimensionsSelected()");
        }

        public void RaiseOnApplyLinkUrlSelected(string url)
        {
            Logger.GetInstance().Debug("RaiseOnApplyLinkUrlSelected() >>");
            OnApplyLinkUrlSelected(url);
            Logger.GetInstance().Debug("<< RaiseOnApplyLinkUrlSelected()");
        }

        public void RaiseOnInsertImageUrlSelected(string url, string alt)
        {
            Logger.GetInstance().Debug("RaiseOnInsertImageUrlSelected() >>");
            OnInsertImageUrlSelected(url, alt);
            Logger.GetInstance().Debug("<< RaiseOnInsertImageUrlSelected()");
        }

        public void RaiseOnPreferencesSaved(PreferencesModel preferences)
        {
            Logger.GetInstance().Debug("RaiseOnPreferencesSaved() >>");
            OnPreferecesSaved(preferences);
            Logger.GetInstance().Debug("<< RaiseOnPreferencesSaved()");
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
