namespace TrendViewer
{
    /// <summary>
    /// Application constants class
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Constant for X axis
        /// </summary>
        public const string XKey = "X";
        
        /// <summary>
        /// Constant for Y axis
        /// </summary>
        public const string YKey = "Y";
        
        /// <summary>
        /// Constant for Z axis
        /// </summary>
        public const string ZKey = "Z";
        
        /// <summary>
        /// Minimal data set value
        /// </summary>
        public const int MinDataSetValue = 7;

        /// <summary>
        /// Error text for wrong data format
        /// </summary>
        public const string WrongDataFormatErrorText = "Wrong data file format.";

        /// <summary>
        /// Error text for not enough measurements
        /// </summary>
        public const string NotEnoughMeasurementsErrorText = "The input data contains no enough measurements.";

        /// <summary>
        /// Error text for duplicate measurement Ids
        /// </summary>
        public const string DuplicateMeasurementsErrorText =
            "The input data contains duplicate measurement identifiers.";
        
        
    }
}