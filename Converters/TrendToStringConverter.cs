using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using TrendViewer.DataModels;

namespace TrendViewer.Converters
{
    /// <summary>
    /// Converts trend enum value to corresponding string
    /// </summary>
    [ExcludeFromCodeCoverage]
    [ValueConversion(typeof(Trend), typeof(string))]
    public class TrendToStringConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Invoked in order to convert the specified Trend value into string
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        /// <summary>
        /// Convert string to trend value is not supported
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}