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
