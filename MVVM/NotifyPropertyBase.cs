using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace TrendViewer.MVVM
{
  /// <summary>
  /// Base class providing <see cref="INotifyPropertyChanged"/> 
  /// for UI classes such as Model, DataModel and ViewModel's.
  /// </summary>
  /// <typeparam name="TModel">Type containing the property to provide NotifyPropertyChanged.</typeparam>
  [ExcludeFromCodeCoverage]
  public abstract class NotifyPropertyBase<TModel> : INotifyPropertyChanged
  {
    #region Public events

    /// <summary>
    /// Value indicating the event invoked when a property on this object has a new value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion // Public events

    /// <summary>
    /// Invoked in order to raise the NotifyPropertyChanged event for the specified property.
    /// </summary>
    /// <typeparam name="TResult">Type of the property raising NotifyPropertyChanged.</typeparam>
    /// <param name="property">The property raising NotifyPropertyChanged.</param>
    protected virtual void NotifyPropertyChanged<TResult>(Expression<Func<TModel, TResult>> property)
    {
      MemberExpression member = property.Body as MemberExpression;

      if (member == null)
      {
        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid expression '{0}.", property));
      }

      string propertyName = member.Member.Name;

      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// This method is called by the Set accessor of each property.
    /// The CallerMemberName attribute that is applied to the optional propertyName
    /// parameter causes the property name of the caller to be substituted as an argument.
    /// </summary>
    /// <param name="propertyName">Value indicating the name for the property to notify.</param>
    protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}