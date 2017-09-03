using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using LogUtils;
using ProjectMarkdown.Annotations;
using ProjectMarkdown.Windows;

namespace ProjectMarkdown.ViewModels
{
    public class AboutViewModel : INotifyPropertyChanged, IRequireViewIdentification
    {
        private readonly string _licenseFilePath = AppDomain.CurrentDomain.BaseDirectory + "License.txt";
        private string _currentVersion;
        private string _licenseText;

        public Guid ViewID { get; }

        public string CurrentVersion
        {
            get { return _currentVersion; }
            set
            {
                _currentVersion = value;
                OnPropertyChanged();
            }
        }

        public string LicenseText
        {
            get { return _licenseText; }
            set
            {
                _licenseText = value;
                OnPropertyChanged();
            }
        }

        public AboutViewModel()
        {
            Logger.GetInstance().Debug("AboutViewModel() >>");
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            ViewID = Guid.NewGuid();
            
            CurrentVersion = "Project Markdown v1.0.0";

            LicenseText = ReadLicense();

            Logger.GetInstance().Debug("<< AboutViewModel()");
        }

        private string ReadLicense()
        {
            Logger.GetInstance().Debug("ReadLicense() >>");

            var license = "Unable to read license file " + _licenseFilePath;

            try
            {
                using (var sr = new StreamReader(_licenseFilePath))
                {
                    license = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Unable to read license file", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< ReadLicense()");
            return license;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
