﻿using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using IOUtils;
using LogUtils;
using Microsoft.Win32;
using ProjectMarkdown.Annotations;
using ProjectMarkdown.Model;
using ProjectMarkdown.Services;
using ProjectMarkdown.Statics;

namespace ProjectMarkdown.ViewModels
{
    public class PreferencesViewModel : INotifyPropertyChanged
    {
        private PreferencesModel _currentPreferences;

        public PreferencesModel CurrentPreferences
        {
            get { return _currentPreferences; }
            set
            {
                _currentPreferences = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectLogFileCommand { get; set; }
        public ICommand CancelPreferenceChangesCommand { get; set; }
        public ICommand SavePreferencesCommand { get; set; }

        public PreferencesViewModel()
        {
            Logger.GetInstance().Debug("PreferencesViewModel() >>");

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            LoadCommands();

            if (File.Exists(FilePaths.PreferencesFilePath))
            {
                var gxs = new GenericXmlSerializer<PreferencesModel>();
                CurrentPreferences = gxs.DeSerialize(FilePaths.PreferencesFilePath);
            }
            else
            {
                throw new Exception(FilePaths.PreferencesFilePath + " file does not exist!");
            }

            Logger.GetInstance().Debug("<< PreferencesViewModel()");
        }

        private void LoadCommands()
        {
            Logger.GetInstance().Debug("LoadCommands() >>");

            SelectLogFileCommand = new RelayCommand(SelectLogFile, CanSelectLogFile);
            CancelPreferenceChangesCommand = new RelayCommand(CancelPreferences, CanCancelPreferences);
            SavePreferencesCommand = new RelayCommand(SavePreferences, CanSavePreferences);

            Logger.GetInstance().Debug("<< LoadCommands()");
        }

        public bool CanSelectLogFile(object obj)
        {
            return true;
        }

        public void SelectLogFile(object obj)
        {
            Logger.GetInstance().Debug("SelectLogFile() >>");

            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.CreatePrompt = true;
                saveFileDialog.Title = "Select a LOG file";
                saveFileDialog.Filter = "LOG file | *.log";
                var result = saveFileDialog.ShowDialog();

                if (result != null)
                {
                    if (result == true)
                    {
                        CurrentPreferences.LogFilePath = saveFileDialog.FileName;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while selecting the LOG file", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< SelectLogFile()");
        }

        public bool CanCancelPreferences(object obj)
        {
            return true;
        }

        public void CancelPreferences(object obj)
        {
            Logger.GetInstance().Debug("CancelPreferences() >>");

            try
            {
                var window = (Window)obj;
                window.Close();
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while closing the preferences window", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< CancelPreferences()");
        }

        public bool CanSavePreferences(object obj)
        {
            return true;
        }

        public void SavePreferences(object obj)
        {
            Logger.GetInstance().Debug("SavePreferences() >>");

            try
            {
                var gxs = new GenericXmlSerializer<PreferencesModel>();

                gxs.Serialize(CurrentPreferences, FilePaths.PreferencesFilePath);

                SharedEventHandler.GetInstance().RaiseOnPreferencesSaved(CurrentPreferences);

                var window = (Window)obj;
                window.Close();
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while saving preferences", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< SavePreferences()");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
