using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace TrendViewer.Converters
{
  /// <summary>
  /// Converts integer value to corresponding visibility, set by TrueValue/FalseValue.
  /// True value is applied for values greater than zero, false otherwise
  /// </summary>
  [ValueConversion(typeof(int), typeof(Visibility))]
  [ExcludeFromCodeCoverage]
  public class IntToVisibilityConverter : MarkupExtension, IValueConverter
  {
    #region public properties

    /// <summary>
    /// Gets or sets a value specifying the value returned in case the converted value is true.
    /// The default value is Visibility.Visible.
    /// </summary>
    public Visibility TrueValue { get; set; }

    /// <summary>
    /// Gets or sets a value specifying the value returned in case the converted value is false.
    /// The default value is Visibility.Collapsed.
    /// </summary>
    public Visibility FalseValue { get; set; }

    #endregion

    #region public methods

    /// <summary>
    /// Initializes a new instance of the BoolToVisibilityConverter class.
    /// </summary>
    public IntToVisibilityConverter()
    {
      this.FalseValue = Visibility.Collapsed;
      this.TrueValue = Visibility.Visible;
    }

    /// <summary>
    /// Invoked in order to convert the specified value into visibility state.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var intValue = (int?) value;

      return intValue > 0 ? TrueValue : FalseValue;
    }

    /// <summary>
    /// Invoked in order to convert back to visibility state.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var visibility = ((Visibility) value);

      if (visibility == Visibility.Hidden)
      {
        visibility = Visibility.Collapsed;
      }

      return (visibility == Visibility.Visible);
    }

    /// <summary>
    /// Returns an object that is provided as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">A service provider that can provide services for the markup extension.</param>
    /// <returns>The current instance to set the property where the extension is applied.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    #endregion
  }
}