using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TrendViewer.DataModels;
using TrendViewer.Helpers;
using TrendViewer.Models;
using TrendViewer.MVVM;

namespace TrendViewer.ViewModels
{
    /// <summary>
    /// View model for measurement data and statistics
    /// </summary>
    public class MainWindowViewModel : NotifyPropertyBase<IMainWindowViewModel>, IMainWindowViewModel
    {
        #region private fields

        /// <summary>
        /// Data model instance
        /// </summary>
        private IMainWindowModel model;

        /// <summary>
        /// Open file dialog helper
        /// </summary>
        private IOpenFileDialogHelper openFileDialogHelper;

        /// <summary>
        /// Data model properties observer
        /// </summary>
        private PropertyObserver<IMainWindowModel> propertyObserver;

        /// <summary>
        /// File path
        /// </summary>
        private string filePath;

        /// <summary>
        /// True if data was successfully loaded
        /// </summary>
        private bool isDataLoaded;

        /// <summary>
        /// Data collection for each axis
        /// </summary>
        private ObservableCollection<MeasurementWithStatisticsDataModel> axisDataSets;

        /// <summary>
        /// Number of data points to show
        /// </summary>
        private int numberOfValues;

        #endregion

        #region public properties

        /// <summary>
        /// Data file path
        /// </summary>
        public string FilePath
        {
            get { return this.filePath; }
            set
            {
                if (value != filePath)
                {
                    this.filePath = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// True if data was successfully loaded
        /// </summary>
        public bool IsDataLoaded
        {
            get { return this.isDataLoaded; }
            set
            {
                if (value != isDataLoaded)
                {
                    this.isDataLoaded = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Number of values to show
        /// </summary>
        public int NumberOfValues
        {
            get { return this.numberOfValues; }
            set
            {
                if (value != numberOfValues)
                {
                    this.numberOfValues = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Collection of measurement and statistics data for each axis
        /// </summary>
        public ObservableCollection<MeasurementWithStatisticsDataModel> AxisDataSets
        {
            get { return this.axisDataSets; }
            set
            {
                if (!value.Equals(axisDataSets))
                {
                    this.axisDataSets = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Open file command
        /// </summary>
        public ICommand OpenFileCommand
        {
            get { return new RelayCommand(() => OnOpenFileClick()); }
        }

        /// <summary>
        /// Recalculate command
        /// </summary>
        public ICommand RecalculateCommand
        {
            get { return new RelayCommand(() => OnRecalculate()); }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Handler for open file action
        /// </summary>
        private void OnOpenFileClick()
        {
            string fileName = openFileDialogHelper.OpenDialog();
            if (!string.IsNullOrEmpty(fileName))
            {
                model.OnFilePathUpdated(fileName);
            }
        }

        /// <summary>
        /// Handler for data update and statistics recalculation
        /// when changed the number of points
        /// </summary>
        private void OnRecalculate()
        {
            model.RecalculateStatistics(NumberOfValues);
        }

        /// <summary>
        /// Initialize property observer
        /// </summary>
        private void InitializePropertyObserver()
        {
            this.propertyObserver = new PropertyObserver<IMainWindowModel>(this.model);
        }

        /// <summary>
        /// Register the properties to observe
        /// </summary>
        private void RegisterPropertyHandlers()
        {
            if (this.propertyObserver == null)
            {
                throw new InvalidOperationException(
                    "Unable to register property handlers before property observer is initialized");
            }

            this.propertyObserver
                .RegisterHandler(n => n.FilePath, m => this.FilePath = m.FilePath)
                .RegisterHandler(n => n.IsDataLoaded, m => this.IsDataLoaded = m.IsDataLoaded)
                .RegisterHandler(n => n.NumberOfValues, m => this.NumberOfValues = m.NumberOfValues)
                .RegisterHandler(n => n.MeasurementData, OnMeasurementDataUpdated)
                .RegisterHandler(n => n.ErrorText, OnErrorHandler);
        }

        
        /// <summary>
        /// Updates the representation object once the measurement data is updated
        /// </summary>
        private void OnMeasurementDataUpdated(IMainWindowModel obj)
        {
            AxisDataSets = new ObservableCollection<MeasurementWithStatisticsDataModel>(obj.MeasurementData);
        }

        /// <summary>
        /// Handler for showing the error text message
        /// </summary>
        private void OnErrorHandler(IMainWindowModel obj)
        {
            if (!string.IsNullOrEmpty(obj.ErrorText))
            {
                MessageBox.Show($"Unable to read data file. {obj.ErrorText}", "Configuration", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel(IMainWindowModel model, IOpenFileDialogHelper openFileDialogHelper)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.openFileDialogHelper = openFileDialogHelper ?? throw new ArgumentNullException(
                nameof(openFileDialogHelper));

            InitializePropertyObserver();
            RegisterPropertyHandlers();
        }

        #endregion
    }
}