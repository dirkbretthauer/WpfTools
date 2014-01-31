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
using System.Reflection;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;

namespace WpfTools.Commands
{
    public class RoutedEventCommandBehavior : CommandBehaviorBase<Control>
    {
        private string _eventName;

        public string EventName
        {
            get { return _eventName; }
            set
            {
                _eventName = value;
                AddEventHandler(_eventName);
            }
        }

        public RoutedEventCommandBehavior(Control targetObject)
            : base(targetObject)
        {
            if (targetObject == null) throw new ArgumentNullException("targetObject");
        }

        private void AddEventHandler(string eventName)
        {
            EventInfo ei = TargetObject.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance);
            if (ei != null)
            {
                ei.RemoveEventHandler(TargetObject, GetEventMethod(ei));
                ei.AddEventHandler(TargetObject, GetEventMethod(ei));
            }
        }

        private Delegate _method;
        private Delegate GetEventMethod(EventInfo ei)
        {
            if (ei == null) throw new ArgumentNullException("ei");
            if (ei.EventHandlerType == null) throw new ArgumentException("EventHandlerType is null");

            if (_method == null)
            {
                _method = Delegate.CreateDelegate(ei.EventHandlerType, this,
                                                  GetType().GetMethod("OnEventRaised",
                                                                      BindingFlags.NonPublic | BindingFlags.Instance));
            }
            return _method;
        }

        /// <summary>
        /// This is invoked by the event - it invokes the command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
    }

}
