using System.IO;
using System.Reflection;
using FluentAssertions;
using TrendViewer.DataProcessing;
using Xunit;

namespace TrendViewer.Tests
{
    /// <summary>
    /// Tests for <see cref="DataReader"/>
    /// </summary>
    public class DataReaderTests
    {
        private readonly IDataReader _dataReader;
        private readonly string _validFileName = "valid-data-file.json";
        private readonly string _invalidFileName = "invalid-data-file.json";
        private readonly string _path;

        /// <summary>
        /// Initialize test object and path values 
        /// </summary>
        public DataReaderTests()
        {
            _dataReader = new DataReader();
            _path = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Resources";
        }

        /// <summary>
        /// Test for valid data reading
        /// </summary>
        [Fact]
        public void ValidFileReading_Test()
        {
            // Arrange
            // Act
            var result = _dataReader.GetTheMeasuredData($"{_path}/{_validFileName}");
            
            // Assert
            result.Should().NotBeEmpty();
        }

        /// <summary>
        /// Test for invalid data reading
        /// </summary>
        [Fact]
        public void InValidFileReading_Test()
        {
            // Arrange
            // Act
            var result = _dataReader.GetTheMeasuredData($"{_path}/{_invalidFileName}");
            
            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Test for invalid data path
        /// </summary>
        [Fact]
        public void InValidFilePath_Test()
        {
            // Arrange
            // Act
            var result = _dataReader.GetTheMeasuredData($"not.exists.file");
            
            // Assert
            result.Should().BeNull();
        }
    }
}