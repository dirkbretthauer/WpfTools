#region Header
////////////////////////////////////////////////////////////////////////////// 
//The MIT License (MIT)

//Copyright (c) 2013 Dirk Bretthauer

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//the Software, and to permit persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
