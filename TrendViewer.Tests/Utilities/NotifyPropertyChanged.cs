using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace TrendViewer.Tests.Utilities
{
    /// <summary>
    /// Extension class to test rise property changed event 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class NotifyPropertyChanged
    {
        #region public methods
        /// <summary>
        /// Should notify for property
        /// </summary>
        public static NotifyExpectation<T>
            ShouldNotifyOn<T, TProperty>(this T owner,
                Expression<Func<T, TProperty>> propertyPicker) 
            where T : INotifyPropertyChanged
        {
            return NotifyPropertyChanged.CreateExpectation(owner,
                propertyPicker, true);
        }
 
        /// <summary>
        /// Should not notify for property
        /// </summary>
        public static NotifyExpectation<T> 
            ShouldNotNotifyOn<T, TProperty>(this T owner,
                Expression<Func<T, TProperty>> propertyPicker)
            where T : INotifyPropertyChanged
        {
            return NotifyPropertyChanged.CreateExpectation(owner,
                propertyPicker, false);
        }
 
        #endregion

        #region private methods
        private static NotifyExpectation<T>
            CreateExpectation<T, TProperty>(T owner,
                Expression<Func<T, TProperty>> pickProperty,
                bool eventExpected) where T : INotifyPropertyChanged
        {
            string propertyName =
                ((MemberExpression)pickProperty.Body).Member.Name;
            return new NotifyExpectation<T>(owner,
                propertyName, eventExpected);
        }
        #endregion
    }
}