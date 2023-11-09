namespace TrendViewer.DataModels
{
    /// <summary>
    /// Data point
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// Measurement identifier
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Measurement value
        /// </summary>
        public float Value { get; }

        public DataPoint(int id, float value)
        {
            Id = id;
            Value = value;
        }
    }
}