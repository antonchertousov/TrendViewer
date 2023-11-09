using System;
using System.Collections.Generic;
using System.Linq;
using TrendViewer.DataModels;

namespace TrendViewer.DataProcessing
{
    /// <summary>
    /// Service for data processing
    /// </summary>
    public class DataProcessor : IDataProcessor
    {
        #region public methods

        /// <summary>
        /// Calculate median and outlier limits based on the list of values
        /// </summary>
        public MedianAndOutlierData CalculateMedianAndOutlierLimits(IEnumerable<float> data)
        {
            // Ordered sequence of values from lower to higher
            var values = data.OrderBy(x => x).ToArray();
            
            // Restriction for simplification: we need not less than 7 values for all interquartiles calculations 
            if (values.Length < Constants.MinDataSetValue)
            {
                throw new ArgumentException(
                    $"The length of data set should not be less than {Constants.MinDataSetValue} values");
            }

            // The median is the second quartile Q2. It divides the ordered data set into higher and lower halves
            int size = values.Length; 
            int midQ2Size = size / 2;
 
            var q2 = size % 2 != 0
                ? values[midQ2Size]
                : (values[midQ2Size] + values[midQ2Size - 1]) / 2;

            // The first quartile, Q1, is the median of the lower half not including Q2
            var q1Array = values.Take(midQ2Size - 1).ToArray();
            int midQ1Size = q1Array.Length / 2;
            
            var q1 = midQ1Size % 2 != 0
                ? q1Array[midQ1Size]
                : (q1Array[midQ1Size] + q1Array[midQ1Size - 1]) / 2;

            // The third quartile, Q3, is the median of the higher half not including Q2
            var q3Array = values.Skip(midQ2Size).ToArray();
            int midQ3Size = q3Array.Length / 2;
            
            var q3 = midQ3Size % 2 != 0
                ? q3Array[midQ3Size]
                : (q3Array[midQ3Size] + q3Array[midQ3Size - 1]) / 2;

            // The range from Q1 to Q3 is the interquartile range (IQR)
            float iQR = q3 - q1;

            // Potential outliers are values that lie above the Upper Fence or below the Lower Fence of the sample set
            float uF = q3 + 1.5f * iQR;
            float lF = q1 - 1.5f * iQR;

            return new MedianAndOutlierData(q2, uF, lF);
        }

        /// <summary>
        /// Find the outlier values based on defined statistics values
        /// </summary>
        /// <param name="dataSource">measurement ids and data values</param>
        /// <param name="upLimit">Upper outlier limit</param>
        /// <param name="lowLimit">Lower outlier limit</param>
        public IEnumerable<DataPoint> CalculateOutliers(IEnumerable<DataPoint> dataSource,
            float upLimit, float lowLimit)
        {
            return dataSource.Where(x => x.Value < lowLimit || x.Value > upLimit);
        }

        /// <summary>
        /// Calculates the maximum difference for the values in the sequence
        /// </summary>
        public float CalculateMaxVariation(IEnumerable<float> values)
        {
            return Math.Abs(values.Min() - values.Max());
        }

        /// <summary>
        /// Calculating the slope of the sequence values
        /// </summary>
        public float CalculateSlope(IEnumerable<DataPoint> inputSequence)
        {
            var data = inputSequence.ToList();
            var averageX = data.Average(d => d.Id);
            var averageY = data.Average(d => d.Value);

            return (float)(data.Sum(d => (d.Id - averageX) * (d.Value - averageY)) /
                           data.Sum(d => Math.Pow(d.Id - averageX, 2)));
        }

        /// <summary>
        /// Calculate the trend value based on slope
        /// </summary>
        /// <param name="slopeValue">slope value</param>
        /// <param name="tolerance">The tolerance value to identify the flat trend</param>
        /// <returns></returns>
        public Trend CalculateTrend(float slopeValue, float tolerance = 0.0001f)
        {
            return slopeValue > tolerance
                ? Trend.Positive
                : slopeValue < -tolerance
                    ? Trend.Negative
                    : Trend.Flat;
        }
        #endregion
    }
}