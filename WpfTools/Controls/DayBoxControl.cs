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
using System.Windows;
using System.Windows.Controls;

namespace WpfTools.Controls
{
    public class DayBoxControl : ContentControl
    {
        #region Date
        /// <summary>
        /// Enter description here
        /// </summary>
        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Date.
        /// </summary>
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(DayBoxControl), new UIPropertyMetadata(default(DateTime), OnDateChanged));

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dayBoxControl = d as DayBoxControl;
            if(dayBoxControl != null)
            {
                dayBoxControl.SetValue(DayNamePropertyKey, Enum.GetName(typeof(DayOfWeek),((DateTime) e.NewValue).DayOfWeek));
            }
        }

        #endregion

        #region DayName
        /// <summary>
        /// Using a DependencyProperty as the backing store for DayName.
        /// </summary>
        public static readonly DependencyPropertyKey DayNamePropertyKey =
            DependencyProperty.RegisterReadOnly("DayName", typeof(string), typeof(DayBoxControl), new UIPropertyMetadata(default(string)));

        public static readonly DependencyProperty DayNameProperty = DayNamePropertyKey.DependencyProperty;
        public double DayName
        {
            get { return (double)GetValue(DayNameProperty); }
        }
        #endregion

        #region IsCurrentMonth
        /// <summary>
        /// Enter description here
        /// </summary>
        public bool IsCurrentMonth
        {
            get { return (bool)GetValue(IsCurrentMonthProperty); }
            set { SetValue(IsCurrentMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCurrentMonth.
        /// </summary>
        public static readonly DependencyProperty IsCurrentMonthProperty =
            DependencyProperty.Register("IsCurrentMonth", typeof(bool), typeof(DayBoxControl), new UIPropertyMetadata(default(bool)));
        #endregion
    }
}
