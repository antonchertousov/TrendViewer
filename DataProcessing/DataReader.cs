using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using TrendViewer.DataModels;

namespace TrendViewer.DataProcessing
{
    /// <summary>
    /// The class responsible for reading the data from file and transform to domain data model
    /// </summary>
    public sealed class DataReader : IDataReader
    {
        #region private fields

        private static readonly ILog log = LogManager.GetLogger(typeof(DataReader));

        #endregion

        #region private methods

        /// <summary>
        /// Read the json data from file by specified the path
        /// </summary>
        private List<CoordinatesJsonModel> ReadDataFromFile(string dataPath)
        {
            try
            {
                if (!System.IO.File.Exists(dataPath))
                {
                    log.ErrorFormat("The path '{0}' does not exists", dataPath);
                    return default;
                }

                var content = System.IO.File.ReadAllText(dataPath);
                var result = JsonConvert.DeserializeObject<List<CoordinatesJsonModel>>(content);
                return result;
            }
            catch (Exception e)
            {
                log.Error($"Error while reading the data from file {dataPath}", e);
            }

            return default;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Returns the list of measured data
        /// </summary>
        public IEnumerable<CoordinatesData> GetTheMeasuredData(string path)
        {
            var rawData = ReadDataFromFile(path);
            return rawData?.Select(x => new CoordinatesData(x.Id, x.X, x.Y, x.Z));
        }
        #endregion
    }
}