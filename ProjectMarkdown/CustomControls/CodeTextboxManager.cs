using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Dragablz;
using LogUtils;
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
            try
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public void InsertText(DocumentModel document, string text)
        {
            Logger.GetInstance().Debug("InsertText() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.InsertText(text);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< InsertText()");
        }

        public string GetSelectedText(DocumentModel document)
        {
            Logger.GetInstance().Debug("GetSelectedText() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    Logger.GetInstance().Debug("<< GetSelectedText()");
                    return codeTextboxHost.GetSelectedText();
                }
                Logger.GetInstance().Debug("<< GetSelectedText()");
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ReplaceText(DocumentModel document, string text)
        {
            Logger.GetInstance().Debug("ReplaceText() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.ReplaceText(text);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< ReplaceText()");
        }

        public void Undo(DocumentModel document)
        {
            Logger.GetInstance().Debug("Undo() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.Undo();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("Undo() >>");
        }

        public void Redo(DocumentModel document)
        {
            Logger.GetInstance().Debug("Redo() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.Redo();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< Redo()");
        }

        public void Cut(DocumentModel document)
        {
            Logger.GetInstance().Debug("Cut() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.Cut();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< Cut()");
        }

        public void Copy(DocumentModel document)
        {
            Logger.GetInstance().Debug("Copy() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.Copy();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< Copy()");
        }

        public void Paste(DocumentModel document)
        {
            Logger.GetInstance().Debug("Paste() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.Paste();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("Paste() >>");
        }

        public void Delete(DocumentModel document)
        {
            Logger.GetInstance().Debug("Delete() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.Delete();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("Delete() >>");
        }

        public void SelectAll(DocumentModel document)
        {
            Logger.GetInstance().Debug("SelectAll() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.SelectAll();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< SelectAll()");
        }

        public void ShowFindDialog(DocumentModel document)
        {
            Logger.GetInstance().Debug("ShowFindDialog() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.ShowFindDialog();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< ShowFindDialog()");
        }

        public void ShowReplaceDialog(DocumentModel document)
        {
            Logger.GetInstance().Debug("ShowReplaceDialog() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.ShowReplaceDialog();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< ShowReplaceDialog()");
        }

        public void ClearUndoRedo(DocumentModel document)
        {
            Logger.GetInstance().Debug("ClearUndoRedo() >>");

            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.ClearUndoRedo();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            Logger.GetInstance().Debug("<< ClearUndoRedo()");
        }

        public bool HasSelectedText(DocumentModel document)
        {
            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    return codeTextboxHost.HasSelectedText();
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void RefreshScrollPosition(DocumentModel document)
        {
            Logger.GetInstance().Debug("RefreshScrollPosition() >>");
            try
            {
                var codeTextboxHost = GetCurrentCodeTextbox(document);

                if (codeTextboxHost != null)
                {
                    codeTextboxHost.RefreshScrollPosition();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            Logger.GetInstance().Debug("<< RefreshScrollPosition()");
        }

        private CodeTextboxHost GetCurrentCodeTextbox(DocumentModel document)
        {
            try
            {
                CodeTextboxHost codeTextbox = null;

                if (Application.Current.MainWindow != null)
                {
                    var tabControl = (TabablzControl)Application.Current.MainWindow.FindName("TabDocuments");

                    if (tabControl != null)
                    {
                        var codeTextboxes = tabControl.GetVisualChildren<CodeTextboxHost>();
                        var codeTextboxHosts = codeTextboxes as IList<CodeTextboxHost> ?? codeTextboxes.ToList();
                        if (codeTextboxHosts.Any())
                        {
                            foreach (var codeTextboxHost in codeTextboxHosts)
                            {
                                if (((ContentPresenter)codeTextboxHost.TemplatedParent).Content == document)
                                {
                                    codeTextbox = codeTextboxHost;
                                }
                            }
                        }
                    }
                }
                
                return codeTextbox;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private CodeTextboxManager() { }
    }
}