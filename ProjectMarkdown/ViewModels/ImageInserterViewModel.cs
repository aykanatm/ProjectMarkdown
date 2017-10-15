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
    public class ImageInserterViewModel:INotifyPropertyChanged, IRequireViewIdentification
    {
        private string _selectedImageUrl;
        private string _selectedAlternateText;
        public Guid ViewID { get; }

        public string SelectedAlternateText
        {
            get { return _selectedAlternateText; }
            set
            {
                _selectedAlternateText = value;
                OnPropertyChanged();
            }
        }

        public string SelectedImageUrl
        {
            get { return _selectedImageUrl; }
            set
            {
                _selectedImageUrl = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectImageUrlCommand { get; set; }

        public ImageInserterViewModel()
        {
            Logger.GetInstance().Debug("ImageInserterViewModel() >>");

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

            Logger.GetInstance().Debug("<< ImageInserterViewModel()");
        }

        private void LoadCommands()
        {
            Logger.GetInstance().Debug("LoadCommands() >>");

            try
            {
                SelectImageUrlCommand = new RelayCommand(SelectImageUrl, CanSelectImageUrl);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while loading the commands", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Logger.GetInstance().Debug("<< LoadCommands()");
        }

        public void SelectImageUrl(object obj)
        {
            Logger.GetInstance().Debug("SelectImageUrl() >>");

            SharedEventHandler.GetInstance().RaiseOnInsertImageUrlSelected(SelectedImageUrl, SelectedAlternateText);
            WindowManager.GetInstance().CloseWindow(ViewID);

            Logger.GetInstance().Debug("<< SelectImageUrl()");
        }

        public bool CanSelectImageUrl(object obj)
        {
            if (string.IsNullOrEmpty(SelectedImageUrl))
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
