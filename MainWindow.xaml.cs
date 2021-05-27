using ClassLibrary;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace labworkUI_2 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private V2MainCollection MainCollection;
        private DataItemView DataItem;

        public MainWindow() {
            MainCollection = new V2MainCollection();
            DataItem = new DataItemView();
            InitializeComponent();
            Resources["MainCollection"] = MainCollection;
            Resources["DataItem"] = DataItem;
        }

        #region Clicks
        private void NewClick(object sender, RoutedEventArgs e) {
            if (MainCollection.IsSave || SaveDialog()) {
                MainCollection = new V2MainCollection();
                Resources["MainCollection"] = MainCollection;
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e) {
            if (MainCollection.IsSave || SaveDialog()) {
                OpenFileDialog OpenDialog = new OpenFileDialog {
                    Filter = "Binary data|*.dat|All|*.*",
                    FilterIndex = 1
                };

                if (OpenDialog.ShowDialog() == true) {
                    MainCollection = new V2MainCollection();
                    MainCollection.Load(OpenDialog.FileName);
                    Resources["MainCollection"] = MainCollection;
                }
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e) {
            SaveFileDialog SaveDialog = new SaveFileDialog {
                Filter = "Binary data|*.dat|All|*.*",
                FilterIndex = 1
            };

            if (SaveDialog.ShowDialog() == true) {
                MainCollection.Save(SaveDialog.FileName);
                MainCollection.IsSave = true;
            }
        }

        private void AddDefaultsClick(object sender, RoutedEventArgs e) {
            MainCollection.AddDefaults();
            MainCollection.IsSave = false;
        }

        private void AddDefaultV2DataCollectionClick(object sender, RoutedEventArgs e) {
            V2DataCollection data_collection = new V2DataCollection(0.0, "Default info");
            data_collection.InitRandom(3, 10.0f, 10.0f, -10.0f, 10.0f);
            MainCollection.Add(data_collection);
            MainCollection.IsSave = false;
        }

        private void AddDefaultV2DataOnGridClick(object sender, RoutedEventArgs e) {
            V2DataOnGrid data_on_grid = new V2DataOnGrid(0.0, "Default info", new double[] { 0.01, 0.01 }, new int[] { 3, 3 });
            data_on_grid.InitRandom(-10.0f, 10.0f);
            MainCollection.Add(data_on_grid);
            MainCollection.IsSave = false;
        }

        private void AddElementFromFileClick(object sender, RoutedEventArgs e) {
            OpenFileDialog OpenDialog = new OpenFileDialog {
                Filter = "All|*.*",
                FilterIndex = 0
            };

            if (OpenDialog.ShowDialog() == true) {
                V2DataCollection data_collection = new V2DataCollection(OpenDialog.FileName);
                MainCollection.Add(data_collection);
                MainCollection.IsSave = false;
            }
        }

        private void AddDataItemClick(object sender, RoutedEventArgs e) {
            if (DataItem.IsValid() && listBox_DataCollection != null && listBox_DataCollection.SelectedItem != null
                && (listBox_DataCollection.SelectedItem as V2DataCollection).CheckNewData(DataItem.CoordField) && Complex.Abs(DataItem.ValueField) > 0.0) {
                (listBox_DataCollection.SelectedItem as V2DataCollection).Add(DataItem.DataItem);
                MainCollection.UpdateData();
            } else {
                MessageBox.Show(
                    "Невозможно добавить DataItem. Не выполнены необходимые условия",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void RemoveClick(object sender, RoutedEventArgs e) {
            if ((sender as MenuItem).DataContext != null) {
                var dc = (V2Data)(sender as MenuItem).DataContext;
                var id = dc.Description;
                var w = dc.Freq_field;
                MainCollection.Remove(id, w);
                MainCollection.IsSave = false;
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!MainCollection.IsSave) {
                e.Cancel = !SaveDialog();
            }
        }
        #endregion

        #region Commands
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e) {
            if (MainCollection.IsSave || SaveDialog()) {
                OpenFileDialog OpenDialog = new OpenFileDialog {
                    Filter = "Binary data|*.dat|All|*.*",
                    FilterIndex = 1
                };

                if (OpenDialog.ShowDialog() == true) {
                    MainCollection = new V2MainCollection();
                    MainCollection.Load(OpenDialog.FileName);
                    Resources["MainCollection"] = MainCollection;
                }
            }
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e) {
            SaveFileDialog SaveDialog = new SaveFileDialog {
                Filter = "Binary data|*.dat|All|*.*",
                FilterIndex = 1
            };

            if (SaveDialog.ShowDialog() == true) {
                MainCollection.Save(SaveDialog.FileName);
                MainCollection.IsSave = true;
            }
        }

        private void SaveCanExecuted(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = !MainCollection.IsSave;
        }

        private void AddDataItemExecuted(object sender, ExecutedRoutedEventArgs e) {
            (listBox_DataCollection.SelectedItem as V2DataCollection).Add(DataItem.DataItem);
            MainCollection.UpdateData();
        }

        private void AddDataItemCanExecuted(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = DataItem.IsValid() && listBox_DataCollection != null && listBox_DataCollection.SelectedItem != null 
                && (listBox_DataCollection.SelectedItem as V2DataCollection).CheckNewData(DataItem.CoordField) && Complex.Abs(DataItem.ValueField) > 0.0;
        }

        private void RemoveExecuted(object sender, ExecutedRoutedEventArgs e) {
            var dc = listBox_Main.SelectedItem as V2Data;
            var id = dc.Description;
            var w = dc.Freq_field;
            MainCollection.Remove(id, w);
            MainCollection.IsSave = false;
        }

        private void RemoveCanExecuted(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = listBox_Main != null && listBox_Main.SelectedItem != null;
        }
        #endregion

        #region Filters
        private void FilterDataCollection(object sender, FilterEventArgs args) {
            args.Accepted = args.Item is V2DataCollection;
        }

        private void FilterDataOnGrid(object sender, FilterEventArgs args) {
            args.Accepted = args.Item is V2DataOnGrid;
        }
        #endregion

        #region Utility
        private bool SaveDialog() {
            var MessageSave = MessageBox.Show("Сохранить текущий экземпляр?", "UILab1", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (MessageSave) {
                case MessageBoxResult.Yes:
                    SaveFileDialog SaveDialog = new SaveFileDialog {
                        Filter = "Binary data|*.dat|All|*.*",
                        FilterIndex = 1
                    };

                    if (SaveDialog.ShowDialog() == true) {
                        MainCollection.Save(SaveDialog.FileName);
                        MainCollection.IsSave = true;
                    }
                    break;
                case MessageBoxResult.Cancel:
                    return false;
            }
            return true;
        }
        #endregion
    }

    public class DataItemView : INotifyPropertyChanged, IDataErrorInfo {
        public string Coord_field_X { get; set; }
        public string Coord_field_Y { get; set; }
        public string Value_field_Re { get; set; }
        public string Value_field_Im { get; set; }

        private readonly string fp_pattern = @"^-?\d+.\d*$";
        private NumberFormatInfo provider = new NumberFormatInfo();

        public DataItemView() {
            provider.NumberGroupSeparator = ".";
        }

        public string this[string columnName] {
            get
            {
                string error = string.Empty;
                switch (columnName) {
                    case "Coord_field_X":
                        if (Coord_field_X != null && !Regex.IsMatch(Coord_field_X.ToString(), fp_pattern, RegexOptions.IgnoreCase)) {
                            error = "Недопустимый формат выражения";
                        }
                        break;
                    case "Coord_field_Y":
                        if (Coord_field_Y != null && !Regex.IsMatch(Coord_field_Y.ToString(), fp_pattern, RegexOptions.IgnoreCase)) {
                            error = "Недопустимый формат выражения";
                        }
                        break;
                    case "Value_field_Re":
                        if (Value_field_Re != null && !Regex.IsMatch(Value_field_Re.ToString(), fp_pattern, RegexOptions.IgnoreCase)) {
                            error = "Недопустимый формат выражения";
                        }
                        break;
                    case "Value_field_Im":
                        if (Value_field_Im != null && !Regex.IsMatch(Value_field_Im.ToString(), fp_pattern, RegexOptions.IgnoreCase)) {
                            error = "Недопустимый формат выражения";
                        }
                        break;
                }
                return error;
            }
        }

        public string Error => throw new System.NotImplementedException();

        public bool IsValid() => this[nameof(Coord_field_X)] == string.Empty && this[nameof(Coord_field_Y)] == string.Empty && 
            this[nameof(Value_field_Re)] == string.Empty && this[nameof(Value_field_Im)] == string.Empty;

        public Vector2 CoordField => new Vector2(Convert.ToSingle(Coord_field_X, provider), Convert.ToSingle(Coord_field_Y, provider));
        public Complex ValueField => new Complex(Convert.ToDouble(Value_field_Re, provider), Convert.ToDouble(Value_field_Im, provider));
        public DataItem DataItem => new DataItem(ValueField, CoordField);

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class WindowCommands {
        public static RoutedCommand AddDataItem { get; set; }

        static WindowCommands() {
            AddDataItem = new RoutedCommand("AddDataItem", typeof(MainWindow));
        }
    }
}
