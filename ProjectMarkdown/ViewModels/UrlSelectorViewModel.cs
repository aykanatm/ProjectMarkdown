using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using LogUtils;
using ProjectMarkdown.Annotations;
using ProjectMarkdown.Services;
using ProjectMarkdown.Windows;

namespace ProjectMarkdown.ViewModels
{
    public class UrlSelectorViewModel : INotifyPropertyChanged, IRequireViewIdentification
    {
        private string _selectedUrl;

        public Guid ViewID { get; }
        public string SelectedUrl
        {
            get { return _selectedUrl; }
            set
            {
                _selectedUrl = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectUrlCommand { get; set; }

        public UrlSelectorViewModel()
        {
            Logger.GetInstance().Debug("UrlSelectorViewModel() >>");

            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    return;
                }

                ViewID = Guid.NewGuid();
                LoadCommands();
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while initializing the window", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< UrlSelectorViewModel()");
        }

        private void LoadCommands()
        {
            Logger.GetInstance().Debug("LoadCommands() >>");

            try
            {
                SelectUrlCommand = new RelayCommand(SelectUrl, CanSelectUrl);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while loading the commands", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< LoadCommands()");
        }

        public void SelectUrl(object obj)
        {
            Logger.GetInstance().Debug("SelectUrl() >>");

            try
            {
                SharedEventHandler.GetInstance().RaiseOnApplyLinkUrlSelected(SelectedUrl);
                WindowManager.GetInstance().CloseWindow(ViewID);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while selecting the URL", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< SelectUrl()");
        }

        public bool CanSelectUrl(object obj)
        {
            if (string.IsNullOrEmpty(SelectedUrl))
            {
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
