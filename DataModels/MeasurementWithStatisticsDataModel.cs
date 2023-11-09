using System.Collections.Generic;

namespace TrendViewer.DataModels
{
    /// <summary>
    /// Measurement with the statistics data model for each axis 
    /// </summary>
    public class MeasurementWithStatisticsDataModel
    {
        /// <summary>
        /// Axis identifier
        /// </summary>
        public int AxisId { get; }

        /// <summary>
        /// Name of the axis
        /// </summary>
        public string AxisName { get; }

        /// <summary>
        /// Collection of data points
        /// </summary>
        public IEnumerable<DataPoint> Measurements { get; }

        /// <summary>
        /// Median and outlier limits
        /// </summary>
        public MedianAndOutlierData MedianAndOutlierLimits { get; }

        /// <summary>
        /// Maximum variation
        /// </summary>
        public double MaxVariation { get; }

        /// <summary>
        /// Outlier values
        /// </summary>
        public IEnumerable<DataPoint> OutlierValues { get; }
        
        /// <summary>
        /// Trend value
        /// </summary>
        public Trend Trend { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MeasurementWithStatisticsDataModel(int axisId, string axisName, IEnumerable<DataPoint> measurements,
            MedianAndOutlierData limits, double maxVariation,
            IEnumerable<DataPoint> outlierValues, Trend trend)
        {
            AxisId = axisId;
            AxisName = axisName;
            Measurements = measurements;
            MedianAndOutlierLimits = limits;
            MaxVariation = maxVariation;
            OutlierValues = outlierValues;
            Trend = trend;
        }
    }
}