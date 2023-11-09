using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using TrendViewer.DataModels;
using TrendViewer.DataProcessing;
using TrendViewer.MVVM;

namespace TrendViewer.Models
{
    /// <summary>
    /// Main window model class
    /// </summary>
    public class MainWindowModel : NotifyPropertyBase<IMainWindowModel>, IMainWindowModel
    {
        #region private fields

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindowModel));

        /// <summary>
        /// Data reader
        /// </summary>
        private readonly IDataReader dataReader;

        /// <summary>
        /// Data processor
        /// </summary>
        private readonly IDataProcessor dataProcessor;
        
        /// <summary>
        /// Measurement data file path
        /// </summary>
        private string filePath;

        /// <summary>
        /// Indicates if data was successfully loaded from file
        /// </summary>
        private bool isDataLoaded;

        /// <summary>
        /// Contains the text of the error
        /// </summary>
        private string errorText;

        /// <summary>
        /// Initial loaded data set from file
        /// </summary>
        private List<CoordinatesData> loadedData;

        /// <summary>
        /// Number of values to show
        /// </summary>
        private int numberOfValues;

        #endregion

        #region public properties

        /// <summary>
        /// Collection of measurements and statistics data for each axis
        /// </summary>
        public List<MeasurementWithStatisticsDataModel> MeasurementData { get; }

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
        /// Error text
        /// </summary>
        public string ErrorText
        {
            get { return this.errorText; }
            set
            {
                if (value != errorText)
                {
                    this.errorText = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// True if data was successfully loaded
        /// False in case of errors on loading
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
        /// The number of first n values to show the data and calculate the statistics
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

        #endregion

        #region private methods

        /// <summary>
        /// Calculate the median value and outlier limits for the axis
        /// </summary>
        /// <param name="data">Data set</param>
        /// <param name="axisName">Axis name</param>
        private MedianAndOutlierData CalculateMedianAdOutlierLimits(string axisName, IEnumerable<float> data)
        {
            var statistics = dataProcessor.CalculateMedianAndOutlierLimits(data);
            log.DebugFormat("Stat data for {0} axis. Median={1:F3}, Upper outlier limit= {2:F3}, Lower outlier limit = {3:F3}",
                axisName, statistics.Median, statistics.UpperLimit, statistics.LowerLimit);
            return statistics;
        }

        /// <summary>
        /// Calculate the slope for the axis
        /// </summary>
        /// <param name="axisName">Axis name</param>
        /// <param name="axisData">Axis data</param>
        private float CalculateSlopeValue(string axisName, IEnumerable<DataPoint> axisData)
        {
            var slope = dataProcessor.CalculateSlope(axisData);
            log.DebugFormat("Calculated slope for {0} axis: {1:F3}.", axisName, slope);
            return slope;
        }

        /// <summary>
        /// Calculate the max variation for the axis
        /// </summary>
        /// <param name="axisName">Axis name</param>
        /// <param name="data">Axis data</param>
        private double CalculateMaxVariationOnAxis(string axisName, IEnumerable<float> data)
        {
            var variation = dataProcessor.CalculateMaxVariation(data);
            log.DebugFormat("Calculated max variation for {0} axis: {1:F3}.", axisName, variation);
            return variation;
        }

        /// <summary>
        /// Prepare the data to show and calculate the statistics
        /// </summary>
        private void PrepareTheDataAndCalculateStatistics(int numberOfValues)
        {
            // If measurement numbers are in mixed order they will be ordered by id
            var measurementData = loadedData
                .OrderBy(x => x.Id)
                .Take(numberOfValues)
                .ToList();

            // Arrange measurement data by axis
            var measurementsByAxis = new List<((int Id, string Name) axis, IEnumerable<DataPoint> measurements)>()
            {
                ((0, Constants.XKey), measurementData.Select(x => new DataPoint(x.Id, x.X))),
                ((1, Constants.YKey), measurementData.Select(x => new DataPoint(x.Id, x.Y))),
                ((2, Constants.ZKey), measurementData.Select(x => new DataPoint(x.Id, x.Z)))
            };

            MeasurementData.Clear();

            // Calculate and prepare the data
            try
            {
                foreach (var data in measurementsByAxis)
                {
                    var measurementValues = data.measurements.Select(x => x.Value).ToList();
                    var medianAndOutlierData = CalculateMedianAdOutlierLimits(data.axis.Name, measurementValues);
                    var slope = CalculateSlopeValue(data.axis.Name, data.measurements);
                    var trend = dataProcessor.CalculateTrend(slope);
                    var maxVariation = CalculateMaxVariationOnAxis(data.axis.Name, measurementValues);
                    var outlierValues = dataProcessor.CalculateOutliers(data.measurements,
                        medianAndOutlierData.UpperLimit, medianAndOutlierData.LowerLimit);

                    MeasurementData.Add(
                        new MeasurementWithStatisticsDataModel(data.axis.Id, data.axis.Name, data.measurements, medianAndOutlierData,
                            maxVariation, outlierValues, trend));
                }
            }
            catch (Exception e)
            {
                log.Error("Error during the statistics calculation", e);
                MeasurementData.Clear();
                return;
            }
            // Notify once the collection is fulfilled
            this.NotifyPropertyChanged(v => v.MeasurementData);

            log.InfoFormat("============Calculations were completed==========");
        }

        /// <summary>
        /// Check if no duplicate measurements are in the data set
        /// </summary>
        private bool IsNoDuplicateMeasurementIds(IEnumerable<int> data)
        {
            var numbers = data.ToList();
            var uniqueNumbers = numbers.ToHashSet();
            return numbers.Count == uniqueNumbers.Count;
        }

        /// <summary>
        /// Validates the loaded data, returns true if it is valid
        /// In case of error updates the error text
        /// </summary>
        private bool ValidateDataAndUpdateErrorText(IEnumerable<CoordinatesData> data, string path)
        {
            if (data == null)
            {
                ErrorText = Constants.WrongDataFormatErrorText;
                log.ErrorFormat("{0} {1}.", ErrorText, path);
                return false;
            }
            
            if(!IsNoDuplicateMeasurementIds(data.Select(x => x.Id)))
            {
                ErrorText = Constants.DuplicateMeasurementsErrorText;
                log.ErrorFormat("{0} {1}.", ErrorText, path);
                return false;
            }

            if(data.ToList().Count < Constants.MinDataSetValue)
            {
                ErrorText = Constants.NotEnoughMeasurementsErrorText;
                log.ErrorFormat("{0} {1}.", ErrorText, path);
                return false;
            }
            return true;
        }
        #endregion
        
        #region public methods

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowModel(IDataReader dataReader, IDataProcessor dataProcessor)
        {
            this.dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
            this.dataProcessor = dataProcessor ?? throw new ArgumentNullException(nameof(dataProcessor));
            MeasurementData = new List<MeasurementWithStatisticsDataModel>();
        }

        /// <summary>
        /// Handler for file path update
        /// </summary>
        public void OnFilePathUpdated(string path)
        {
            loadedData = dataReader.GetTheMeasuredData(path)?.ToList();
            ErrorText = string.Empty;
            IsDataLoaded = ValidateDataAndUpdateErrorText(loadedData, path);
            FilePath = path;
            if (!IsDataLoaded)
            {
                log.ErrorFormat("No data was loaded from file {0}.", path);
                return;
            }
            
            log.InfoFormat("Loaded data from file {0}.", path);
            log.DebugFormat("Initial dataset contains {0} coordinates", loadedData);
            NumberOfValues = 20;
            log.DebugFormat("Initial data to show is {0} coordinates", NumberOfValues);
            PrepareTheDataAndCalculateStatistics(NumberOfValues);
        }

        /// <summary>
        /// Recalculate statistics in case of changing the number of data values
        /// </summary>
        public void RecalculateStatistics(int number)
        {
            // Simple validation for the input value
            NumberOfValues = number <= Constants.MinDataSetValue
                ? Constants.MinDataSetValue
                : number > loadedData.Count
                    ? loadedData.Count
                    : number;

            log.InfoFormat("Start recalculating for dataset with {0} coordinates", NumberOfValues);
            PrepareTheDataAndCalculateStatistics(NumberOfValues);
        }

        #endregion
    }
}