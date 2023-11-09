using System.Collections.Generic;
using System.ComponentModel;
using TrendViewer.DataModels;

namespace TrendViewer.Models
{
    /// <summary>
    /// Interface for <see cref="MainWindowModel"/>
    /// Provides the data model properties and methods
    /// </summary>
    public interface IMainWindowModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Data file path
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Error text message
        /// </summary>
        string ErrorText { get; }

        /// <summary>
        /// Collection of data points with the statistics 
        /// </summary>
        List<MeasurementWithStatisticsDataModel> MeasurementData { get; }

        /// <summary>
        /// True if data was loaded from file
        /// </summary>
        bool IsDataLoaded { get; set; }

        /// <summary>
        /// Handler for file path update
        /// </summary>
        void OnFilePathUpdated(string path);

        /// <summary>
        /// Handler for statistics recalculation
        /// </summary>
        void RecalculateStatistics(int numberOfValues);

        /// <summary>
        /// Number of values to show
        /// </summary>
        int NumberOfValues { get; set; }
    }
}