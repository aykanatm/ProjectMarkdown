using System.Windows;
using System.Windows.Controls;
using ProjectMarkdown.Model;
using WPFUtils.ExtensionMethods;

namespace ProjectMarkdown.CustomControls
{
    public class CodeTextboxManager
    {
        private static CodeTextboxManager _codeTextboxManager;

        private static readonly object LockObject = new object();

        public static CodeTextboxManager GetInstance()
        {
            if (_codeTextboxManager == null)
            {
                lock (LockObject)
                {
                    if (_codeTextboxManager == null)
                    {
                        _codeTextboxManager = new CodeTextboxManager();
                    }
                }
            }

            return _codeTextboxManager;
        }

        public void Undo(DocumentModel document)
        {
            var codeTextboxHost = GetCurrentCodeTextbox(document);

            if (codeTextboxHost != null)
            {
                codeTextboxHost.Undo();
            }
        }

        public void Redo(DocumentModel document)
        {
            var codeTextboxHost = GetCurrentCodeTextbox(document);

            if (codeTextboxHost != null)
            {
                codeTextboxHost.Redo();
            }
        }

        public void ClearUndoRedo(DocumentModel document)
        {
            var codeTextboxHost = GetCurrentCodeTextbox(document);

            if (codeTextboxHost != null)
            {
                codeTextboxHost.ClearUndoRedo();
            }
        }

        private CodeTextboxHost GetCurrentCodeTextbox(DocumentModel document)
        {
            CodeTextboxHost codeTextbox = null;

            var tabControl = (TabControl)Application.Current.MainWindow.FindName("TabDocuments");

            if (tabControl != null)
            {
                var contentPresenters = tabControl.GetVisualChildren<ContentPresenter>();

                foreach (var contentPresenter in contentPresenters)
                {
                    var parent = contentPresenter.TemplatedParent;
                    if (parent.Equals(tabControl))
                    {
                        if (tabControl.SelectedItem == document)
                        {
                            codeTextbox = (CodeTextboxHost)tabControl.ContentTemplate.FindName("CodeTextboxHost", contentPresenter);
                        }
                    }
                }
            }

            return codeTextbox;
        }

        private CodeTextboxManager() { }
    }
}
