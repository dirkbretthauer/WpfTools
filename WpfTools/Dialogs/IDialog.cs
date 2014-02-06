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

namespace WpfTools.Dialogs
{
    /// <summary>
    /// Specifies an IDialog.
    /// A WPF window class already implements the
    /// properties, methods and the event of this interface.
    /// </summary>
    public interface IDialog
    {
        /// <summary>
        /// Occurs when the dialog as about to close.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Gets or sets the Window that owns this dialog.
        /// </summary>
        /// <value>
        /// The owner of this dialog.
        /// </value>
        Window Owner { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the dialog is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        bool IsActive { get; }

        /// <summary>
        /// Gets or sets the data context for an element to participate in data binding
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        object DataContext { get; set; }

        /// <summary>
        /// Opens a dialog and returns without waiting for the newly opened dialog to close.
        /// </summary>
        void Show();

        /// <summary>
        /// Opens a dialog and returns only when the newly opened dialog is closed.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the dialog is closed via the 'Ok' Button; otherwise, <c>false</c>.
        /// </returns>
        bool? ShowDialog();

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets or sets the dialog result value, which is the value that is returned from the ShowDialog method.
        /// </summary>
        /// <value>
        /// The dialog result.
        /// </value>
        bool? DialogResult { get; set; }

    }
}
