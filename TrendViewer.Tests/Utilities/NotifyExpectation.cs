using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace TrendViewer.Tests.Utilities
{
    /// <summary>
    /// Helper class to test property changed expectations
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NotifyExpectation<T>
        where T : INotifyPropertyChanged
    {
        private readonly T owner;
        private readonly string propertyName;
        private readonly bool eventExpected;
 
        public NotifyExpectation(T owner,
            string propertyName, bool eventExpected)
        {
            this.owner = owner;
            this.propertyName = propertyName;
            this.eventExpected = eventExpected;
        }
 
        public void When(Action<T> action)
        {
            bool eventWasRaised = false;
            this.owner.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == this.propertyName)
                {
                    eventWasRaised = true;
                }
            };
            action(this.owner);
 
            
            Assert.Equal<bool>(this.eventExpected,
                eventWasRaised);
            //"PropertyChanged on {0}", this.propertyName);
        }
    }
}