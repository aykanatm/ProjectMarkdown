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
    public class TableInserterViewModel : INotifyPropertyChanged, IRequireViewIdentification
    {
        private string _numberOfRows;
        private string _numberOfColumns;
        public Guid ViewID { get; }

        public string NumberOfRows
        {
            get { return _numberOfRows; }
            set
            {
                _numberOfRows = value;
                OnPropertyChanged();
            }
        }

        public string NumberOfColumns
        {
            get { return _numberOfColumns; }
            set
            {
                _numberOfColumns = value;
                OnPropertyChanged();
            }
        }

        public ICommand InsertTableCommand { get; set; }

        public TableInserterViewModel()
        {
            Logger.GetInstance().Debug("TableInserterViewModel() >>");

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

            Logger.GetInstance().Debug("<< TableInserterViewModel() >>");
        }

        private void LoadCommands()
        {
            Logger.GetInstance().Debug("LoadCommands() >>");

            try
            {
                InsertTableCommand = new RelayCommand(InsertTable, CanInsertTable);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while loading the commands", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< LoadCommands()");
        }

        public void InsertTable(object obj)
        {
            Logger.GetInstance().Debug("InsertTable() >>");
            try
            {
                int rows;
                int columns;

                var rowResult = int.TryParse(NumberOfRows, out rows);
                var columnResult = int.TryParse(NumberOfColumns, out columns);

                if (rowResult && columnResult)
                {
                    SharedEventHandler.GetInstance().RaiseOnInsertTableDimensionsSelected(rows, columns);
                }
                else
                {
                    throw new Exception("Rows and/or Columns cannot be converter into integer.");
                }

                WindowManager.GetInstance().CloseWindow(ViewID);
            }
            catch (Exception e)
            {
                Logger.GetInstance().Error(e.ToString());
                MessageBox.Show(e.Message, "An error occured while generating the dimensions of the table", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Logger.GetInstance().Debug("<< InsertTable()");
        }

        public bool CanInsertTable(object obj)
        {
            if (!string.IsNullOrEmpty(NumberOfRows) && !string.IsNullOrEmpty(NumberOfColumns))
            {
                return true;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
