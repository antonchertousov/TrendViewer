using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

namespace TrendViewer.MVVM
{
  /// <summary>
  /// Monitors the PropertyChanged event of an object that implements INotifyPropertyChanged,
  /// and executes callback methods (i.e. handlers) registered for properties of that object.
  /// </summary>
  /// <typeparam name="TPropertySource">The type of object to monitor for property changes.</typeparam>
  [ExcludeFromCodeCoverage]
  public class PropertyObserver<TPropertySource> : IWeakEventListener
    where TPropertySource : INotifyPropertyChanged
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of PropertyObserver, which
    /// observes the 'propertySource' object for property changes.
    /// </summary>
    /// <param name="propertySource">The object to monitor for property changes.</param>
    public PropertyObserver(TPropertySource propertySource)
    {
      if (propertySource == null)
        throw new ArgumentNullException("propertySource");

      propertySourceRef = new WeakReference(propertySource);
      propertyNameToHandlerMap = new Dictionary<string, Action<TPropertySource>>();
    }

    #endregion // Constructor

    #region Public Methods

    #region RegisterHandler

    /// <summary>
    /// Registers a callback to be invoked when the PropertyChanged event has been raised for the specified property.
    /// </summary>
    /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
    /// <param name="handler">The callback to invoke when the property has changed.</param>
    /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
    public PropertyObserver<TPropertySource> RegisterHandler(
      Expression<Func<TPropertySource, object>> expression,
      Action<TPropertySource> handler)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      string propertyName = GetPropertyName(expression);
      if (String.IsNullOrEmpty(propertyName))
        throw new ArgumentException("'expression' did not provide a property name.");

      if (handler == null)
        throw new ArgumentNullException("handler");

      TPropertySource propertySource = GetPropertySource();
      if (propertySource != null)
      {
        propertyNameToHandlerMap[propertyName] = handler;
        PropertyChangedEventManager.AddListener(propertySource, this, propertyName);
      }

      return this;
    }

    #endregion // RegisterHandler

    #region UnregisterHandler

    /// <summary>
    /// Removes the callback associated with the specified property.
    /// </summary>
    /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
    /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
    public PropertyObserver<TPropertySource> UnregisterHandler(Expression<Func<TPropertySource, object>> expression)
    {
      if (expression == null)
        throw new ArgumentNullException("expression");

      string propertyName = GetPropertyName(expression);
      if (String.IsNullOrEmpty(propertyName))
        throw new ArgumentException("'expression' did not provide a property name.");

      TPropertySource propertySource = GetPropertySource();
      if (propertySource != null)
      {
        if (propertyNameToHandlerMap.ContainsKey(propertyName))
        {
          propertyNameToHandlerMap.Remove(propertyName);
          PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
        }
      }

      return this;
    }

    #endregion // UnregisterHandler

    #endregion // Public Methods

    #region Private Helpers

    #region GetPropertyName

    private string GetPropertyName(Expression<Func<TPropertySource, object>> expression)
    {
      var lambda = expression as LambdaExpression;
      MemberExpression memberExpression;
      if (lambda.Body is UnaryExpression)
      {
        var unaryExpression = lambda.Body as UnaryExpression;
        memberExpression = unaryExpression.Operand as MemberExpression;
      }
      else
      {
        memberExpression = lambda.Body as MemberExpression;
      }


      if (memberExpression != null)
      {
        var propertyInfo = memberExpression.Member as PropertyInfo;
        return propertyInfo.Name;
      }

      return null;
    }

    #endregion // GetPropertyName

    #region GetPropertySource

    private TPropertySource GetPropertySource()
    {
      try
      {
        return (TPropertySource) propertySourceRef.Target;
      }
      catch
      {
        return default(TPropertySource);
      }
    }

    #endregion // GetPropertySource

    #endregion // Private Helpers

    #region Fields

    private readonly Dictionary<string, Action<TPropertySource>> propertyNameToHandlerMap;
    private readonly WeakReference propertySourceRef;

    #endregion // Fields

    #region IWeakEventListener Members

    bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
    {
      if (managerType == typeof (PropertyChangedEventManager))
      {
        string propertyName = ((PropertyChangedEventArgs) e).PropertyName;
        var propertySource = (TPropertySource) sender;

        if (String.IsNullOrEmpty(propertyName))
        {
          // When the property name is empty, all properties are considered to be invalidated.
          // Iterate over a copy of the list of handlers, in case a handler is registered by a callback.
          foreach (var handler in propertyNameToHandlerMap.Values.ToArray())
            handler(propertySource);

          return true;
        }
        else
        {
          Action<TPropertySource> handler;
          if (propertyNameToHandlerMap.TryGetValue(propertyName, out handler))
          {
            handler(propertySource);
            return true;
          }
        }
      }

      return false;
    }

    #endregion
  }
}