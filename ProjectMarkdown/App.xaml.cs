﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Windows;
using IOUtils;
using LogUtils;
using ProjectMarkdown.Model;
using ProjectMarkdown.Statics;

namespace ProjectMarkdown
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string StartupFilePath = "";

        private Mutex _mutex;

        public App()
        {
            bool isNew;
            _mutex = new Mutex(true, "{DFA6F1D9-7EC8-4557-AA0C-B14BF307AE77}", out isNew);

            var commandLineArgs = Environment.GetCommandLineArgs();
            ProcessCommandLineArgs(commandLineArgs);

            if (!isNew)
            {
                var writerThread = new Thread(WriterThread);
                writerThread.IsBackground = true;
                writerThread.Start();

                _mutex.ReleaseMutex();
                Current.Shutdown();
            }

            InitializeLogger();
            GenerateFolders();
            
            _mutex.ReleaseMutex();
        }

        private void WriterThread()
        {
            var client = new NamedPipeClientStream("{DFA6F1D9-7EC8-4557-AA0C-B14BF307AE77}");
            client.Connect(Timeout.Infinite);

            using (var writer = new BinaryWriter(client))
            {
                writer.Write(StartupFilePath);
            }
        }

        private void InitializeLogger()
        {
            try
            {
                if (File.Exists(FilePaths.PreferencesFilePath))
                {
                    var gxs = new GenericXmlSerializer<PreferencesModel>();
                    var preferences = gxs.DeSerialize(FilePaths.PreferencesFilePath);
                    var logLevel = preferences.CurrentLogLevel;
                    var logFilePath = preferences.LogFilePath;
                    var isLoggingEnabled = preferences.IsLoggingEnabled;

                    if (logLevel == "DEBUG")
                    {
                        Logger.Initialize(logFilePath, Logger.LogLevels.Debug, isLoggingEnabled);
                    }
                    else if (logLevel == "INFO")
                    {
                        Logger.Initialize(logFilePath, Logger.LogLevels.Info, isLoggingEnabled);
                    }
                    else if (logLevel == "ERROR")
                    {
                        Logger.Initialize(logFilePath, Logger.LogLevels.Error, isLoggingEnabled);
                    }
                }
                else
                {
                    Logger.Initialize(FilePaths.DefaultLogFilePath, Logger.LogLevels.Info, true);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "An error occured while initializing the logger", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateFolders()
        {
            try
            {
                if (!Directory.Exists(FolderPaths.PreferencesFolderPath))
                {
                    Directory.CreateDirectory(FolderPaths.PreferencesFolderPath);
                }

                if (!Directory.Exists(FolderPaths.DefaultLogFolderPath))
                {
                    Directory.CreateDirectory(FolderPaths.DefaultLogFolderPath);
                }

                if (!Directory.Exists(FolderPaths.TempFolderPath))
                {
                    Directory.CreateDirectory(FolderPaths.TempFolderPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Unable to generate application folders", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessCommandLineArgs(string[] commandLineArgs)
        {
            if (commandLineArgs.Any())
            {
                foreach (var arg in commandLineArgs)
                {
                    if (arg.EndsWith(".pmd"))
                    {
                        StartupFilePath = arg;
                        break;
                    }
                }
            }
        }
    }
}
