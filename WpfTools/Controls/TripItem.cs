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
using System.Windows;
using System.Windows.Controls;

namespace WpfTools.Controls
{
    public class TripItem : Control
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
        /// Using a DependencyProperty as the backing store for DateTime.
        /// </summary>
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(TripItem), new UIPropertyMetadata(default(DateTime)));
        #endregion

        #region DriverName
        /// <summary>
        /// Enter description here
        /// </summary>
        public string DriverName
        {
            get { return (string)GetValue(DriverNameProperty); }
            set { SetValue(DriverNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DriverName.
        /// </summary>
        public static readonly DependencyProperty DriverNameProperty =
            DependencyProperty.Register("DriverName", typeof(string), typeof(TripItem), new UIPropertyMetadata(default(string)));
        #endregion

        #region PassengerNames
        /// <summary>
        /// Enter description here
        /// </summary>
        public IList<string> PassengerNames
        {
            get { return (IList<string>)GetValue(PassengerNamesProperty); }
            set { SetValue(PassengerNamesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PassengerNames.
        /// </summary>
        public static readonly DependencyProperty PassengerNamesProperty =
            DependencyProperty.Register("PassengerNames", typeof(IList<string>), typeof(TripItem), new UIPropertyMetadata(default(IList<string >)));
        #endregion
    }
}
