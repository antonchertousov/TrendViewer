using Newtonsoft.Json;

namespace TrendViewer.DataModels
{
    /// <summary>
    /// Coordinates json data object
    /// </summary>
    public class CoordinatesJsonModel
    {
        /// <summary>
        /// Measurement identifier
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }

        /// <summary>
        /// X axis value
        /// </summary>
        [JsonProperty("X")]
        public float X { get; set; }

        /// <summary>
        /// Y axis value
        /// </summary>
        [JsonProperty("Y")]
        public float Y { get; set; }

        /// <summary>
        /// Z axis value
        /// </summary>
        [JsonProperty("Z")]
        public float Z { get; set; }
    }
}