using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using OxyPlot;
using TrendViewer.DataModels;
using TrendViewer.Helpers;

namespace TrendViewer.Converters
{
    /// <summary>
    /// Converts measurement and statistics data to the plot model object
    /// </summary>
    [ExcludeFromCodeCoverage]
    [ValueConversion(typeof(MeasurementWithStatisticsDataModel), typeof(PlotModel))]
    public class DataToPlotModelConverter : MarkupExtension, IValueConverter
    {
        #region public methods

        /// <summary>
        /// Invoked in order to convert measurement and statistics data to the plot model object
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (MeasurementWithStatisticsDataModel) value;
            if (data == null)
            {
                return null;
            }

            return PlotModelCreator.Create(
                $"{data.AxisName} axis data",
                data.Measurements.Select(d => (d.Id, d.Value)),
                data.MedianAndOutlierLimits.Median,
                data.MedianAndOutlierLimits.UpperLimit,
                data.MedianAndOutlierLimits.LowerLimit);
        }


        /// <summary>
        /// Convert back is not supported
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}