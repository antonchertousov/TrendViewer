namespace TrendViewer.DataModels
{
    /// <summary>
    /// Measured data element with id and xyz coordinates
    /// </summary>
    public sealed class CoordinatesData
    {
        /// <summary>
        /// Measurement identifier
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Measured X value
        /// </summary>
        public float X { get; }

        /// <summary>
        /// Measured Y value
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// Measured Z value
        /// </summary>
        public float Z { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CoordinatesData(int idNumber, float xValue, float yValue, float zValue)
        {
            Id = idNumber;
            X = xValue;
            Y = yValue;
            Z = zValue;
        }
    }
}