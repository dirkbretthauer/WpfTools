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
