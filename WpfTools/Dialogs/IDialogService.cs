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
    /// Callback method, which is called when a dialog is closed.
    /// The dialogResult delivers information how the dialog was 
    /// closed, cancelled or accepted.
    /// </summary>
    public delegate void OnDialogClose(IDialogViewModel model, bool? dialogResult, object state);

    /// <summary>
    /// Does the plumbing between IDialog and IDialogViewModel and
    /// shows the IDialog.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Cancels and closes all open dialogs.
        /// </summary>
        void CloseAllDialogs();

        /// <summary>
        /// Cancels and closes the specified dialog.
        /// </summary>
        void CloseDialog<TView>() where TView : IDialog;

        /// <summary>
        /// Shows a dialog in a non-modal way, connects the specified IDialog to
        /// the IDialogViewModel.
        /// Calls the specfied OnDialogClose action - if specified - when the dialog is closed.
        /// </summary>
        /// <param name="view">The <see cref="IDialog"/> view which will be shown.</param>
        /// <param name="viewModel">The <see cref="IDialogViewModel"/> is set as DataContext for the <paramref name="view"/>.</param>
        /// <param name="onDialogClose">Callback which is called after the dialog is closed.</param>
        /// <param name="state">Object to transport a state to the <paramref name="onDialogClose"/> callback.</param>
        void Show(Type view, IDialogViewModel viewModel, OnDialogClose onDialogClose, object state = null);

        /// <summary>
        /// Shows a dialog in a modal way, connects the specified IDialog to
        /// the IDialogViewModel. The state object is passed to the OnDialogClose callback.
        /// Calls the specfied OnDialogClose action - if specified - when the dialog is closed.
        /// </summary>
        /// <param name="view">The <see cref="IDialog"/> view which will be shown.</param>
        /// <param name="viewModel">The <see cref="IDialogViewModel"/> is set as DataContext for the <paramref name="view"/>.</param>
        /// <param name="onDialogClose">Callback which is called after the dialog is closed.</param>
        /// <param name="state">Object to transport a state to the <paramref name="onDialogClose"/> callback.</param>
        /// <returns>A Nullable value of type Boolean that signifies how a window was closed by the user.</returns>
        bool? ShowDialog(Type view, IDialogViewModel viewModel, OnDialogClose onDialogClose, object state = null);
    }
}
