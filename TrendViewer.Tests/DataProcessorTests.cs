using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TrendViewer.DataModels;
using TrendViewer.DataProcessing;
using Xunit;

namespace TrendViewer.Tests
{
    /// <summary>
    /// Unit tests for <see cref="DataProcessor"/>
    /// </summary>
    public class DataProcessorTests
    {
        private readonly IDataProcessor _dataProcessor;
        
        public DataProcessorTests()
        {
            _dataProcessor = new DataProcessor();
        }

        /// <summary>
        /// Test for calculating median and outlier limits method
        /// </summary>
        [Fact]
        public void CalculateMedianAndOutlierLimits_Test()
        {
            // Arrange
            var data = new List<float>() {0, -1, -2, -3, -4, -5, -6};
            
            // Act
            var result = _dataProcessor.CalculateMedianAndOutlierLimits(data);

            // Assert
            result.Should().NotBeNull();
            result.Median.Should().Be(-3);
            result.LowerLimit.Should().BeNegative();
            result.UpperLimit.Should().BePositive();
        }

        /// <summary>
        /// Test for calculating median and outlier limits method
        /// </summary>
        [Fact]
        public void CalculateMedianAndOutlierLimits_ThrowsException_Test()
        {
            // Arrange
            var data = new List<float>() {0, -1, -2, -3, 4, -5};
            
            // Assert
            Assert.Throws<ArgumentException>(() => _dataProcessor.CalculateMedianAndOutlierLimits(data));
        }
        
        /// <summary>
        /// Test for calculating outliers method
        /// </summary>
        [Fact]
        public void CalculateOutliers_Test()
        {
            // Arrange
            var data = new List<DataPoint>()
            {
                new DataPoint(1, 0), new DataPoint(2, -1), new DataPoint(3, -2), new DataPoint(4, -3),
                new DataPoint(5, -4), new DataPoint(6, -5), new DataPoint(7, -6)
            };
            
            // Act
            var result = _dataProcessor.CalculateOutliers(data, data[1].Value, data[5].Value);
            
            // Assert
            result.Should().NotBeEmpty();
            result.ToList().Count.Should().Be(2);
        }

        /// <summary>
        /// Test for calculating max variation method
        /// </summary>
        [Fact]
        public void CalculateMaxVariation_Test()
        {
            // Arrange
            var data = new List<float>() {0, -1, -2, -3, -4, -5, -6};
            
            // Act
            var result = _dataProcessor.CalculateMaxVariation(data);
            
            // Assert
            result.Should().Be(Math.Abs(data.First() - data.Last()));
        }

        [Fact]
        public void CalculateNegativeSlope_Test()
        {
            // Arrange
            var data = new List<DataPoint>()
            {
                new DataPoint(1, 0), new DataPoint(2, -1), new DataPoint(3, -2), new DataPoint(4, -3),
                new DataPoint(5, -4), new DataPoint(6, -5), new DataPoint(7, -6)
            };

            // Act
            var result = _dataProcessor.CalculateSlope(data);
            
            // Assert
            result.Should().BeNegative();
        }
        
        [Fact]
        public void CalculatePositiveSlope_Test()
        {
            // Arrange
            var data = new List<DataPoint>()
            {
                new DataPoint(1, 0), new DataPoint(2, 1), new DataPoint(3, 2), new DataPoint(4, 3), new DataPoint(5, 4),
                new DataPoint(6, 5), new DataPoint(7, 6)
            };

            // Act
            var result = _dataProcessor.CalculateSlope(data);
            
            // Assert
            result.Should().BePositive();
        }

        [Theory]
        [InlineData(1, Trend.Positive)]
        [InlineData(-1, Trend.Negative)]
        [InlineData(0.0001, Trend.Flat)]
        public void CalculateTrend_Test(float value, Trend expectedTrend)
        {
            // Act
            var result = _dataProcessor.CalculateTrend(value);
            
            // Assert
            result.Should().Be(expectedTrend);
        }
    }
}