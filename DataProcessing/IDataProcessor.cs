using System.Collections.Generic;
using TrendViewer.DataModels;

namespace TrendViewer.DataProcessing
{
    public interface IDataProcessor
    {
        /// <summary>
        /// Calculate median and outlier limits based on the list of values
        /// </summary>
        /// <param name="data">Data collection</param>
        MedianAndOutlierData CalculateMedianAndOutlierLimits(IEnumerable<float> data);

        /// <summary>
        /// Find the outlier values based on defined statistics values
        /// </summary>
        /// <param name="dataSource">measurement ids and data values</param>
        /// <param name="upLimit">Upper outlier limit</param>
        /// <param name="lowLimit">Lower outlier limit</param>
        IEnumerable<DataPoint> CalculateOutliers(IEnumerable<DataPoint> dataSource,
            float upLimit, float lowLimit);

        /// <summary>
        /// Calculates the maximum difference for the values in the sequence
        /// </summary>
        float CalculateMaxVariation(IEnumerable<float> values);

        /// <summary>
        /// Calculating the slope of the sequence values
        /// </summary>
        /// /// <param name="inputSequence">measurement ids and data values</param>
        float CalculateSlope(IEnumerable<DataPoint> inputSequence);

        /// <summary>
        /// Calculate the trend value based on slope
        /// </summary>
        /// <param name="slopeValue">slope value</param>
        /// <param name="tolerance">The tolerance value to identify the flat trend</param>
        Trend CalculateTrend(float slopeValue, float tolerance = 0.0001f);
    }
}