using System;
using System.Windows;

namespace WpfTools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class LoadTripsRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the date.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadTripsRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        public LoadTripsRoutedEventArgs(DateTime date)
            : this(date, null)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadTripsRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="routedEvent">The routed event.</param>
        public LoadTripsRoutedEventArgs(DateTime date, RoutedEvent routedEvent)
            : this(date, routedEvent, null)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadTripsRoutedEventArgs"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source.</param>
        public LoadTripsRoutedEventArgs(DateTime date, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            Date = date;
        }
    }
}
