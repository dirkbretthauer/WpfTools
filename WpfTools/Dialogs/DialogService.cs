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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace WpfTools.Dialogs
{
    /// <summary>
    /// Does the plumbing between IDialog and IDialogViewModel and
    /// shows the IDialog in a modal or non-modal way.
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly IList<IDialog> _openDialogs;
        private readonly SynchronizationContext _syncContext;
        private bool _isClosingAllDialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        public DialogService()
        {
            _openDialogs = new List<IDialog>();
            _syncContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Cancels and closes all open dialogs.
        /// </summary>
        public void CloseAllDialogs()
        {
            if(_syncContext != null)
            {
                _syncContext.Send(s => InternalCloseAllDialogs(), null);
            }
        }

        /// <summary>
        /// Cancels and closes the specified dialog, if it is open.
        /// </summary>
        public void CloseDialog<TView>() where TView : IDialog
        {
            var temp = _openDialogs.OfType<TView>().ToList();

            temp.ForEach(y =>
                {
                    if (y.IsActive)
                    {
                        y.DialogResult = false;
                    }
                });
        }

        private void InternalCloseAllDialogs()
        {
            _isClosingAllDialogs = true;

            var temp = new List<IDialog>(_openDialogs);
            //dialogs must be closed in reverse shown order
            temp.Reverse();
            foreach (IDialog view in temp)
            {
                // cancels and closes the dialog
                // it can be that a dialog is
                // already closed.
                if (view.IsActive)
                {
                    view.DialogResult = false;
                }
            }

            _isClosingAllDialogs = false;
        }

        /// <summary>
        /// Shows a dialog in a non-modal way, connects the specified IDialog to
        /// the IDialogViewModel.
        /// Calls the specfied OnDialogClose action - if specified - when the dialog is closed.
        /// </summary>
        /// <param name="view">The <see cref="IDialog"/> view which will be shown.</param>
        /// <param name="viewModel">The <see cref="IDialogViewModel"/> is set as DataContext for the <paramref name="view"/>.</param>
        /// <param name="onDialogClose">Callback which is called after the dialog is closed.</param>
        /// <param name="state">Object to transport a state to the <paramref name="onDialogClose"/> callback.</param>
        public void Show(Type view, IDialogViewModel viewModel, OnDialogClose onDialogClose, object state = null)
        {
            if(IsClosing(onDialogClose, viewModel, state))
                return;
            
            IDialog dialog = PrepareDialog(onDialogClose, viewModel, state, view);

            dialog.Show();
        }

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
        public bool? ShowDialog(Type view, IDialogViewModel viewModel, OnDialogClose onDialogClose, object state = null)
        {
            if (IsClosing(onDialogClose, viewModel, state))
                return null;

            IDialog dialog = PrepareDialog(onDialogClose, viewModel, state, view);
            
            return dialog.ShowDialog();
        }

        private IDialog PrepareDialog(OnDialogClose onDialogClose, IDialogViewModel viewModel, object state, Type view)
        {
            IDialog dialog = CreateDialog(view);
            ConnectViewToViewModel(dialog, viewModel, onDialogClose, state);

            if (Application.Current != null)
            {
                dialog.Owner = Application.Current.MainWindow;
            }
            return dialog;
        }

        private bool IsClosing(OnDialogClose onDialogClose, IDialogViewModel viewModel, object state)
        {
            if (_isClosingAllDialogs)
            {
                if (onDialogClose != null)
                {
                    onDialogClose(viewModel, false, state);
                }

                return true;
            }

            return false;
        }

        private void ConnectViewToViewModel(IDialog view, IDialogViewModel viewModel,  OnDialogClose onDialogClose, object state)
        {
            EventHandler<RequestCloseEventArgs> requestCloseHandler = delegate(object sender, RequestCloseEventArgs args)
            {
                view.DialogResult = args.DialogResult;
            };

            AddDialog(view, viewModel);

            view.Closed += (sender, args) =>
            {
                RemoveDialog(view);

                if (onDialogClose != null)
                {
                    onDialogClose(viewModel, view.DialogResult, state);
                }
                viewModel.RequestClose -= requestCloseHandler;

                var disposable = viewModel as IDisposable;
                if(disposable != null)
                {
                    disposable.Dispose();
                }
            };


            viewModel.RequestClose += requestCloseHandler;
        }

        private void AddDialog(IDialog view, IDialogViewModel viewModel)
        {
            _openDialogs.Add(view);
            view.DataContext = viewModel;
        }

        private void RemoveDialog(IDialog view)
        {
            _openDialogs.Remove(view);
        }

        /// <summary>
        /// Creates the specified dialog.
        /// </summary>
        /// <param name="dialogType">The <see cref="Type"/> of the Dialog to be created.</param>
        /// <returns>An instance of <paramref name="dialogType"/></returns>
        /// <exception cref="ArgumentException"> if <paramref name="dialogType"/> does not implement <see cref="IDialog"/>.</exception>
        private static IDialog CreateDialog(Type dialogType)
        {
            IDialog result = null;

            ConstructorInfo constructorInfo = dialogType.GetConstructor(new Type[] {});
            if(constructorInfo != null)
            {
                result = constructorInfo.Invoke(null) as IDialog;
            }

            if (result == null)
            {
                throw new ArgumentException("Specified type {0} is not an IDialog", dialogType.Name);
            }

            return result;
        }
    }
}
