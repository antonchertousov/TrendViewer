using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using TrendViewer.DataModels;
using TrendViewer.DataProcessing;
using TrendViewer.Models;
using Xunit;

namespace TrendViewer.Tests
{
    /// <summary>
    /// Unit tests for <see cref="MainWindowModel"/>
    /// </summary>
    public class MainWindowModelTests
    {
        private readonly IMainWindowModel _model;
        private readonly Mock<IDataReader> _dataReaderMock = new Mock<IDataReader>();
        private readonly Mock<IDataProcessor> _dataProcessorMock = new Mock<IDataProcessor>();
        private readonly string _path = @"c:/data.json";

        /// <summary>
        /// Test init
        /// </summary>
        public MainWindowModelTests()
        {
            _model = new MainWindowModel(_dataReaderMock.Object, _dataProcessorMock.Object);
            _dataProcessorMock.Setup(
                    x => x.CalculateOutliers(It.IsAny<IEnumerable<DataPoint>>(), It.IsAny<float>(),
                        It.IsAny<float>()))
                .Returns(new List<DataPoint>());
            _dataProcessorMock.Setup(
                    x => x.CalculateMedianAndOutlierLimits(It.IsAny<IEnumerable<float>>()))
                .Returns(new MedianAndOutlierData());
            _dataProcessorMock.Setup(
                    x => x.CalculateSlope(It.IsAny<IEnumerable<DataPoint>>()))
                .Returns(1);
            _dataProcessorMock.Setup(
                    x => x.CalculateMaxVariation(It.IsAny<IEnumerable<float>>()))
                .Returns(1);
            _dataProcessorMock.Setup(
                    x => x.CalculateTrend(It.IsAny<float>(), It.IsAny<float>()))
                .Returns(Trend.Flat);
        }

        /// <summary>
        /// Successful data load test
        /// </summary>
        [Fact]
        public void DataLoadSuccessful_Test()
        {
            // Arrange
            var measurementData = new List<CoordinatesData>()
            {
                new CoordinatesData(1, 1.1f, -1, 1),
                new CoordinatesData(2, 1.1f, -1, 1),
                new CoordinatesData(3, 1.1f, -1, 1),
                new CoordinatesData(4, 1.1f, -1, 1),
                new CoordinatesData(5, 1.1f, -1, 1),
                new CoordinatesData(6, 1.1f, -1, 1),
                new CoordinatesData(7, 1.1f, -1, 1)
            };

            _dataReaderMock.Setup(
                    x => x.GetTheMeasuredData(It.IsAny<string>()))
                .Returns(measurementData);
            
            // Act
            _model.OnFilePathUpdated(_path);
            
            // Assert
            _model.IsDataLoaded.Should().BeTrue();
            _model.ErrorText.Should().BeEmpty();
            _model.FilePath.Should().Be(_path);
            _model.MeasurementData.Should().NotBeEmpty();
            foreach (var dataSet in _model.MeasurementData)
            {
                dataSet.Measurements.Should().NotBeEmpty();
                dataSet.Measurements.ToList().Count.Should().Be(measurementData.Count);
            }
        }

        /// <summary>
        /// Wrong data load test
        /// </summary>
        [Fact]
        public void DataLoadWrongData_Test()
        {
            // Arrange
            _dataReaderMock.Setup(
                    x => x.GetTheMeasuredData(It.IsAny<string>()))
                .Returns((List<CoordinatesData>) null);
            
            // Act
            _model.OnFilePathUpdated(_path);
            
            // Assert
            _model.IsDataLoaded.Should().BeFalse();
            _model.ErrorText.Should().Be(Constants.WrongDataFormatErrorText);
            _model.FilePath.Should().Be(_path);
            _model.MeasurementData.Should().BeEmpty();
        }

        /// <summary>
        /// Data with duplicate Ids load test
        /// </summary>
        [Fact]
        public void DataLoadDuplicateIdsData_Test()
        {
            // Arrange
            var measurementData = Enumerable.Range(1, 20)
                .Select(x => new CoordinatesData(11, 1, 1.1f, -100))
                .ToList();

            _dataReaderMock.Setup(
                    x => x.GetTheMeasuredData(It.IsAny<string>()))
                .Returns(measurementData);
            
            // Act
            _model.OnFilePathUpdated(_path);
            
            // Assert
            _model.IsDataLoaded.Should().BeFalse();
            _model.ErrorText.Should().Be(Constants.DuplicateMeasurementsErrorText);
            _model.FilePath.Should().Be(_path);
            _model.MeasurementData.Should().BeEmpty();
        }

        /// <summary>
        /// Data with not enough measurements load test
        /// </summary>
        [Fact]
        public void DataLoadNotEnoughMeasurements_Test()
        {
            // Arrange
            var measurementData = Enumerable.Range(1, 6)
                .Select(x => new CoordinatesData(x, 1, 1.1f, -100))
                .ToList();

            _dataReaderMock.Setup(
                    x => x.GetTheMeasuredData(It.IsAny<string>()))
                .Returns(measurementData);
            
            // Act
            _model.OnFilePathUpdated(_path);
            
            // Assert
            _model.IsDataLoaded.Should().BeFalse();
            _model.ErrorText.Should().Be(Constants.NotEnoughMeasurementsErrorText);
            _model.FilePath.Should().Be(_path);
            _model.MeasurementData.Should().BeEmpty();
        }

        /// <summary>
        /// Statistics recalculation test
        /// </summary>
        [Fact]
        public void RecalculateStatistics_Test()
        {
            // Arrange
            int numberOfElements = 10;
            
            var measurementData = Enumerable.Range(1, 20)
                .Select(x => new CoordinatesData(x, -1, 0, 1))
                .ToList();
            _dataReaderMock.Setup(
                    x => x.GetTheMeasuredData(It.IsAny<string>()))
                .Returns(measurementData);

            _model.OnFilePathUpdated(_path);
            
            // Act
            _model.RecalculateStatistics(numberOfElements);

            // Assert
            _model.IsDataLoaded.Should().BeTrue();
            _model.FilePath.Should().Be(_path);
            _model.NumberOfValues.Should().Be(numberOfElements);
            _model.MeasurementData.Should().NotBeEmpty();
            _model.MeasurementData.Count.Should().Be(3);
            foreach (var dataSet in _model.MeasurementData)
            {
                dataSet.Measurements.Should().NotBeEmpty();
                dataSet.Measurements.ToList().Count.Should().Be(numberOfElements);
            }
        }
        
        /// <summary>
        /// Recalculate statistics with wrong limits tests
        /// </summary>
        [Theory]
        [InlineData(6, 20, Constants.MinDataSetValue)]
        [InlineData(25, 20, 20)]
        public void RecalculateStatisticsWrongLimits_Test(int numberOfElements, int datasetNumbers, int expectedValue)
        {
            // Arrange
            var measurementData = Enumerable.Range(1, datasetNumbers)
                .Select(x => new CoordinatesData(x, -1, 0, 1))
                .ToList();
            _dataReaderMock.Setup(
                    x => x.GetTheMeasuredData(It.IsAny<string>()))
                .Returns(measurementData);

            _model.OnFilePathUpdated(_path);
            
            // Act
            _model.RecalculateStatistics(numberOfElements);

            // Assert
            _model.IsDataLoaded.Should().BeTrue();
            _model.FilePath.Should().Be(_path);
            _model.NumberOfValues.Should().Be(expectedValue);
            _model.MeasurementData.Should().NotBeEmpty();
            _model.MeasurementData.Count.Should().Be(3);
            foreach (var dataSet in _model.MeasurementData)
            {
                dataSet.Measurements.Count().Should().Be(expectedValue);
            }
        }
    }
}