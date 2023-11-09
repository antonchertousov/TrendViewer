using System.Collections.Generic;
using TrendViewer.DataModels;

namespace TrendViewer.DataProcessing
{
    /// <summary>
    /// Interface for <see cref="DataReader"/>
    /// </summary>
    public interface IDataReader
    {
        /// <summary>
        /// Returns the list of measured data
        /// </summary>
        IEnumerable<CoordinatesData> GetTheMeasuredData(string path);
    }
}