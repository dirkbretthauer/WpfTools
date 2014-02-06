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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfTools.Commands
{
    public class RoutedEvent
    {
        private static readonly DependencyProperty RoutedEventCommandBehaviorProperty = DependencyProperty.RegisterAttached(
                "RoutedEventCommandBehavior",
                typeof(RoutedEventCommandBehavior),
                typeof(RoutedEvent),
                null);


        /// <summary>
        /// Command to execute on the routed event.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(RoutedEvent),
            new PropertyMetadata(OnSetCommandCallback));

        /// <summary>
        /// Command parameter to supply on command execution.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(RoutedEvent),
            new PropertyMetadata(OnSetCommandParameterCallback));

        /// <summary>
        /// The name of the RoutedEvent
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.RegisterAttached(
            "EventName",
            typeof(string),
            typeof(RoutedEvent),
            new PropertyMetadata(OnSetEventNameCallback));

        /// <summary>
        /// Sets the <see cref="ICommand"/> to execute on the 'EventName' event.
        /// </summary>
        /// <param name="control">control to attach command</param>
        /// <param name="command">Command to attach</param>
        public static void SetCommand(Control control, ICommand command)
        {
            if (control == null) throw new ArgumentNullException("control");
            control.SetValue(CommandProperty, command);
        }

        /// <summary>
        /// Retrieves the <see cref="ICommand"/> attached to the <see cref="Control"/>.
        /// </summary>
        /// <param name="control">Control containing the Command dependency property</param>
        /// <returns>The value of the command attached</returns>
        public static ICommand GetCommand(Control control)
        {
            if (control == null) throw new ArgumentNullException("control");
            return control.GetValue(CommandProperty) as ICommand;
        }

        /// <summary>
        /// Sets the value for the CommandParameter attached property on the provided <see cref="Control"/>.
        /// </summary>
        /// <param name="control">Control to attach CommandParameter</param>
        /// <param name="parameter">Parameter value to attach</param>
        public static void SetCommandParameter(Control control, object parameter)
        {
            if (control == null) throw new ArgumentNullException("control");
            control.SetValue(CommandParameterProperty, parameter);
        }

        /// <summary>
        /// Gets the value in CommandParameter attached property on the provided <see cref="Control"/>
        /// </summary>
        /// <param name="control">Control that has the CommandParameter</param>
        /// <returns>The value of the property</returns>
        public static object GetCommandParameter(Control control)
        {
            if (control == null) throw new ArgumentNullException("control");
            return control.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Gets the value in EventName attached property on the provided <see cref="Control"/>
        /// </summary>
        /// <param name="control">Control that has the CommandParameter</param>
        /// <returns>The value of the property</returns>
        public static object GetEventName(Control control)
        {
            if (control == null) throw new ArgumentNullException("control");
            return control.GetValue(EventNameProperty);
        }

        /// <summary>
        /// Sets the value for the EventName attached property on the provided <see cref="Control"/>.
        /// </summary>
        /// <param name="control">Control to attach CommandParameter</param>
        /// <param name="parameter">Parameter value to attach</param>
        public static void SetEventName(Control control, object parameter)
        {
            if (control == null) throw new ArgumentNullException("control");
            control.SetValue(EventNameProperty, parameter);
        }

        private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Control control = dependencyObject as Control;
            if (control != null)
            {
                RoutedEventCommandBehavior behavior = GetOrCreateBehavior(control);
                behavior.Command = e.NewValue as ICommand;
            }
        }

        private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Control control = dependencyObject as Control;
            if (control != null)
            {
                RoutedEventCommandBehavior behavior = GetOrCreateBehavior(control);
                behavior.CommandParameter = e.NewValue;
            }
        }

        private static void OnSetEventNameCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Control control = dependencyObject as Control;
            if (control != null)
            {
                RoutedEventCommandBehavior behavior = GetOrCreateBehavior(control);
                behavior.EventName = e.NewValue as string;
            }
        }

        private static RoutedEventCommandBehavior GetOrCreateBehavior(Control control)
        {
            RoutedEventCommandBehavior behavior = control.GetValue(RoutedEventCommandBehaviorProperty) as RoutedEventCommandBehavior;
            if (behavior == null)
            {
                behavior = new RoutedEventCommandBehavior(control);
                control.SetValue(RoutedEventCommandBehaviorProperty, behavior);
            }

            return behavior;
        }
    }

}
