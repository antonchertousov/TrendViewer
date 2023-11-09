using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrendViewer.DataModels;

namespace TrendViewer.ViewModels
{
    /// <summary>
    /// View model interface for <see cref="MainWindowViewModel"/>
    /// </summary>
    public interface IMainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// File path to show
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// True if data was loaded from file
        /// </summary>
        bool IsDataLoaded { get; set; }

        /// <summary>
        /// Number of values to show
        /// </summary>
        int NumberOfValues { get; set; }

        /// <summary>
        /// Measurement data objects with the statistics 
        /// </summary>
        ObservableCollection<MeasurementWithStatisticsDataModel> AxisDataSets { get; set; }

        /// <summary>
        /// Open file command
        /// </summary>
        ICommand OpenFileCommand { get; }

        /// <summary>
        /// Recalculate command
        /// </summary>
        ICommand RecalculateCommand { get; }
    }
}