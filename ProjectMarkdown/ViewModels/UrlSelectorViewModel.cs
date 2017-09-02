using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            ViewID = Guid.NewGuid();
            LoadCommands();
        }

        private void LoadCommands()
        {
            SelectUrlCommand = new RelayCommand(SelectUrl, CanSelectUrl);
        }

        public void SelectUrl(object obj)
        {
            Logger.GetInstance().Debug("SelectUrl() >>");

            SharedEventHandler.GetInstance().RaiseOnApplyLinkUrlSelected(SelectedUrl);
            WindowManager.GetInstance().CloseWindow(ViewID);

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
