using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TrendViewer.DataModels;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using DataPoint = TrendViewer.DataModels.DataPoint;
using PlotModel = OxyPlot.PlotModel;

namespace TrendViewer.Helpers
{
    /// <summary>
    /// The helper class to create Plot model objects
    /// OxyPlot 2.1.2 requires to provide PlotModel object to the view
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class PlotModelCreator
    {
        #region public methods

        /// <summary>
        /// Creates the plot model for the data
        /// </summary>
        /// <param name="axisTitle">Axis title</param>
        /// <param name="data">Data points collection</param>
        /// <param name="median">Median value</param>
        /// <param name="upperLimit">Upper limit value</param>
        /// <param name="lowerLimit">Lower limit value</param>
        /// <returns>Plot model</returns>
        public static PlotModel Create(string axisTitle, IEnumerable<(int Id, float Value)> data,
            float median, float upperLimit, float lowerLimit)
        {
            var collection = data.ToList();
            var model = new PlotModel
            {
                Title = axisTitle
            };

            var series = new ScatterSeries()
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Green,
                BinSize = 1
            };
            series.Points.AddRange(collection.Select(x => new ScatterPoint(x.Id, x.Value)));
            model.Series.Add(series);

            var yAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                MaximumPadding = 0.1,
                MinimumPadding = 0.1,
            };

            double maxRange = Math.Abs(collection.Max(x => x.Value) - collection.Min(x => x.Value));
            if (maxRange != 0)
            {
                yAxis.MajorStep = maxRange / 5;
            }

            var xAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                AbsoluteMaximum = collection.Count + 1,
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            // Median line
            LineAnnotation line = new LineAnnotation()
            {
                StrokeThickness = 1,
                Color = OxyColors.Blue,
                Type = LineAnnotationType.Horizontal,
                Text = $"{median:F3}",
                TextColor = OxyColors.Blue,
                Y = median
            };

            LineAnnotation hiLine = new LineAnnotation()
            {
                StrokeThickness = 1,
                Color = OxyColors.Red,
                Type = LineAnnotationType.Horizontal,
                Y = (double) upperLimit
            };
            LineAnnotation lowLine = new LineAnnotation()
            {
                StrokeThickness = 1,
                Color = OxyColors.Red,
                Type = LineAnnotationType.Horizontal,
                Y = (double) lowerLimit
            };
            model.Annotations.Add(line);
            model.Annotations.Add(hiLine);
            model.Annotations.Add(lowLine);
            model.Axes.Clear();
            model.Axes.Add(xAxis);
            model.Axes.Add(yAxis);
            model.InvalidatePlot(true);
            return model;
        }

        #endregion
    }
}