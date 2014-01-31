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

namespace WpfTools.Dialogs
{
    /// <summary>
    /// ViewModel (or DataContext) of an IDialog.
    /// </summary>
    public interface IDialogViewModel
    {
        /// <summary>
        /// Raised when the Dialog should be removed from the UI.
        /// </summary>
        event EventHandler<RequestCloseEventArgs> RequestClose;

        /// <summary>
        /// Provides the user input of the dialog.
        /// </summary>
        string Result { get; }

        /// <summary>
        /// Provides the Result of the dialog.
        /// </summary>
        object ResultObject { get; }
    }
}
