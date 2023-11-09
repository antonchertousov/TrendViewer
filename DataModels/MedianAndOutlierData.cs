namespace TrendViewer.DataModels
{
    /// <summary>
    /// Median and outliers data for each axis
    /// </summary>
    public class MedianAndOutlierData
    {
        /// <summary>
        /// Median value
        /// </summary>
        public float Median { get; }
        
        /// <summary>
        /// Upper outlier limit
        /// </summary>
        public float UpperLimit { get; }

        /// <summary>
        /// Lower outlier limit
        /// </summary>
        public float LowerLimit { get; }

        public MedianAndOutlierData(float median = 0.0f, float upperLimit = 0.0f, float lowerLimit = 0.0f)
        {
            this.Median = median;
            this.UpperLimit = upperLimit;
            this.LowerLimit = lowerLimit;
        }
    }
}