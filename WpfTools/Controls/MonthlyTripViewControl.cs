#region Header
////////////////////////////////////////////////////////////////////////////// 
//    This file is part of $projectname$.
//
//    $projectname$ is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    $projectname$ is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with $projectname$.  If not, see <http://www.gnu.org/licenses/>.
///////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace WpfTools.Controls
{
    public delegate void LoadTripsRoutedEventHandler(object sender, LoadTripsRoutedEventArgs args);

    public class MonthlyTripViewControl : Canvas
    {
        //Mo-Su
        private const int NumberOfColumns = 7;
        private const int NumberOfRows = 6;

        private DayBoxControl[,] _dayBoxes;
        private ListCollectionView _internalTripList;

        #region SelectedDate
        /// <summary>
        /// Enter description here
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDate.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(MonthlyTripViewControl), new UIPropertyMetadata(null, OnSelectedDateChanged));

        private static void OnSelectedDateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var monthlyTripViewControl = obj as MonthlyTripViewControl;
            if (monthlyTripViewControl != null)
            {
                monthlyTripViewControl.OnSelectedDateChanged((DateTime)args.NewValue);
            }
        }
        #endregion

        #region CurrentMonth
        /// <summary>
        /// Description
        /// </summary>
        public DateTime CurrentMonth
        {
            get { return (DateTime) GetValue(CurrentMonthProperty); }
            set { SetValue(CurrentMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentMonth.
        /// </summary>
        public static readonly DependencyProperty CurrentMonthProperty =
            DependencyProperty.Register("CurrentMonth", typeof (DateTime), typeof (MonthlyTripViewControl), new UIPropertyMetadata(default(DateTime), OnMonthChanged));

        private static void OnMonthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var monthlyTripViewControl = obj as MonthlyTripViewControl;
            if(monthlyTripViewControl != null)
            {
                monthlyTripViewControl.OnMonthChanged((DateTime)args.NewValue);
            }
        }
        #endregion

        #region TripList
        /// <summary>
        /// Description
        /// </summary>
        public IList<TripItem> TripList
        {
            get { return (IList<TripItem>)GetValue(TripListProperty); }
            set { SetValue(TripListProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TripList.
        /// </summary>
        public static readonly DependencyProperty TripListProperty =
            DependencyProperty.Register("TripList", typeof(IList<TripItem>), typeof(MonthlyTripViewControl), new UIPropertyMetadata(null, OnTripListChanged));

        private static void OnTripListChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var monthlyTripViewControl = obj as MonthlyTripViewControl;
            if (monthlyTripViewControl != null)
            {
                monthlyTripViewControl.OnTripListChanged((IList<TripItem>)args.OldValue, (IList<TripItem>)args.NewValue);
            }
        }
        #endregion

        #region LoadTripsEvent
        // Create a custom routed event by first registering a RoutedEventID 
        // This event uses the bubbling routing strategy 
        public static readonly RoutedEvent LoadTripsEvent = EventManager.RegisterRoutedEvent(
            "LoadTrips", RoutingStrategy.Bubble, typeof(LoadTripsRoutedEventHandler), typeof(MonthlyTripViewControl));

        // Provide CLR accessors for the event 
        public event LoadTripsRoutedEventHandler LoadTrips
        {
            add { AddHandler(LoadTripsEvent, value); }
            remove { RemoveHandler(LoadTripsEvent, value); }
        }
        #endregion

        // This method raises the Tap event 
        private void RaiseLoadTripsEvent()
        {
            if(CurrentMonth == default(DateTime))
                return;

            RaiseEvent(new LoadTripsRoutedEventArgs(CurrentMonth, LoadTripsEvent));
        }

        public MonthlyTripViewControl()
        {
            PopulateDayBoxes();
            CurrentMonth = DateTime.Today;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double width = arrangeSize.Width / NumberOfColumns;
            double height = arrangeSize.Height / NumberOfRows;

            for (int x = 0; x < NumberOfColumns; x++)
            {
                for (int y = 0; y < NumberOfRows; y++)
                {
                    int column = (int)(x * width);
                    int row = (int)(y * height);
                    _dayBoxes[x, y].Arrange(new Rect(column, row, width, height));    
                }
            }

            return arrangeSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement uiElement in this.InternalChildren)
            {
                if (uiElement != null)
                {
                    uiElement.Measure(availableSize);
                    if (!uiElement.DesiredSize.Equals(availableSize))
                    {
                        constraint = new Size(uiElement.DesiredSize.Width*NumberOfColumns,
                                        uiElement.DesiredSize.Height*NumberOfRows);
                    }
                }
            }
            return constraint;
        }

        private void PopulateDayBoxes()
        {
            _dayBoxes = new DayBoxControl[NumberOfColumns, NumberOfRows];
            for (int x = 0; x < NumberOfColumns; x++)
            {
                for (int y = 0; y < NumberOfRows; y++)
                {
                    var dayBox = new DayBoxControl();
                    dayBox.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnDayBoxMouseButtonUp), true);
                    Children.Add(dayBox);
                    _dayBoxes[x, y] = dayBox;
                }
            }
        }

        private void OnDayBoxMouseButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var dayBox = sender as DayBoxControl;
            if(dayBox == null)
                return;

            if(dayBox.DataContext == null)
                return;

            SelectedDate = dayBox.Date;
        }

        private void OnSelectedDateChanged(DateTime newValue)
        {
            
        }

        private void OnMonthChanged(DateTime newMonthValue)
        {
            if (newMonthValue.Equals(default(DateTime)))
                return;

            LabelDayBoxes();

            RaiseLoadTripsEvent();

            FillWithTripInfo();
        }

        private void OnTripListChanged(IEnumerable<TripItem> oldValue, IEnumerable<TripItem> newValue)
        {
            // Remove handler for oldValue.CollectionChanged
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= TripItemCollectionChanged;
            }
            // Add handler for newValue.CollectionChanged (if possible)
            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += TripItemCollectionChanged;
            }
        }

        private void TripItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FillWithTripInfo();
        }

        private void FillWithTripInfo()
        {
            if(TripList == null || TripList.Count == 0)
                return;

            for (int x = 0; x < NumberOfColumns; x++)
            {
                for (int y = 0; y < NumberOfRows; y++)
                {
                    var dayBox = _dayBoxes[x, y];

                    var trip = TripList.FirstOrDefault(item => item.Date.Year == dayBox.Date.Year &&
                                                               item.Date.Month == dayBox.Date.Month &&
                                                               item.Date.Day == dayBox.Date.Day);

                    if(trip != null && trip.Parent != null)
                    {
                        ((DayBoxControl) trip.Parent).Content = null;
                    }

                    dayBox.Content = trip;
                }
            }
        }

        private void LabelDayBoxes()
        {
            int maxDays = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
            DateTime prevMonth = CurrentMonth.Month > 1 ? new DateTime(CurrentMonth.Year, CurrentMonth.Month - 1, 1) :
                                                                                                                         new DateTime(CurrentMonth.Year - 1, 12, 1);
            DateTime nextMonth = CurrentMonth.Month == 12 ? new DateTime(CurrentMonth.Year + 1, 1, 1) :
                                                                                                          new DateTime(CurrentMonth.Year, CurrentMonth.Month + 1, 1);
            int prevMaxDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            int weekOfDay = (int)new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1).DayOfWeek;
            weekOfDay = weekOfDay == 0 ? 7 : weekOfDay;
            int numberOfEmptyDayBoxesStarting = weekOfDay - 1;

            int lastMonthDayCounter = 0;
            while (numberOfEmptyDayBoxesStarting - lastMonthDayCounter > 0)
            {
                _dayBoxes[lastMonthDayCounter, 0].Date = new DateTime(prevMonth.Year, prevMonth.Month, prevMaxDays - numberOfEmptyDayBoxesStarting + lastMonthDayCounter + 1);
                _dayBoxes[lastMonthDayCounter, 0].IsCurrentMonth = false;
                lastMonthDayCounter++;
            }

            int thisMonthDayCounter = 0;
            while (thisMonthDayCounter < maxDays)
            {
                int x = (thisMonthDayCounter + numberOfEmptyDayBoxesStarting) % NumberOfColumns;
                int y = (thisMonthDayCounter + numberOfEmptyDayBoxesStarting) / NumberOfColumns;

                _dayBoxes[x, y].Date = new DateTime(CurrentMonth.Year, CurrentMonth.Month, thisMonthDayCounter + 1);
                _dayBoxes[x, y].IsCurrentMonth = true;
                thisMonthDayCounter++;
            }

            int nextMonthDayCounter = 0;
            int cIndex;
            int rIndex;
            do
            {
                cIndex = (thisMonthDayCounter + numberOfEmptyDayBoxesStarting + nextMonthDayCounter) % NumberOfColumns;
                rIndex = (thisMonthDayCounter + numberOfEmptyDayBoxesStarting + nextMonthDayCounter) / NumberOfColumns;

                _dayBoxes[cIndex, rIndex].Date = new DateTime(nextMonth.Year, nextMonth.Month, nextMonthDayCounter + 1);
                _dayBoxes[cIndex, rIndex].IsCurrentMonth = false;
                nextMonthDayCounter++;
            } while (cIndex < 6 || rIndex < 5);
        }
    }
}
